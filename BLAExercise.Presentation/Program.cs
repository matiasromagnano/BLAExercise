using BLAExercise.Application.Configuration;
using BLAExercise.Application.Interfaces;
using BLAExercise.Application.Services;
using BLAExercise.Infrastructure.Database;
using BLAExercise.Infrastructure.Interfaces;
using BLAExercise.Infrastructure.Repositories;
using BLAExercise.Presentation.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ApiExceptionFilter>();
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "SneakerCollection API", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
            });
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection(nameof(ApplicationOptions)).GetSection(nameof(ApplicationOptions.JWTSecretKey)).Value!)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

        builder.Services.Configure<ApplicationOptions>(builder.Configuration.GetSection(nameof(ApplicationOptions)));

        // We get the options to be able to inject the Repositories that need the connection string to be build.
        var appOptions = new ApplicationOptions();
        builder.Configuration.GetSection(nameof(ApplicationOptions)).Bind(appOptions);

        // Call DB creator to generate our data storage
        var dbCreator = new DatabaseCreator(appOptions?.SqlServerConnectionString!);
        dbCreator.CreateDatabaseAndTables(appOptions?.DatabaseName);

        // Register services
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        builder.Services.AddScoped<IUserRepository, UserRepository>(sp => { return new UserRepository(appOptions?.GetFullConnectionString()!); });
        builder.Services.AddScoped<ISneakerRepository, SneakerRepository>(sp => { return new SneakerRepository(appOptions?.GetFullConnectionString()!); });
        builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        builder.Services.AddScoped<ISneakerService, SneakerService>();
        builder.Services.AddScoped<IUserService, UserService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
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