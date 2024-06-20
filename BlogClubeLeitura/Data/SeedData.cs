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

            // Normal Users
            for (int i = 1; i <= 10; i++)
            {
                var normalUser = new ApplicationUser
                {
                    UserName = $"user{i}@blog.com",
                    Email = $"user{i}@blog.com",
                    ProfilePicture = $"/images/profiles/user{i}.png",
                    EmailConfirmed = true,
                    NormalizedUserName = $"USER{i}@BLOG.COM",
                    NormalizedEmail = $"USER{i}@BLOG.COM"
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
        }

        private static void SeedBooks(ApplicationDbContext context)
        {
            if (!context.Books.Any())
            {
                var books = new List<Book>
                {
                    new Book
                    {
                        Title = "O Segredo nas Sombras",
                        Author = "Morgana Moraes",
                        Description = "Este livro é um suspense aterrorizante que mergulha em eventos misteriosos ocorridos em uma data específica em março de 1996.",
                        PublishedYear = 2023,
                        CoverImagePath = "/images/books/1.jpeg"
                    },
                    new Book
                    {
                        Title = "A Modista Vintage",
                        Author = "Jaqueline Silva",
                        Description = "Este livro foca no retorno da moda vintage, fornecendo insights e segredos para mulheres modernas que apreciam e querem incorporar estilos vintage em seu guarda-roupa.",
                        PublishedYear = 2022,
                        CoverImagePath = "/images/books/2.jpeg"
                    },
                    new Book
                    {
                        Title = "A Rapariga no Abismo",
                        Author = "Charlie Gallagher",
                        Description = "Uma história envolvente sobre uma garota que foge de sua vida problemática e luta pela sobrevivência. O livro combina elementos de mistério e suspense.",
                        PublishedYear = 2020,
                        CoverImagePath = "/images/books/3.jpeg"
                    },
                    new Book
                    {
                        Title = "Robin Williams: A Biografia",
                        Author = "Emily Herbert",
                        Description = "Esta biografia explora a vida e a carreira do amado ator e comediante Robin Williams, oferecendo insights sobre suas experiências pessoais e profissionais.",
                        PublishedYear = 2014,
                        CoverImagePath = "/images/books/4.jpeg"
                    },
                    new Book
                    {
                        Title = "Viagem ao Centro da Terra",
                        Author = "Júlio Verne",
                        Description = "Um romance clássico de ficção científica que leva os leitores em uma jornada aventureira ao centro da Terra, repleta de encontros imaginativos e emocionantes.",
                        PublishedYear = 1864,
                        CoverImagePath = "/images/books/5.jpeg"
                    },
                    new Book
                    {
                        Title = "Harry Potter e a Pedra Filosofal",
                        Author = "J.K. Rowling",
                        Description = "O primeiro livro da série Harry Potter, apresentando o jovem bruxo Harry Potter e suas aventuras iniciais na Escola de Magia e Bruxaria de Hogwarts.",
                        PublishedYear = 1997,
                        CoverImagePath = "/images/books/6.jpeg"
                    },
                    new Book
                    {
                        Title = "Harry Potter e a Câmara Secreta",
                        Author = "J.K. Rowling",
                        Description = "O segundo livro da série Harry Potter, onde Harry retorna a Hogwarts e desvenda o mistério da Câmara Secreta.",
                        PublishedYear = 1998,
                        CoverImagePath = "/images/books/7.jpeg"
                    },
                    new Book
                    {
                        Title = "Harry Potter and the Deathly Hallows",
                        Author = "J.K. Rowling",
                        Description = "O livro final da série Harry Potter, onde Harry enfrenta sua batalha final contra Voldemort.",
                        PublishedYear = 2007,
                        CoverImagePath = "/images/books/8.jpeg"
                    },
                    new Book
                    {
                        Title = "Avaliando a Inteligência Emocional",
                        Author = "Steve Simmons e John C. Simmons Jr.",
                        Description = "Este livro explora o conceito de inteligência emocional, fornecendo insights e estratégias para entender e melhorar as capacidades emocionais para o sucesso pessoal e profissional.",
                        PublishedYear = 2003,
                        CoverImagePath = "/images/books/9.jpeg"
                    },
                    new Book
                    {
                        Title = "Aprenda a Programar C#",
                        Author = "Fabiela Ventavoli",
                        Description = "Um guia para aprender programação em C#, cobrindo os tópicos básicos e avançados para ajudar os leitores a desenvolver aplicações usando o Microsoft Visual Studio.",
                        PublishedYear = 2010,
                        CoverImagePath = "/images/books/10.jpeg"
                    },
                    new Book
                    {
                        Title = "Python Levado a Sério",
                        Author = "Julien Danjou",
                        Description = "Conselhos de um faixa-preta sobre implantação, escalabilidade, testes e outros assuntos relacionados a Python.",
                        PublishedYear = 2016,
                        CoverImagePath = "/images/books/11.jpeg"
                    },
                    new Book
                    {
                        Title = "Clean Code",
                        Author = "Robert C. Martin",
                        Description = "Um manual de artesanato de software ágil que oferece princípios e melhores práticas para escrever código limpo e sustentável.",
                        PublishedYear = 2008,
                        CoverImagePath = "/images/books/12.jpeg"
                    },
                    new Book
                    {
                        Title = "Cosmos",
                        Author = "Carl Sagan",
                        Description = "Uma exploração do universo, cobrindo as origens da vida e a compreensão humana do cosmos.",
                        PublishedYear = 1980,
                        CoverImagePath = "/images/books/13.jpeg"
                    },
                    new Book
                    {
                        Title = "Está a Brincar, Sr. Feynman!",
                        Author = "Richard P. Feynman",
                        Description = "Uma coleção de anedotas da vida do físico Richard Feynman, mostrando sua natureza curiosa e brincalhona.",
                        PublishedYear = 1985,
                        CoverImagePath = "/images/books/14.jpeg"
                    },
                    new Book
                    {
                        Title = "O Mundo de Sofia",
                        Author = "Jostein Gaarder",
                        Description = "Um romance que serve como um guia introdutório à filosofia, seguindo uma jovem chamada Sofia que recebe cartas misteriosas sobre questões filosóficas.",
                        PublishedYear = 1991,
                        CoverImagePath = "/images/books/15.jpeg"
                    }
                };

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
                    var random = new Random();
                    var comments = new List<string>
            {
                "Uma leitura cativante, não consegui parar!",
                "Achei a trama um pouco lenta no início, mas depois melhorou.",
                "O desenvolvimento dos personagens neste livro é fenomenal.",
                "Uma trama intrigante com reviravoltas inesperadas.",
                "O estilo de escrita do autor é imersivo e envolvente.",
                "Adorei o contexto histórico deste livro.",
                "Um pouco técnico demais para o meu gosto, mas ainda assim informativo.",
                "Os temas explorados neste livro são muito relevantes hoje.",
                "Uma história linda e comovente.",
                "Este livro mudou minha perspectiva sobre muitas coisas."
            };

                    foreach (var book in books)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            var commentIndex = random.Next(comments.Count);
                            var randomUserIndex = random.Next(users.Count);
                            var randomUser = users[randomUserIndex];
                            var randomYear = DateTime.UtcNow.Year - random.Next(0, 5);
                            var randomMonth = random.Next(1, 13);
                            var randomDay = random.Next(1, DateTime.DaysInMonth(randomYear, randomMonth) + 1);

                            var randomDate = new DateTime(randomYear, randomMonth, randomDay);

                            context.Posts.Add(
                              new Post
                              {
                                  Comment = comments[commentIndex],
                                  PostedDate = randomDate,
                                  BookId = book.Id,
                                  UserId = randomUser.Id,
                                  Rating = new Rating
                                  {
                                      Stars = random.Next(1, 6),
                                      RatingDate = randomDate,
                                      BookId = book.Id,
                                      UserId = randomUser.Id
                                  }
                              });
                        }
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}