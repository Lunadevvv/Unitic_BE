using Microsoft.AspNetCore.Identity;
using Unitic_BE.Abstracts;
using Unitic_BE.Constants;
using Unitic_BE.Entities;
using Unitic_BE.Enums;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Exceptions;
using Unitic_BE.Contracts;
using Unitic_BE.DTOs.Responses;

namespace Unitic_BE.Services;

public class AccountService : IAccountService
{
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;
    private readonly CustomValidator _validator;
    private readonly IUniversityService _universityService;
    public AccountService(IAuthTokenProcessor authTokenProcessor, UserManager<User> userManager, CustomValidator validator,
        IUserRepository userRepository, IUniversityService universityService)
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _userRepository = userRepository;
        _validator = validator;
        _universityService = universityService;
    }

    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        //trim all
        StringTrimmerExtension.TrimAllString(registerRequest);
        //check xem user đã tồn tại chưa
        var userExists = await _userManager.FindByEmailAsync(registerRequest.Email) != null;

        if (userExists)
        {
            throw new UserAlreadyExistsException(email: registerRequest.Email);
        }
        //validate registerRequest
        List<string> universityNames = await _universityService.GetAllUniversityNames();
        var (erros, isValid) = _validator.ValidateUser(registerRequest, universityNames);
        // nếu không thành công do validate thì ném ra exception
        if (!isValid)
        {
            throw new RegistrationFailedException(erros);
        }
        //tìm university và tạo id user

        var universityId = await _userRepository.GetUniversityIdByNameAsync(registerRequest.UniversityName);
        string id = await GenerateUserId();
        //tạo user mới
        var user = User.Create(id, registerRequest.Mssv, registerRequest.Email, registerRequest.FirstName, registerRequest.LastName, universityId);
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, registerRequest.Password); //hash password trước khi lưu vào bảng AspNetUsers
                                                                                                      //gọi hàm CreateAsync để vừa check validate vừa lưu vào bảng AspNetUsers
        user.Role = Role.User; //gán role mặc định là User
        await _userManager.CreateAsync(user);
        //gán user với role vào bảng ASpNetUserRoles
        var addRoleResult = await _userManager.AddToRoleAsync(user, GetStringIdentityRoleName(Role.User));
    }

    public async Task<string> LoginAsync(LoginRequest loginRequest)
    {
        //trim all
        StringTrimmerExtension.TrimAllString(loginRequest);
        //tìm user dựa trên login request
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);
        //không có user hoặc mật khẩu không đúng thì ném ra exception
        var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);

        if (user == null || result == false)
        {
            throw new LoginFailedException();
        }
        //gernater jwt token dựa trên role và user
        IList<string> roles = await _userManager.GetRolesAsync(user);

        var (jwtToken, expirationDateInUtc) = _authTokenProcessor.GenerateJwtToken(user, roles);

        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("ACCESS_TOKEN", jwtToken, expirationDateInUtc);
        return jwtToken;
    }

    private string GetStringIdentityRoleName(Role role)
    {
        return role switch
        {
            Role.Moderator => IdentityRoleConstants.Moderator,
            Role.Staff => IdentityRoleConstants.Staff,
            Role.Organizer => IdentityRoleConstants.Organizer,
            Role.Admin => IdentityRoleConstants.Admin,
            Role.User => IdentityRoleConstants.User,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Provided role is not supported.")
        };
    }
    private async Task<string> GenerateUserId()
    {
        string lastId = await _userRepository.GetLastId();
        if (lastId == null) return "User0001";
        int id = int.Parse(lastId.Substring(4)) + 1; // lấy id cuối cùng và cộng thêm 1
        string generatedId = "User" + id.ToString("D4");
        return generatedId;
    }

    public async Task<string> ForgotPassword(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            throw new Exception("User not found!");
        }

        var (resetToken, expirationDateInUtc) = _authTokenProcessor.GenerateResetJwtToken(user);
        _authTokenProcessor.WriteAuthTokenAsHttpOnlyCookie("RESET_TOKEN", resetToken, expirationDateInUtc);
        return resetToken;
    }

    public async Task ResetPassword(string userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found!");
        }
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);

        await _userManager.UpdateAsync(user);
    }

    public async Task ChangePassword(string? userId, ChangePasswordRequest changePasswordRequest)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found!");
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, changePasswordRequest.OldPassword);
        if (!passwordValid)
        {
            throw new Exception("Current password is incorrect!");
        }

        var (erros, isValid) = await _validator.ValidateChangePasswordAsync(changePasswordRequest);
        if (!isValid)
        {
            throw new Exception("Your new password need have 8-16 characters!");
        }

        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, changePasswordRequest.NewPassword);
        await _userManager.UpdateAsync(user);
    }

    public async Task<User> GetCurrentUserAsync(string userId)
    {
        return await _userRepository.GetUserById(userId);
    }

    public Task<bool> CheckMoneySufficent(int money, int userMoney)
    {
        return Task.FromResult(userMoney >= money);
    }

    public async Task<bool> UpdateUserWallet(User user, int money)
    {
        user.wallet += money; // Cập nhật số tiền trong ví của người dùng
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return true;
        }
        return false; // Trả về false nếu cập nhật không thành công
    }

    public async Task<bool> ChangeUserMoney(User user)
    {
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return true;
        }
        var errorMessages = result.Errors.Select(e => e.Description);
        throw new Exception("Failed to update user: " + string.Join("; ", errorMessages));
    }

    public async Task<List<AccountResponse>> GetAllUsers()
    {
        List<User> users = await _userRepository.GetAllUsers();
        List<AccountResponse> accountResponses = new List<AccountResponse>();
        foreach (var user in users)
        {
            var accountResponse = new AccountResponse
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Mssv = user.Mssv,
                Wallet = user.wallet,
                University = await _universityService.GetUniversityById(user.UniversityId), // Assuming University is a navigation property
                Role = user.Role // Assuming Role is a property of User
            };
            accountResponses.Add(accountResponse);
        }
        return accountResponses;
    }

    public async Task<AccountResponse> GetUserById(string accountId)
    {
        if (accountId == null)
            throw new Exception("AccountID is empty");
        User user = await _userRepository.GetUserById(accountId);
        return new AccountResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Mssv = user.Mssv,
            Wallet = user.wallet,
            University = await _universityService.GetUniversityById(user.UniversityId), // Assuming University is a navigation property
            Role = user.Role
        };
    }

    public async Task UpdateAccountRoleAsync(string accountId, Role role)
    {
        User user = await _userRepository.GetUserById(accountId);

        //update user's role in the database
        if (user == null)
        {
            throw new ObjectNotFoundException($"User with id {accountId} not found.");
        }
        var currentRoles = await _userManager.GetRolesAsync(user);
        if (currentRoles.Contains(GetStringIdentityRoleName(role)))
        {
            throw new Exception($"User with id {accountId} already has the role {role}.");
        }
        // Remove the user from all current roles
        foreach (var currentRole in currentRoles)
        {
            var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, currentRole);
            if (!removeRoleResult.Succeeded)
            {
                throw new Exception($"Failed to remove user from role {currentRole}: {string.Join(", ", removeRoleResult.Errors.Select(e => e.Description))}");
            }
        }
        // Add the user to the new role
        var addRoleResult = await _userManager.AddToRoleAsync(user, GetStringIdentityRoleName(role));
        if (!addRoleResult.Succeeded)
        {
            throw new Exception($"Failed to add user to role {role}: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
        }
        user.Role = role; // Update the user's role property
        await _userManager.UpdateAsync(user);
    }
}