using System.Net;
using Microsoft.EntityFrameworkCore;
using DishesAPI.DbContexts;
using DishesAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DishesDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DishesDBConnectionString")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddProblemDetails();

// --------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    // app.UseExceptionHandler(configureApplicationBuilder =>
    // {
    //     configureApplicationBuilder.Run(async context =>
    //     {
    //         context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //         context.Response.ContentType = "text/html";
    //         await context.Response.WriteAsync("An unexpected problem happened.");
    //     });
    // });    
}

app.UseHttpsRedirection();

// --------------------------------

app.RegisterDishesEndpoints();
app.RegisterIngredientsEndpoints();

app.Run();
