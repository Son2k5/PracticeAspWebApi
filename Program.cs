using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MyApiProject.Data;
using MyApiProject.Repository.Interfaces;
using MyApiProject.Dtos;
using MyApiProject.Repository;
namespace MyApiProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // =============== CONFIG DATABASE ===============
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")))
            );
            // ============== REPOSITORY DI ================
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            //================= CONFIG JWT ================
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
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                    )
                };
            });

            // ========= CONFIG SWAGGER ===========
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();

            var app = builder.Build();

            // ========== CONFIG MIDDLEWARE ==============
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

            }
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();
            app.MapControllers();
            app.Run();

        }

    }

}

