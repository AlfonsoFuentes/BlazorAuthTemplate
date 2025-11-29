using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Server.DataContext;
using Server.Domain.Identities;
using Server.Interfaces.EndPoints;
using Server.Repositories;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;

namespace Server.Services
{
    public static class RegisterServices
    {
        public static WebApplicationBuilder AddServerServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("Default"))
            );

            // For Identity  
            builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 7;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Adding Authentication  
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer  
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]!))
                };
            });
            builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorWasm",
                    builder => builder
                    .WithOrigins("http://localhost:5022", "https://localhost:7232")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            }

                );
            builder.Services.AddEndPoints();
            builder.Services.AddLazyCache();
            builder.Services.AddTransient<ITokenService, TokenService>();
            builder.Services.AddSingleton<ICache, Cache>();
          
            return builder;
        }
        public static IServiceCollection AddEndPoints(this IServiceCollection service)
        {
            service.AddEndPoints(Assembly.GetExecutingAssembly());
            return service;
        }
        static IServiceCollection AddEndPoints(this IServiceCollection service, Assembly assembly)
        {
            ServiceDescriptor[] serviceDescriptors = assembly
                .DefinedTypes
                .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                type.IsAssignableTo(typeof(IEndPoint)))
                .Select(type => ServiceDescriptor.Transient(typeof(IEndPoint), type)).ToArray();
            var descr = serviceDescriptors.ToList();
            service.TryAddEnumerable(serviceDescriptors);
            return service;
        }

    }
}
