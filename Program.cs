using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// سجّل الـ Repository والخدمة في الـ DI
builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<VendorApprovalRepository>();
builder.Services.AddSingleton<HrEmployeeRepository>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<CreatuserService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "My API", 
        Version = "v1",
        Description = "Example API with Swagger in ASP.NET Core"
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty; // Swagger UI على الصفحة الرئيسية http://localhost:5000
    });
}
// إنشاء الجدول مرّة واحدة عند الإقلاع
using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<UserRepository>();
    repo.CreateTable();
       var vendorRepo = scope.ServiceProvider.GetRequiredService<VendorApprovalRepository>();
    vendorRepo.CreateTable();
    var hr = scope.ServiceProvider.GetRequiredService<HrEmployeeRepository>();
    vendorRepo.CreateTable();
}

var RootGroup = app.MapGroup("/api/v1");
RootGroup.MapGroup("/auth").MapRegisterEndpoints()
   .MapAuthEndpoints();


app.Run();

