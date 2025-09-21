using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<VendorApprovalRepository>();
builder.Services.AddSingleton<HrEmployeeRepository>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<FetureHR>();
builder.Services.AddSingleton<FetureVendor>();
builder.Services.AddSingleton<CreatuserService>();
builder.Services.AddSingleton<VendorRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "My API", 
        Version = "v1",
        Description = "Example API with Swagger in ASP.NET Core"
    });
});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; 
    });
}

using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<UserRepository>();
    repo.CreateTable();
       var vendorRepo = scope.ServiceProvider.GetRequiredService<VendorApprovalRepository>();
    vendorRepo.CreateTable();
    var hr = scope.ServiceProvider.GetRequiredService<HrEmployeeRepository>();
    hr.CreateTable();
    var Vendors = scope.ServiceProvider.GetRequiredService<VendorRepository>();
    Vendors.CreateTable();
}

var RootGroup = app.MapGroup("/api/v1");

RootGroup.MapGroup("/auth").MapRegisterEndpoints()
                        .MapAuthEndpoints();

RootGroup.MapGroup("/vendor").MapVendorEndpoints();
RootGroup.MapGroup("/hr").MapHrEndpoints();
//RootGroup.MapGroup("/users").MapUserEndpoints();



app.Run();

