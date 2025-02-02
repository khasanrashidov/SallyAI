using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenAiSample.WebApi.Models.Account;
using OpenAiSample.WebApi.Options;
using OpenApiSample.Data.Entities;
using OpenApiSample.Data.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OpenAiSample.WebApi.Services.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<IdentityService> _logger;
        private readonly RoleManager<Role> _roleManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly AuthSettingsOptions _authSettingsOptions;

        public IdentityService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<IdentityService> logger,
            RoleManager<Role> roleManager,
            ICurrentUserService currentUserService,
            IOptions<AuthSettingsOptions> authSettingsOptions
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _currentUserService = currentUserService;
            _authSettingsOptions = authSettingsOptions.Value;
        }

        public async Task<RegisterResponse> RegisterUserAsync(RegisterViewModel registerModel)
        {
            if (registerModel is null)
                throw new NullReferenceException("Register Model is null")!;

            if (registerModel.Password != registerModel.ConfirmPassword)
                return new RegisterResponse
                {
                    Message = "Confirm password doesn't match the password",
                    IsSuccess = false,
                };

            var user = new User
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email,
                UserName = registerModel.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (result.Succeeded)
            {
                _ = await _userManager.AddToRoleAsync(user, AccountRole.Sales.ToString());

                return new RegisterResponse
                {
                    Message = "User was created successfully!",
                    IsSuccess = true,
                };
            }

            return new RegisterResponse
            {
                Message = "User was not created",
                IsSuccess = false,
                Errors = result.Errors.Select(e => e.Description),
            };
        }

        public async Task<LoginResponse> LoginUserAsync(LoginViewModel loginModel)
        {
            if (
                loginModel is null
                || string.IsNullOrEmpty(loginModel.Email)
                || string.IsNullOrEmpty(loginModel.Password)
            )
            {
                return new LoginResponse
                {
                    Errors = new[] { "User data is empty" },
                    IsSuccess = false,
                };
            }
            var user = await _userManager.FindByEmailAsync(loginModel.Email);

            if (user is null)
            {
                return new LoginResponse
                {
                    Errors = new[] { "User is not found" },
                    IsSuccess = false,
                };
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginModel.Password,
                false,
                false
            );

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in");
            }
            else
            {
                return new LoginResponse
                {
                    Errors = new[] { "Wrong password or email" },
                    IsSuccess = false,
                };
            }

            if (!user.EmailConfirmed)
            {
                return new LoginResponse
                {
                    Errors = new[] { "User is not active" },
                    IsSuccess = false,
                };
            }

            var token = await GenerateJWTTokenWithUserClaimsAsync(user);

            token.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()!;
            token.UserId = user.Id;

            return new LoginResponse
            {
                IsSuccess = true,
                Result = token,
                Message = "User logged in"
            };
        }

        /// <summary>
        /// Generation of JWT token with User claims including Email and role
        /// </summary>
        /// <param name="user">
        /// User object
        /// </param>
        /// <returns>
        /// TokenModel object
        /// </returns>
        private async Task<TokenModel> GenerateJWTTokenWithUserClaimsAsync(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim> { new Claim(ClaimTypes.Email, user.Email!), };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authSettingsOptions.Key));

            var token = new JwtSecurityToken(
                issuer: _authSettingsOptions.Issuer,
                audience: _authSettingsOptions.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenModel { AccessToken = tokenAsString, ExpireDate = token.ValidTo };
        }
    }
}
