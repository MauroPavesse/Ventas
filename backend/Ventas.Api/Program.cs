using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Ventas.Api.Middlewares;
using Ventas.Application;
using Ventas.Application.Behaviors;
using Ventas.Application.Entities.Categories;
using Ventas.Application.Entities.Configurations;
using Ventas.Application.Entities.Customers;
using Ventas.Application.Entities.DailyBoxes;
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
using Ventas.Infrastructure.Data;
using Ventas.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Open", builder =>
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod());
});

var config = TypeAdapterConfig.GlobalSettings;
config.Scan(typeof(ApplicationAssemblyMarker).Assembly);

builder.Services.AddSingleton(config);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al aplicar las migraciones.");
    }
}

app.UseCors("Open");

app.Run();

