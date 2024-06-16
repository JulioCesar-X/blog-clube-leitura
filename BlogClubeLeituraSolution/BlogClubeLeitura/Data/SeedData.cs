using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BlogClubeLeitura.Models;

namespace BlogClubeLeitura.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string seedVersion)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Verifica se a seed já foi aplicada
                if (context.SeedHistories.Any(sh => sh.SeedVersion == seedVersion))
                {
                    return; // Seed já aplicada
                }

                // Executa os métodos de seeding
                await CreateUsers(serviceProvider);
                SeedBooks(context);
                SeedPosts(context);
                SeedRatings(context);

                // Adicione um registro no SeedHistory
                context.SeedHistories.Add(new SeedHistory
                {
                    SeedVersion = seedVersion,
                    AppliedOn = DateTime.UtcNow
                });

                context.SaveChanges();
            }
        }

        private static async Task CreateUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roleNames = { "Admin", "Gestor", "User" };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Admin User
            var adminUser = new ApplicationUser
            {
                UserName = "admin@blog.com",
                Email = "admin@blog.com",
                ProfilePicture = "/images/profiles/admin.jpg"
            };
            if (userManager.Users.All(u => u.UserName != adminUser.UserName))
            {
                var user = await userManager.FindByEmailAsync(adminUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(adminUser, "Admin123!");
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Gestor User
            var gestorUser = new ApplicationUser
            {
                UserName = "gestor@blog.com",
                Email = "gestor@blog.com",
                ProfilePicture = "/images/profiles/gestor.png"
            };
            if (userManager.Users.All(u => u.UserName != gestorUser.UserName))
            {
                var user = await userManager.FindByEmailAsync(gestorUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(gestorUser, "Gestor123!");
                    await userManager.AddToRoleAsync(gestorUser, "Gestor");
                }
            }

            // Normal User
            var normalUser = new ApplicationUser
            {
                UserName = "user@blog.com",
                Email = "user@blog.com",
                ProfilePicture = "/images/profiles/user.png"
            };
            if (userManager.Users.All(u => u.UserName != normalUser.UserName))
            {
                var user = await userManager.FindByEmailAsync(normalUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(normalUser, "User123!");
                    await userManager.AddToRoleAsync(normalUser, "User");
                }
            }
        }

        private static void SeedBooks(ApplicationDbContext context)
        {
            if (!context.Books.Any())
            {
                context.Books.AddRange(
                    new Book
                    {
                        Title = "Book1",
                        Author = "Author1",
                        Description = "Description1",
                        PublishedDate = DateTime.UtcNow,
                        CoverImagePath = "/images/books/default.png"
                    },
                    new Book
                    {
                        Title = "Book2",
                        Author = "Author2",
                        Description = "Description2",
                        PublishedDate = DateTime.UtcNow,
                        CoverImagePath = "/images/books/default.png"
                    }
                );
                context.SaveChanges();
            }
        }

        private static void SeedPosts(ApplicationDbContext context)
        {
            if (!context.Posts.Any())
            {
                var book1 = context.Books.FirstOrDefault(b => b.Title == "Book1");
                var book2 = context.Books.FirstOrDefault(b => b.Title == "Book2");
                var user = context.Users.FirstOrDefault(u => u.UserName == "user@blog.com");

                if (book1 != null && book2 != null && user != null)
                {
                    context.Posts.AddRange(
                        new Post
                        {
                            Content = "This is a post about Book1.",
                            PostedDate = DateTime.UtcNow,
                            BookId = book1.Id,
                            UserId = user.Id
                        },
                        new Post
                        {
                            Content = "This is a post about Book2.",
                            PostedDate = DateTime.UtcNow,
                            BookId = book2.Id,
                            UserId = user.Id
                        }
                    );
                    context.SaveChanges();
                }
            }
        }

        private static void SeedRatings(ApplicationDbContext context)
        {
            if (!context.Ratings.Any())
            {
                var book1 = context.Books.FirstOrDefault(b => b.Title == "Book1");
                var book2 = context.Books.FirstOrDefault(b => b.Title == "Book2");
                var user = context.Users.FirstOrDefault(u => u.UserName == "user@blog.com");

                if (book1 != null && book2 != null && user != null)
                {
                    context.Ratings.AddRange(
                        new Rating
                        {
                            Stars = 5,
                            Comment = "Great book!",
                            RatingDate = DateTime.UtcNow,
                            BookId = book1.Id,
                            UserId = user.Id
                        },
                        new Rating
                        {
                            Stars = 4,
                            Comment = "Good read.",
                            RatingDate = DateTime.UtcNow,
                            BookId = book2.Id,
                            UserId = user.Id
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}