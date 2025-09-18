using Core.Contract;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Infrastructure.SymptomChecker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Infrastructure.Containerpipline
{
    public static class RegisterDI
    {
        public static IServiceCollection AddDIPipline(this IServiceCollection Services, IConfiguration _IConfig)
        {
            string connectionString = "HealthcareContext";
            Services.AddDbContext<HealthcareDb>(dataCtxt =>
            {

                dataCtxt.UseSqlServer(_IConfig.GetConnectionString(connectionString), sqlOption =>
                {
                    sqlOption.MigrationsAssembly(typeof(HealthcareDb).Assembly.FullName);
                    sqlOption.EnableRetryOnFailure();
                });


            }, ServiceLifetime.Scoped);


            Services.AddSingleton<IAIClient, GroqClient>();
            Services.AddScoped(typeof(IGenericService<>), typeof(Repository<>));
            Services.AddScoped<SymptomCheckerService>();
            Services.AddScoped<MedicalRecordService>();
            Services.AddScoped<IPatientService,PatientService>();
            //Encryption Service 
            Services.AddScoped<IEncryptionService, AesEncryptionService>();
            Services.AddAuthorization();



            //Authentication - JWT(Single config)
            var jwtKey = _IConfig["Jwt:Key"] ?? "replace-this-secret-with-secure-key";
            var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
            Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
                };
            });

            return Services;
        }
    }
}
