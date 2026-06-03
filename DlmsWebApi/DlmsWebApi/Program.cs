using DlmsWebApi.Business.AuthorBusiness;
using DlmsWebApi.Business.BookBusiness;
using DlmsWebApi.Repository.AuthorRepository;
using DlmsWebApi.Repository.BookRepository;
using DlmsWebApi.Repository.Data;
//using LibrarySystem.Business.CategoryBusiness;
//using LibrarySystem.Business.PublicationBusiness;
//using LibrarySystem.Repository.CategoryRepository;
//using LibrarySystem.Repository.PublicationRepository;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


// 1. Add CORS services to the container
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:8000") // Or Use AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
 options.UseSqlite(connectionString));

builder.Services.AddScoped<IAuthorBusiness, AuthorBusiness>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookBusiness, BookBusiness>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
//builder.Services.AddScoped<ICategoryBusiness, CategoryBusiness>();
//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
//builder.Services.AddScoped<IPublicationBusiness, PublicationBusiness>();
//builder.Services.AddScoped<IPublicationRepository, PublicationRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowFrontend");


app.MapControllers();

app.Run();
