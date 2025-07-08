
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Unitic_BE.Constants;
using Unitic_BE.Entities;

namespace Unitic_BE;

//khóa chính là string
public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<string>, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<University> Universities { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Payment> Payments { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)

    {
        base.OnModelCreating(builder);

        //quy định độ dài của các trường trong bảng User
        builder.Entity<User>()
            .Property(u => u.FirstName).HasMaxLength(256);

        builder.Entity<User>()
            .Property(u => u.LastName).HasMaxLength(256);
        //Seed data mặc định vào bảng university
        builder.Entity<University>()
            .HasData(new List<University>
            {
                new University
                {
                    Id = "Uni0001", // Id là khóa chính, có thể là Guid hoặc string
                    Name = "Đại học FPT",
                },
                new University
                {
                    Id = "Uni0002",
                    Name = "Đại học Bách Khoa",
                },
                new University
                {
                    Id = "Uni0003",
                    Name = "Đại học Khoa Học Tự Nhiên",
                },
                new University
                {
                    Id = "Uni0004",
                    Name = "Đại học Công Nghệ Thông Tin",
                }
            });

        //Seed data mặc định vào bảng asp.net role
        builder.Entity<IdentityRole<string>>()
            .HasData(new List<IdentityRole<string>>
            {
                new IdentityRole<string>()
                {
                    Id = IdentityRoleConstants.AdminRoleString,
                    Name = IdentityRoleConstants.Admin,
                    NormalizedName = IdentityRoleConstants.Admin.ToUpper()
                },
                new IdentityRole<string>()
                {
                    Id = IdentityRoleConstants.ModeratorRoleString,
                    Name = IdentityRoleConstants.Moderator,
                    NormalizedName = IdentityRoleConstants.Moderator.ToUpper()
                },

                new IdentityRole<string>()
                {
                    Id = IdentityRoleConstants.OrganizerRoleString,
                    Name = IdentityRoleConstants.Organizer,
                    NormalizedName = IdentityRoleConstants.Organizer.ToUpper()
                },

                new IdentityRole<string>()
                {
                    Id = IdentityRoleConstants.StaffRoleString,
                    Name = IdentityRoleConstants.Staff,
                    NormalizedName = IdentityRoleConstants.Staff.ToUpper()
                },

                new IdentityRole<string>()
                {
                    Id = IdentityRoleConstants.UserRoleString,
                    Name = IdentityRoleConstants.User,
                    NormalizedName = IdentityRoleConstants.User.ToUpper()
                }

            });
        //seed data vào category
        builder.Entity<Category>()
           .HasData(new List<Category>
           {
                new Category
                {
                    CateID = "Cate0001",
                    Name = "Entertainment",
                },
                new Category
                {
                    CateID = "Cate0002",
                    Name = "Education",
                },
                new Category
                {
                    CateID = "Cate0003",
                    Name = "Sharing",
                },
                new Category
                {
                    CateID = "Cate0004",
                    Name = "Music",
                }
           });
        //các mqh
        builder.Entity<User>()
            .HasOne(u => u.University)
            .WithMany(u => u.Users)
            .HasForeignKey(u => u.UniversityId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Entity<Event>()
            .HasOne(e => e.Category)
            .WithMany(u => u.Events)
            .HasForeignKey(e => e.CateID);


    }
}