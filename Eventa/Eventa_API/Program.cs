using Eventa_BusinessObject;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MongoDB.Driver;
using Eventa_Repositories.Interfaces;
using Eventa_Repositories.Implements;
using Eventa_Services.Interfaces;
using Eventa_Services.Implements;
using MongoDB.Driver.Core.Configuration;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Eventa_DAOs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("*")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Aventa", Version = "v1" });

    // Cấu hình Bearer token 
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Vui lòng nhập 'Bearer' [space] và token vào đây.",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"

    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});// Load JWT settings

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
// Add services to the container.


// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience
    };
});

// Load Firebase settings from configuration
var firebaseSettings = builder.Configuration.GetSection("Firebase").Get<FirebaseSetting>();

if (firebaseSettings == null)
{
    throw new Exception("Firebase settings not found in configuration.");
}

builder.Configuration.GetSection("Firebase").Get<FirebaseSetting>();

// Combine the base path with the relative path
var credentialPath = Path.Combine(builder.Environment.ContentRootPath, firebaseSettings.CredentialPath);

// Initialize FirebaseApp and register it as a singleton service
var firebaseApp = FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(credentialPath)
});

builder.Services.AddSingleton(firebaseApp);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đăng ký MongoDB
var connectionString = builder.Configuration.GetConnectionString("MongoDbConnection");
var databaseName = builder.Configuration["MongoDb:DatabaseName"];

// Tạo MongoClient và MongoDatabase
var mongoClient = new MongoClient(connectionString);
var mongoDatabase = mongoClient.GetDatabase(databaseName);

builder.Services.AddSingleton<IMongoClient>(mongoClient);
builder.Services.AddSingleton(mongoDatabase);

// Đăng ký EventaDBContext với IMongoDatabase
builder.Services.AddSingleton<EventaDBContext>();
builder.Services.AddMemoryCache();


// Register DAOs
builder.Services.AddSingleton<AccountDAO>();


// Register repositories
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IFirebaseService, FirebaseService>();
builder.Services.AddScoped<IVerificationTokenService, VerificationTokenService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
