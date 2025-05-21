using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using AutoMapper;
using DishesAPI.DbContexts;
using DishesAPI.Models;
using DishesAPI.Entities;


namespace DishesAPI.EndpointHandlers;

public static class DishesHandlers
{
    public static async Task<Ok<IEnumerable<DishDto>>> GetDishesAsync(
        DishesDbContext dishesDbContext,
        IMapper mapper,
        ILogger<DishDto> logger,
        string? name)
    {
        logger.LogInformation("Getting dishes...");
        
        return TypedResults.Ok(mapper.Map<IEnumerable<DishDto>>(await dishesDbContext.Dishes
            .Where(d => name == null || d.Name.Contains(name))
            .ToListAsync()));
    }

    public static async Task<Results<NotFound, Ok<DishDto>>> GetDishByIdAsync(DishesDbContext dishesDbContext,
        IMapper mapper,
        Guid dishId)
    {
        var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        if (dishEntity == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(mapper.Map<DishDto>(dishEntity));
    }

    public static async Task<Results<NotFound, Ok<DishDto>>> GetDishByNameAsync(DishesDbContext dishesDbContext,
        IMapper mapper,
        string name)
    {
        var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Name == name);
        if (dishEntity == null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(mapper.Map<DishDto>(dishEntity));
    }

    public static async Task<CreatedAtRoute<DishDto>> CreateDishAsync(DishesDbContext dishesDbContext,
        IMapper mapper,
        DishForCreationDto dto)
    {
        var dishEntity = mapper.Map<Dish>(dto);
        dishesDbContext.Add(dishEntity);
        await dishesDbContext.SaveChangesAsync();

        var dishToReturn = mapper.Map<DishDto>(dishEntity);
        
        return TypedResults.CreatedAtRoute(dishToReturn, "GetDish", new { dishId = dishToReturn.Id });
    }

    public static async Task<Results<NotFound, NoContent>> UpdateDishAsync(DishesDbContext dishesDbContext,
        IMapper mapper,
        Guid dishId,
        DishForUpdateDto dto)
    {
        var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        if (dishEntity == null)
            return TypedResults.NotFound();

        mapper.Map(dto, dishEntity);
        await dishesDbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    public static async Task<Results<NotFound, NoContent>> DeleteDishAsync(DishesDbContext dishesDbContext,
        Guid dishId)
    {
        var dishEntity = await dishesDbContext.Dishes.FirstOrDefaultAsync(d => d.Id == dishId);
        if (dishEntity == null)
            return TypedResults.NotFound();
        
        dishesDbContext.Dishes.Remove(dishEntity);
        await dishesDbContext.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}