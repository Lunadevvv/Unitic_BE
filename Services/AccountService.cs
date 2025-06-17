using Microsoft.AspNetCore.Identity;
using Unitic_BE.Abstracts;
using Unitic_BE.Constants;
using Unitic_BE.Entities;
using Unitic_BE.Enums;
using Unitic_BE.Requests;
using Unitic_BE.Exceptions;

namespace Unitic_BE.Services;

public class AccountService : IAccountService
{
    private readonly IAuthTokenProcessor _authTokenProcessor;
    private readonly UserManager<User> _userManager;
    private readonly IUserRepository _userRepository;

    public AccountService(IAuthTokenProcessor authTokenProcessor, UserManager<User> userManager,
        IUserRepository userRepository)
    {
        _authTokenProcessor = authTokenProcessor;
        _userManager = userManager;
        _userRepository = userRepository;
    }

    public async Task RegisterAsync(RegisterRequest registerRequest)
    {
        //check xem user đã tồn tại chưa
        var userExists = await _userManager.FindByEmailAsync(registerRequest.Email) != null;

        if (userExists)
        {
            throw new UserAlreadyExistsException(email: registerRequest.Email);
        }
        //tìm university và tạo id user
        var universityId = await _userRepository.GetUniversityIdByNameAsync(registerRequest.UniversityName.Trim());
        string id = await GenerateUserId();
        //tạo user mới
        var user = User.Create(registerRequest.Password, id, registerRequest.Mssv.Trim() ,registerRequest.Email.Trim(), registerRequest.FirstName.Trim(), registerRequest.LastName.Trim(), registerRequest.PhoneNumber.Trim(), universityId);
        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, registerRequest.Password); //hash password trước khi lưu vào bảng AspNetUsers
        //gọi hàm CreateAsync để vừa check validate vừa lưu vào bảng AspNetUsers
        var result = await _userManager.CreateAsync(user);
        // nếu không thành công do validate thì ném ra exception
        if (!result.Succeeded)
        {
            throw new RegistrationFailedException(result.Errors.Select(x => x.Description));
        }

        //gán user với role vào bảng ASpNetUserRoles
        var addRoleResult = await _userManager.AddToRoleAsync(user, GetStringIdentityRoleName(Role.User));
        await _userManager.UpdateAsync(user); //cập nhật user sau khi thêm role
        if (!addRoleResult.Succeeded)
        {
            // Log hoặc ném exception để biết lỗi
            throw new Exception("Add role failed: " + string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
        }
    }

    public async Task LoginAsync(LoginRequest loginRequest)
    {
        //tìm user dựa trên login request
        var user = await _userManager.FindByEmailAsync(loginRequest.Email);
        //không có user hoặc mật khẩu không đúng thì ném ra exception
        var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
      
        if (user == null || result == false)
        {
            throw new LoginFailedException(loginRequest.Email);
        }
        //gernater jwt token dựa trên role và user
        IList<string> roles = await _userManager.GetRolesAsync(user);

        var (jwtToken, expirationDateInUtc) = _authTokenProcessor.GenerateJwtToken(user, roles);


        user.Token = jwtToken;
        user.TokenExpiresAtUtc = expirationDateInUtc;

        //thêm token vào bảng user
        await _userManager.UpdateAsync(user);
        _authTokenProcessor.WriteAuthTokenAsHeader(jwtToken, expirationDateInUtc);
    }

    public async Task LogoutAsync(string token)
    {
        //tìm user dựa trên token
        var user = await _userRepository.GetUserByTokenAsync(token);
        //không có user thì ném ra exception
        if (user == null)
        {
            throw new TokenException("No User Found");
        }
        //gernater jwt token dựa trên role và user
        IList<string> roles = await _userManager.GetRolesAsync(user);

        var (jwtToken, expirationDateInUtc) = _authTokenProcessor.GenerateExpiredJwtToken(user, roles);
        user.Token = jwtToken; //cập nhật token thành expired token
        user.TokenExpiresAtUtc = expirationDateInUtc; //cập nhật thời gian hết hạn token
        await _userManager.UpdateAsync(user);
        _authTokenProcessor.WriteAuthTokenAsLogOutToken(jwtToken, expirationDateInUtc); //viết token vào header để server biết token đã hết hạn

        //chức năng này chỉ thay thế token header ở phía server, không xóa ở phía client
        //FE sẽ xóa token ở phía client bằng cách xóa localstorage
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

}