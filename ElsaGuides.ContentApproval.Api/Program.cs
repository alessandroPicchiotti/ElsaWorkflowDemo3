using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Identity;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

// Configurazione CORS
var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "https://localhost:7002" };

services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

Elsa.EndpointSecurityOptions.DisableSecurity();

// Configurazione Elsa 3.x
services.AddElsa(elsa =>
{
    // Workflow Management (Designer, Definitions)
    elsa.UseWorkflowManagement(management =>
    {
        management.UseEntityFrameworkCore(ef =>
        {
            ef.UseSqlServer(configuration.GetConnectionString("Elsa"));
        });
    });

    // Workflow Runtime (Execution)
    elsa.UseWorkflowRuntime(runtime =>
    {
        runtime.UseEntityFrameworkCore(ef =>
        {
            ef.UseSqlServer(configuration.GetConnectionString("Elsa"));
        });
    });

    //// Identity (Users, Roles) - opzionale per questa demo
    //elsa.UseIdentity(identity =>
    //{
    //    identity.UseEntityFrameworkCore(ef =>
    //    {
    //        ef.UseSqlServer(configuration.GetConnectionString("Elsa"));
    //    });
    //});

    //// Activities
    elsa.UseHttp(http =>
    {
        http.ConfigureHttpOptions = options =>
        {
            options.BaseUrl = new Uri(configuration["Elsa:Server:BaseUrl"]!);
        };
    });
    //elsa.UseHttp(http => http.ConfigureHttpOptions = options =>
    //{
    //    options.BaseUrl = new Uri("https://localhost:7001");
    //    options.BasePath = "/workflows";
    //});

    // Default Identity features for authentication/authorization.
    elsa.UseIdentity(identity =>
    {
        identity.TokenOptions = options => options.SigningKey = "sufficiently-large-secret-signing-key"; // This key needs to be at least 256 bits long.
        identity.UseAdminUserProvider();
    });

    elsa.UseEmail(email =>
    {
        email.ConfigureOptions = options =>
        {
            options.Host = configuration["Elsa:Smtp:Host"]!;
            options.Port = int.Parse(configuration["Elsa:Smtp:Port"]!);
            options.DefaultSender = configuration["Elsa:Smtp:DefaultSender"]!;
        };
    });

    elsa.UseScheduling();

    // Registra workflows
    elsa.AddWorkflowsFrom<Program>();

    // Add Elsa API endpoints for Designer
    elsa.UseWorkflowsApi();
});

// Swagger
services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Elsa Workflow Demo API",
        Version = "v1"
    });
    options.CustomSchemaIds(type => type.ToString()); // <-- aggiungi questa riga!
});

services.AddControllers();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Elsa Content Approval API v1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseRouting();
app.UseAuthorization();

// Elsa HTTP activities middleware
app.UseWorkflowsApi();
app.UseWorkflows();

app.MapControllers();

//// Run migrations automaticamente
//using (var scope = app.Services.CreateScope())
//{
//    var runMigrations = async (DbContext dbContext) =>
//    {
//            await dbContext.Database.MigrateAsync();
//    };

//    await runMigrations(scope.ServiceProvider.GetRequiredService<ManagementElsaDbContext>());
//    await runMigrations(scope.ServiceProvider.GetRequiredService<RuntimeElsaDbContext>());
//    await runMigrations(scope.ServiceProvider.GetRequiredService<IdentityElsaDbContext>());
//}
//await app.RunElsaMigrationsAsync();
app.Run();