using System.Net;
using Microsoft.EntityFrameworkCore;
using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DishesDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DishesDBConnectionString")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminFromBelgium", policy =>
        policy.RequireRole("Admin").RequireClaim("country", "Belgium"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("TokenAuthNZ",
        new()
        {
            Name = "Authorization",
            Description = "Basic Authorization header using the Bearer scheme.",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            In = ParameterLocation.Header
        });
    options.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "TokenAuthNZ"
                }
            }, new List<string>()
        }
    });
});

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

app.UseAuthentication();  // Not necessary, already include
app.UseAuthorization();  // should be after  UseAuthentication

app.UseSwagger();
app.UseSwaggerUI();

// --------------------------------

app.RegisterDishesEndpoints();
app.RegisterIngredientsEndpoints();

app.Run();
