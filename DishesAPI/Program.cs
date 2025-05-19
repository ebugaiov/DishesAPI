using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DishesAPI.DbContexts;
using DishesAPI.Models;
using DishesAPI.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DishesDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DishesDBConnectionString")));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// --------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// --------------------------------

app.MapGet("/dishes", async Task<Ok<IEnumerable<DishDto>>>
    (DishesDbContext db, IMapper mapper, [FromQuery] string? name) =>
{
    return TypedResults.Ok(mapper.Map<IEnumerable<DishDto>>(await db.Dishes
        .Where(d => name == null || d.Name.Contains(name))
        .ToListAsync()));
});

app.MapGet("/dishes/{dishId:guid}", async Task<Results<NotFound, Ok<DishDto>>>
    (DishesDbContext db, IMapper mapper, Guid dishId) =>
{
    var dishEntity = await db.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
    if (dishEntity == null)
        return TypedResults.NotFound();
    return TypedResults.Ok(mapper.Map<DishDto>(dishEntity));
});

app.MapGet("/dishes/{dishName}", async Task<Ok<DishDto>>
    (DishesDbContext db, IMapper mapper, string dishName) =>
{
    return TypedResults.Ok(mapper.Map<DishDto>(await db.Dishes.FirstOrDefaultAsync(d => d.Name == dishName)));
});

app.MapGet("/dishes/{dishId}/ingredients", async Task<Results<NotFound, Ok<IEnumerable<IngredientDto>>>>
    (DishesDbContext db, IMapper mapper, Guid dishId) =>
{
    var dishEntity = await db.Dishes
        .FirstOrDefaultAsync(d => d.Id == dishId);

    if (dishEntity == null)
        return TypedResults.NotFound();

    return TypedResults.Ok(mapper.Map<IEnumerable<IngredientDto>>((await db.Dishes
        .Include(d => d.Ingredients)
        .FirstOrDefaultAsync(d => d.Id == dishId))?.Ingredients));
});

app.MapPost("/dishes", async (DishesDbContext db, IMapper mapper, DishForCreationDto dish) =>
{
    var dishEntity = mapper.Map<Dish>(dish);
    db.Add(dishEntity);
    await db.SaveChangesAsync();
    
    var dishToReturn = mapper.Map<DishDto>(dishEntity);
    return TypedResults.Created(dishToReturn);
});

// recreate & migrate database on each run
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
    context.Database.EnsureDeleted();
    context.Database.Migrate();
}

app.Run();
