using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenAiSample.WebApi.Infrastructure.MediatR;
using OpenAiSample.WebApi.Models.OpenAi;
using OpenAiSample.WebApi.Options;
using OpenAiSample.WebApi.Services;
using OpenAiSample.WebApi.Services.Api;
using OpenAiSample.WebApi.Services.Identity;
using OpenAiSample.WebApi.Services.JsonToExcel;
using OpenApiSample.Data;
using OpenApiSample.Data.Entities;
using OpenApiSample.Data.Repositories;
using RestEase;
using System.Text;

namespace OpenAiSample.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRouting(options => options.LowercaseUrls = true); // So that the URLs are lowercase

            // Load OpenAiConfig from appsettings.json
            var openAiConfig = builder.Configuration.GetSection("OpenAI").Get<OpenAiConfig>();

            builder.Services.AddHttpClient<IOpenAiService, OpenAiService>();

            builder.Services.AddSingleton<IOpenAiApi>(provider =>
            {
                // Create an HttpClient and configure the default Authorization header
                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri(Constants.OpenAiConstants.OpenAiSourceApi)
                };
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openAiConfig.Key);

                // Create the RestEase client using the configured HttpClient
                var client = new RestClient(httpClient)
                {
                    JsonSerializerSettings = new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    }
                }.For<IOpenAiApi>();

                return client;
            });

            builder.Services.Configure<AuthSettingsOptions>(
                builder.Configuration.GetSection(AuthSettingsOptions.AuthSettings)
            );

            builder.Services.AddScoped<AppDbContextInitializer>();

            // Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme()
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description =
                            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                    }
                );

                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    }
                );
            });

            // MediatR and related behaviors
            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<Program>();
                config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            });

            // Entity Framework and Identity
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services
                  .AddIdentity<User, Role>(options =>
                  {
                      options.Password.RequiredLength = 8;
                      options.Password.RequireDigit = true;
                      options.Password.RequireLowercase = true;
                      options.Password.RequireUppercase = true;
                      options.Password.RequireNonAlphanumeric = true;
                  })
                  .AddEntityFrameworkStores<AppDbContext>()
                  .AddDefaultTokenProviders();

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["AuthSettings:Audience"],
                        ValidIssuer = builder.Configuration["AuthSettings:Issuer"],
                        RequireExpirationTime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["AuthSettings:Key"]!)
                        ),
                        ValidateIssuerSigningKey = true
                    };
                });

            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IIdentityService, IdentityService>();

            // Repository
            builder.Services.AddScoped<IProjectsRepository, ProjectsRepository>();
            builder.Services.AddScoped<IIdeaRepository, IdeaRepository>();
            builder.Services.AddScoped<IMemberRepository, MemberRepository>();
            builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
            builder.Services.AddScoped<IUserProjectRepository, UserProjectRepository>();
            builder.Services.AddScoped<IPdwRepository, PdwRepository>();
            builder.Services.AddScoped<IExcelService, ExcelService>();
            builder.Services.AddScoped<IJsonService, JsonService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    }
                );
            });

            // Controllers
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });

            var app = builder.Build();

            using var scope = app.Services.CreateScope();

            var initializer =
                scope.ServiceProvider.GetRequiredService<AppDbContextInitializer>();

            await initializer.InitializeAsync();
            await initializer.SeedAsync();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseRouting();  // Ensure routing is enabled
            app.UseCors("AllowAll");
            app.UseAuthentication();  // Authentication middleware
            app.UseAuthorization();  // Authorization middleware

            app.MapControllers();

            app.Run();
        }
    }
}
