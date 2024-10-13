using GymApplication.api.Attribute;
using GymApplication.api.Extension;
using GymApplication.api.Middleware;
using GymApplication.Repository.Entities;
using GymApplication.Repository.Extension;
using GymApplication.Repository.Repository;
using GymApplication.Repository.Repository.Abstraction;
using GymApplication.Services.Extension;
using GymApplication.Shared.Emuns;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using UUIDNext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddApiVersionForController();
builder.Services.AddTransient<IRepoBase<Subscription, Guid>, RepoBase<Subscription, Guid>>();
builder.Services.AddTransient<IRepoBase<DayGroup, Guid>, RepoBase<DayGroup, Guid>>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();
    options.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
    options.SchemaFilter<SwaggerIgnoreFilter>();
    // Define the security scheme (JWT Bearer Token)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer abc123\""
    });
});

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services
    .AddServicesLayer(builder.Configuration)
    .AddRepositoryLayer(builder.Configuration);

builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
}
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () =>
{
    await using var scope = app.Services.CreateAsyncScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var user = await userManager.FindByNameAsync("admin@gmail.com");
    if (user is not null) return;
    var newUser = new ApplicationUser()
    {
        Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
        UserName = "admin@gmail.com",
        PhoneNumber = "0123456789",
        Email = "admin@gmail.com",
        FullName = "Admin",
        CreatedAt = DateTime.UtcNow,
        DateOfBirth = DateOnly.Parse("2003-03-21")
    };
    var result = await userManager.CreateAsync(newUser, "Admin@123");
    if (result.Succeeded)
    {
        await roleManager.CreateAsync(new ApplicationRole()
        {
            Name = Role.Admin.ToString()
        });
        await userManager.AddToRoleAsync(newUser, Role.Admin.ToString());
    }
});

app.Run();