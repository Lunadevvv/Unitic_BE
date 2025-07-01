using Microsoft.AspNetCore.Identity;

using Unitic_BE.Entities;


namespace Unitic_BE.Seeder
{
    public static class Seeder
    {
        public static async Task SeedAdminDataAsync(UserManager<User> userManager)
        {
            //nếu đã có tài khoản admin thì không cần seed lại
            //tránh lỗi tracking
            var result = await userManager.FindByEmailAsync("caohoangnhat58@gmail.com");
            if (result != null)
            {
                return;
            }


            //tài khoản admin
            var admin = User.Create("User0001", string.Empty, "caohoangnhat58@gmail.com", "Tony", "Jugo", "Uni0001");
            admin.PasswordHash = userManager.PasswordHasher.HashPassword(admin, "12345678");
            //create query user trên normalizedname trùng thì ném exception
            //sau đó insert data user vào db
            await userManager.CreateAsync(admin);
            var addRoleResult = await userManager.AddToRoleAsync(admin, "Admin");
            //tài khoản moderator
            var moderator = User.Create("User0002", string.Empty, "caohoangnhat59@gmail.com", "Tony", "Jugo", "Uni0001");
            moderator.PasswordHash = userManager.PasswordHasher.HashPassword(moderator, "12345678");
            await userManager.CreateAsync(moderator);
            var addRoleResult2 = await userManager.AddToRoleAsync(moderator, "Moderator");
            //tài khoản organizer
            var organizer = User.Create("User0003", string.Empty, "caohoangnhat60@gmail.com", "Tony", "Jugo", "Uni0001");
            organizer.PasswordHash = userManager.PasswordHasher.HashPassword(organizer, "12345678");
            await userManager.CreateAsync(organizer);
            var addRoleResult3 = await userManager.AddToRoleAsync(organizer, "Organizer");

        }
    }
}
