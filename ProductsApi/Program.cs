
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProductsApi.Configrations;
using ProductsApi.Models;
using ProductsApi.Repositories;
using ProductsApi.UnitOFWork;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace ProductsApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<EpicContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DB")));
            builder.Services.AddScoped<GenericRepo<Product>>();
            builder.Services.AddScoped<UnitOfWorks>();
            builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("JWTConfig"));

            builder.Services.AddIdentity<AppUser, IdentityRole<int>>(o =>
            {
                o.User.AllowedUserNameCharacters = null;
                o.User.RequireUniqueEmail = true;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredUniqueChars = 0;
                o.Password.RequiredLength = 5;
                o.Password.RequireUppercase = false;
                o.SignIn.RequireConfirmedEmail = false;
            }).AddEntityFrameworkStores<EpicContext>();


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwt =>
            {
                var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = false,
                };
            });
            string txt = "";
            builder.Services.AddCors(o =>
            {
                o.AddPolicy(txt, builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors(txt);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
