using Microsoft.AspNetCore.Identity;
using Unitic_BE.Abstracts;
using Unitic_BE.Constants;
using Unitic_BE.Entities;
using Unitic_BE.Enums;
using Unitic_BE.DTOs.Requests;
using Unitic_BE.Exceptions;
using Unitic_BE.Contracts;

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
    public async Task RegisterRoleAsync(string role, RegisterRequest registerRequest)
    {
        //trim all
        StringTrimmerExtension.TrimAllString(registerRequest);
        //check xem user đã tồn tại chưa
        var userExists = await _userManager.FindByEmailAsync(registerRequest.Email) != null;

        if (userExists)
        {
            throw new UserAlreadyExistsException(email: registerRequest.Email);
        }
        //check xem role hợp lệ ko
        if (!Enum.TryParse(role, false, out Role parsedRole))
        {
            throw new RegistrationFailedException(new List<string>
            {
                $"\nRole '{role}' is not a valid role.",
                "\nValid roles are: Admin, Moderator, Staff, Organizer, User."
            });
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
        await _userManager.CreateAsync(user);
        //gán user với role vào bảng ASpNetUserRoles
        var addRoleResult = await _userManager.AddToRoleAsync(user, role);
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

    public async Task ResetPassword(string? userId, string newPassword)
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

    public async Task<List<User>> GetAllUsers()
    {
        List<User> users = await _userRepository.GetAllUsers();
        return users;
    }

    public async Task<User> GetUserById(string accountId)
    {
        if (accountId == null)
            throw new Exception("AccountID is empty");
        User user = await _userRepository.GetUserById(accountId);
        return user;
    }
}