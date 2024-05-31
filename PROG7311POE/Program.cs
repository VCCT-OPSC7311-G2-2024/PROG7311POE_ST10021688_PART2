using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PROG7311POE.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PROG7311POE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure website
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            // Configure JWT authentication & Auhorisation
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            // __ Configure Entity Framework and SQLite ____________________________________________________________________
            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("MyDefaultConnection")));

            // __ Configure JWT authentication with multiple audiences(role-based access) ____________________________________________________________________
            /* builder.Services.AddAuthentication(options =>
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
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudiences = jwtSettings.GetSection("Audiences").Get<List<string>>(),
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    RoleClaimType = ClaimTypes.Role // Ensure roles are extracted from the token
                };
            });*/

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            RoleClaimType = ClaimTypes.Role // Ensure roles are extracted from the token Becuasew it's not obvious neough to be built into an authroiing package \:
        };
    });

            // Role-based access ____________________________________________________________________
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("EmployeePolicy", policy =>
                    policy.RequireRole("Employee"));
                options.AddPolicy("FarmerPolicy", policy =>
                    policy.RequireRole("Farmer"));
            });

            // Configure the HTTP request pipeline ____________________________________________________________________
            var myWebApp = builder.Build();
            if (!myWebApp.Environment.IsDevelopment())
            {
                myWebApp.UseExceptionHandler("/Home/Error");
                myWebApp.UseHsts();
            }
            myWebApp.UseHttpsRedirection();
            myWebApp.UseStaticFiles();
            myWebApp.UseRouting();
            myWebApp.UseAuthentication(); // Enable JWT authentication 
            myWebApp.UseAuthorization();

            myWebApp.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            myWebApp.Run();
        }
    }
}
// ??????? --<< End of File >>-- ???????