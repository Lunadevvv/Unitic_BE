using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unitic_BE;
using Unitic_BE.Entities;
using Unitic_BE.Enums;
using Unitic_BE.Exceptions;
using Unitic_BE.Requests;

namespace WebTicket.Infrastructure.Seeder
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
            var user = User.Create("User0001", string.Empty, "caohoangnhat58@gmail.com", "Tony", "Jugo", "Uni0001");
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, "12345678"); 
            await userManager.CreateAsync(user);
            var addRoleResult = await userManager.AddToRoleAsync(user, "Admin");
            await userManager.UpdateAsync(user);
            //tài khoản moderator
            var user2 = User.Create("User0002", string.Empty, "caohoangnhat59@gmail.com", "Tony", "Jugo", "Uni0001");
            user2.PasswordHash = userManager.PasswordHasher.HashPassword(user2, "12345678");
            await userManager.CreateAsync(user2);
            var addRoleResult2 = await userManager.AddToRoleAsync(user2, "Moderator");
            await userManager.UpdateAsync(user2);
            //tài khoản organizer
            var user3 = User.Create("User0003", string.Empty, "caohoangnhat60@gmail.com", "Tony", "Jugo", "Uni0001");
            user3.PasswordHash = userManager.PasswordHasher.HashPassword(user3, "12345678");
            await userManager.CreateAsync(user3);
            var addRoleResult3 = await userManager.AddToRoleAsync(user3, "Organizer");
            await userManager.UpdateAsync(user3);

        }
    }
}
