using Common.Contexts;
using Common.Contexts.Models;
using Common.Repositories;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Services;
using PaymentGateway.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<TSFContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("Con")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMvc();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IRepository<BookingTransactionTableModel>, Repository<BookingTransactionTableModel>>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod();
    });
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
