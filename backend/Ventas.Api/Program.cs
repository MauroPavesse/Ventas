using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Ventas.Api.Middlewares;
using Ventas.Application;
using Ventas.Application.Behaviors;
using Ventas.Application.Entities.AfipTokens;
using Ventas.Application.Entities.Categories;
using Ventas.Application.Entities.Configurations;
using Ventas.Application.Entities.Customers;
using Ventas.Application.Entities.DailyBoxes;
using Ventas.Application.Entities.Externas.Afip;
using Ventas.Application.Entities.Externas.Encryption;
using Ventas.Application.Entities.Externas.FileStorage;
using Ventas.Application.Entities.Externas.Jwt;
using Ventas.Application.Entities.Externas.Licences;
using Ventas.Application.Entities.Externas.Prints.BudgetDocument;
using Ventas.Application.Entities.Externas.Prints.DailyBoxDocument;
using Ventas.Application.Entities.Externas.Prints.TicketDocument;
using Ventas.Application.Entities.PaymentMethods;
using Ventas.Application.Entities.PointOfSales;
using Ventas.Application.Entities.PointOfSaleVoucherTypes;
using Ventas.Application.Entities.Products;
using Ventas.Application.Entities.Roles;
using Ventas.Application.Entities.TaxConditions;
using Ventas.Application.Entities.TaxRates;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.Users;
using Ventas.Application.Entities.VoucherDetails;
using Ventas.Application.Entities.VoucherPayments;
using Ventas.Application.Entities.Vouchers;
using Ventas.Application.Entities.VoucherTypes;
using Ventas.Domain.Others;
using Ventas.Infrastructure.Data;
using Ventas.Infrastructure.Persistence.Repositories;
using Ventas.Infrastructure.Persistence.Services;
using Ventas.Infrastructure.Persistence.Services.Afip;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IDailyBoxRepository, DailyBoxRepository>();
builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
builder.Services.AddScoped<IPointOfSaleRepository, PointOfSaleRepository>();
builder.Services.AddScoped<IPointOfSaleVoucherTypeRepository, PointOfSaleVoucherTypeRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<ITaxConditionRepository, TaxConditionRepository>();
builder.Services.AddScoped<ITaxRateRepository, TaxRateRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IVoucherDetailRepository, VoucherDetailRepository>();
builder.Services.AddScoped<IVoucherPaymentRepository, VoucherPaymentRepository>();
builder.Services.AddScoped<IVoucherTypeRepository, VoucherTypeRepository>();
builder.Services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ITicketDocumentService, PrintTicketService>();
builder.Services.AddScoped<IBudgetDocumentService, PrintBudgetService>();
builder.Services.AddScoped<IDailyBoxDocumentService, PrintDailyBoxService>();
builder.Services.AddScoped<IAfipTokenRepository, AfipTokenRepository>();
builder.Services.AddScoped<IAfipAuthService, AfipAuthService>();
builder.Services.AddScoped<IAfipService, AfipService>();
builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Open", policy =>
        policy.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod());

    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("https://*.reservacanchita.online")
              .SetIsOriginAllowedToAllowWildcardSubdomains()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registrar la memoria caché
builder.Services.AddMemoryCache();

// Registrar el HttpClient apuntando a tu IP del Nginx de Licencias
builder.Services.AddHttpClient<ILicenseService, LicenseService>(client =>
{
    client.BaseAddress = new Uri("http://72.60.60.66:81/"); // La IP y puerto del sistema de Licencias
});

var config = TypeAdapterConfig.GlobalSettings;
config.Scan(typeof(ApplicationAssemblyMarker).Assembly);

builder.Services.AddSingleton(config);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ventas API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("Open");

app.UseAuthorization();

app.UseMiddleware<TenantMiddleware>();       // Primero se identifica al cliente (Subdominio)
app.UseMiddleware<LicenseCheckMiddleware>(); // Segundo se verifica si pagó la licencia

app.MapControllers();
app.Run();

