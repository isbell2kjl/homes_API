using System.Text;
using blog_API.Migrations;
using blog_API.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddDbContext<PostDbContext>();
 
// builder.Services.AddMySqlDataSource(builder.Configuration.GetConnectionString("Default")!);

// builder.Services.AddSqlite<PostDbContext>("Data Source=blog_API.db");
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var secretKey = builder.Configuration["TokenSecret"];
//convert string to byte
byte[] bArray = Encoding.ASCII.GetBytes(secretKey);

// JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
.AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = true;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        IssuerSigningKey = new SymmetricSecurityKey(bArray),
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = false,
        RequireExpirationTime = false,
        ClockSkew = TimeSpan.Zero,
        ValidateIssuerSigningKey = true
    };
}
);
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "blog_API_tokens", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        In = ParameterLocation.Header, 
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey 
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    { 
        new OpenApiSecurityScheme 
        { 
        Reference = new OpenApiReference 
        { 
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer" 
        } 
        },
        new string[] { } 
        } 
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
    .WithOrigins("http://localhost:8100","http://localhost:4200", "http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod());
    
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
