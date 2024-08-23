
using Booking.Services;
using Booking.Services.Interfaces;
using Common.Contexts;
using Common.Contexts.Models;
using Common.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<TSFContext>(options => options.UseMySQL(builder.Configuration.GetConnectionString("Con")));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMvc();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IRepository<FlightTableModel>, Repository<FlightTableModel>>();
builder.Services.AddScoped<IRepository<BookingTableModel>, Repository<BookingTableModel>>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
        policy.WithOrigins("https://localhost:7026").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
