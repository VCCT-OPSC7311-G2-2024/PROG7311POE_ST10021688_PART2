using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // __ Configure Entity Framework and SQLite ____________________________________________________________________
            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("MyDefaultConnection")));

            // __ Configure JWT authentication with multiple audiences(role-based access) ____________________________________________________________________
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var jwtSettings = builder.Configuration.GetSection("Jwt");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudiences = jwtSettings.GetSection("Audiences").Get<List<string>>(),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // Enable JWT authentication 
            app.UseAuthorization();

            // Redirect users to login page is un-authorized  ___________________________________________________________________
            /*app.Use(async (context, next) =>
            {
                if (!context.User.Identity.IsAuthenticated && !context.Request.Path.StartsWithSegments("/Auth"))
                {
                    context.Response.Redirect("/Auth/Login");
                    return;
                }
                await next();
            });*/
            // Middleware to handle JWT token from cookies
            // Middleware to handle JWT token from cookies
            app.Use(async (context, next) =>
            {
                var token = context.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtSettings = builder.Configuration.GetSection("Jwt");
                    var validations = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidateAudience = true,
                        ValidAudiences = jwtSettings.GetSection("Audiences").Get<List<string>>(),
                        ValidateLifetime = true
                    };

                    try
                    {
                        var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                        context.User = new ClaimsPrincipal(claims.Identity);
                    }
                    catch (SecurityTokenException)
                    {
                        // Token validation failed
                    }
                }

                await next();
            });


            // Configure the default route to point to the HomeController's Index action
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
