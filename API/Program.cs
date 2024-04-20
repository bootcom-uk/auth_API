using API.Services;
using Communication;
using Database.Mongo;
using Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the connection string
string connectionString = builder.Configuration.GetConnectionString("AppConfig")!;

// Load configuration from Azure App Configuration
builder.Configuration.AddAzureAppConfiguration(connectionString);

builder.Services.AddControllers();

var serilogLogger = new LoggerConfiguration()
    .WriteTo.MongoDBBson(config =>
    {
        config.SetConnectionString(builder.Configuration["LOGGING_MONGO_CONNECTION_STRING"]!);
        config.SetCollectionName("Identity");
    })
    .CreateLogger();

builder.Services.AddLogging(configure => configure.AddSerilog(serilogLogger, dispose: true));

builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IEmailProviderService, EmailProviderService>();
builder.Services.AddScoped<AuthTokenService>();
builder.Services.AddScoped<RefreshTokenService>();
//builder.Services.AddScoped<IUserActionsService, UserActionsService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JWT_ISSUER"],
            ValidAudience = builder.Configuration["JWT_AUDIENCE"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT_SECRET_KEY"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {

                serilogLogger.Debug("{0}", context.Exception);

                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    if(context.Response.Headers.Count(record => record.Key == "Token-Expired") == 0)
                    {
                        context.Response.Headers.Add("Token-Expired", "true");
                    }
                }

                return Task.CompletedTask;
            }
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
