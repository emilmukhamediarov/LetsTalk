using LetsTalk.Options;
using LetsTalk.Persistence;
using Microsoft.EntityFrameworkCore;
using LetsTalk.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using LetsTalk.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LetsTalk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddOptions<JwtOptions>()
                .BindConfiguration("Jwt")
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection["Key"] ?? throw new InvalidOperationException("JWT Key is missing");

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSection["Issuer"],

                        ValidateAudience = true,
                        ValidAudience = jwtSection["Audience"],

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddSingleton<ITokenService, TokenService>();
            builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddSignalR();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            //app.MapHub<ChatHub>("/ChatHub");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
