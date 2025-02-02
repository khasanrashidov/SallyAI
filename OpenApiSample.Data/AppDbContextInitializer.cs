using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenApiSample.Data.Entities;
using OpenApiSample.Data.Enums;

namespace OpenApiSample.Data
{
    public class AppDbContextInitializer
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public AppDbContextInitializer(AppDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task InitializeAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task TrySeedAsync()
        {
            // Default roles
            var roles = new List<Role>
            {
                new Role { Name = "Administrator" },
                new Role { Name = "TechLead" },
                new Role { Name = "Sales" },
            };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name!))
                {
                    await _roleManager.CreateAsync(role);
                }
            }

            // Default users
            var users = new List<User>
            {
                new User
                {
                    UserName = "super.admin@ventionteams.com",
                    FirstName = "Super",
                    LastName ="admin",
                    Email = "super.admin@ventionteams.com",
                    EmailConfirmed = true
                },
                new User
                {
                    UserName = "john.smith@ventionteams.com",
                    FirstName = "John",
                    LastName = "Smith",
                    Email = "john.smith@ventionteams.com",
                    EmailConfirmed = true
                },
                new User
                {
                     UserName = "makhmudjon.sodikov@ventionteams.com",
                     FirstName = "Makhmudjon",
                     LastName = "Sodikov",
                     Email = "makhmudjon@sodikov.com",
                     EmailConfirmed = true
                }
            };

            foreach (var user in users)
            {
                if ((await _userManager.FindByNameAsync(user.UserName!) is null))
                {
                    await _userManager.CreateAsync(user, "Test@123456");
                    switch (user.UserName)
                    {
                        case "super.admin@ventionteams.com":
                            var adminRole = roles.Find(r =>
                                r.Name == AccountRole.Administrator.ToString()
                            );
                            if (adminRole != null)
                            {
                                await _userManager.AddToRolesAsync(
                                    user,
                                    new List<string> { adminRole.Name! }
                                );
                            }
                            break;
                        case "john.smith@ventionteams.com":
                            var salesRole = roles.Find(r =>
                                r.Name == AccountRole.Sales.ToString()
                            );
                            if (salesRole != null)
                            {
                                await _userManager.AddToRolesAsync(
                                    user,
                                    new List<string> { salesRole.Name! }
                                );
                            }
                            break;

                        case "makhmudjon.sodikov@ventionteams.com":
                            var techLeadRole = roles.Find(r =>
                                r.Name == AccountRole.TechLead.ToString()
                            );
                            if (techLeadRole != null)
                            {
                                await _userManager.AddToRolesAsync(
                                    user,
                                    new List<string> { techLeadRole.Name! }
                                );
                            }
                            break;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
