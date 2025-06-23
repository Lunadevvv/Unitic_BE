namespace Unitic_BE.Constants;

//static class để không instantiate
//Entitiy của bảng IdentityRole
public static class IdentityRoleConstants
{
    //khai bao static để truy cập trực tiếp qua class name
    public static readonly string AdminRoleString = new("000000000001");

    public static readonly string ModeratorRoleString = new("000000000002");
    public static readonly string OrganizerRoleString = new("000000000003");
    public static readonly string StaffRoleString = new("000000000004");
    public static readonly string UserRoleString = new("000000000005");

    public const string Admin = "Admin";
    public const string Staff = "Staff";
    public const string Moderator = "Moderator";
    public const string Organizer = "Organizer";
    public const string User = "User";
}