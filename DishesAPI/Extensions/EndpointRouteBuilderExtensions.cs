using DishesAPI.EndpointHandlers;
using DishesAPI.EndpointFilters;

namespace DishesAPI.Extensions;

public static class EndpointRouteBuilderExtensions
{
    public static void RegisterDishesEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var dishesEndpoints = endpointRouteBuilder.MapGroup("/dishes");
        var dishWithGuidIdEndpoints = dishesEndpoints.MapGroup("/{dishId:guid}");
        var dishWithGuidIdEndpointsAndLockFilters = 
            endpointRouteBuilder.MapGroup("/dishes/{dishId:guid}")
                .AddEndpointFilter(new DishIsLockedFilter(
                    new("fd630a57-2352-4731-b25c-db9cc7601b16")))
                .AddEndpointFilter(new DishIsLockedFilter(
                    new("eacc5169-b2a7-41ad-92c3-dbb1a5e7af06")));    

        dishesEndpoints.MapGet("", DishesHandlers.GetDishesAsync);
        dishesEndpoints.MapGet("/{dishName}", DishesHandlers.GetDishByNameAsync);
        dishWithGuidIdEndpoints.MapGet("", DishesHandlers.GetDishByIdAsync).WithName("GetDish");
        dishesEndpoints.MapPost("", DishesHandlers.CreateDishAsync);
        dishWithGuidIdEndpoints.MapPost("", DishesHandlers.UpdateDishAsync);
    }

    public static void RegisterIngredientsEndpoints(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var ingredientsEndpoints = endpointRouteBuilder.MapGroup("/dishes/{dishId:guid}/ingredients");
        
        ingredientsEndpoints.MapGet("", IngredientsHandlers.GetIngredientsAsync);
        
        // For experiments
        ingredientsEndpoints.MapPost("", () =>
        {
            throw new NotImplementedException();
        });
    }
}