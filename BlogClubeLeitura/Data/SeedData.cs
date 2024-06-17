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
                ProfilePicture = "/images/profiles/admin.jpg",
                EmailConfirmed = true,
                NormalizedUserName = "ADMIN@BLOG.COM",
                NormalizedEmail = "ADMIN@BLOG.COM"
            };
            if (userManager.Users.All(u => u.UserName != adminUser.UserName))
            {
                var user = await userManager.FindByEmailAsync(adminUser.Email);
                if (user == null)
                {
                    var result = await userManager.CreateAsync(adminUser, "Admin123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }

            // Gestor User
            var gestorUser = new ApplicationUser
            {
                UserName = "gestor@blog.com",
                Email = "gestor@blog.com",
                ProfilePicture = "/images/profiles/gestor.png",
                EmailConfirmed = true,
                NormalizedUserName = "GESTOR@BLOG.COM",
                NormalizedEmail = "GESTOR@BLOG.COM"
            };
            if (userManager.Users.All(u => u.UserName != gestorUser.UserName))
            {
                var user = await userManager.FindByEmailAsync(gestorUser.Email);
                if (user == null)
                {
                    var result = await userManager.CreateAsync(gestorUser, "Gestor123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(gestorUser, "Gestor");
                    }
                }
            }

            // Normal User
            var normalUser = new ApplicationUser
            {
                UserName = "user@blog.com",
                Email = "user@blog.com",
                ProfilePicture = "/images/profiles/user.png",
                EmailConfirmed = true,
                NormalizedUserName = "USER@BLOG.COM",
                NormalizedEmail = "USER@BLOG.COM"
            };
            if (userManager.Users.All(u => u.UserName != normalUser.UserName))
            {
                var user = await userManager.FindByEmailAsync(normalUser.Email);
                if (user == null)
                {
                    var result = await userManager.CreateAsync(normalUser, "User123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(normalUser, "User");
                    }
                }
            }
        }

        private static void SeedBooks(ApplicationDbContext context)
        {
            if (!context.Books.Any())
            {
                var books = new List<Book>();
                for (int i = 1; i <= 20; i++)
                {
                    books.Add(
                      new Book
                      {
                          Title = $"Book{i}",
                          Author = $"Author{i}",
                          Description = $"Description{i}",
                          PublishedDate = DateTime.UtcNow,
                          CoverImagePath = "/images/books/default.png"
                      });
                }

                context.Books.AddRange(books);
                context.SaveChanges();
            }
        }

        private static void SeedPosts(ApplicationDbContext context)
        {
            if (!context.Posts.Any())
            {
                var books = context.Books.ToList();
                var users = context.Users.ToList();

                if (books.Any() && users.Any())
                {
                    foreach (var user in users)
                    {
                        foreach (var book in books)
                        {
                            for (int i = 1; i <= 10; i++)
                            {
                                context.Posts.Add(
                                  new Post
                                  {
                                      Comment = $"This is post {i} about {book.Title} by {user.UserName}.",
                                      PostedDate = DateTime.UtcNow,
                                      BookId = book.Id,
                                      UserId = user.Id,
                                      Rating = new Rating
                                      {
                                          Stars = new Random().Next(1, 6), // Random rating between 1 and 5
                                          RatingDate = DateTime.UtcNow,
                                          BookId = book.Id,
                                          UserId = user.Id
                                      }
                                  });
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}