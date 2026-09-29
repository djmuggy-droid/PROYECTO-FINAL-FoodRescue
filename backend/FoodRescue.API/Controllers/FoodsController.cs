using FoodRescue.API.Data;
using FoodRescue.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodRescue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodsController : ControllerBase
{
    private readonly FoodRescueDbContext _context;

    public FoodsController(FoodRescueDbContext context)
    {
        _context = context;
    }

    // GET: api/Foods
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Food>>> GetFoods()
    {
        return await _context.Foods.ToListAsync();
    }

    // GET: api/Foods/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Food>> GetFood(int id)
    {
        var food = await _context.Foods.FindAsync(id);

        if (food == null)
        {
            return NotFound();
        }

        return food;
    }

    // POST: api/Foods
    [HttpPost]
    public async Task<ActionResult<Food>> CreateFood(Food food)
    {
        _context.Foods.Add(food);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetFood),
            new { id = food.Id },
            food
        );
    }

    // PUT: api/Foods/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFood(int id, Food food)
    {
        if (id != food.Id)
        {
            return BadRequest();
        }

        _context.Entry(food).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Foods/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFood(int id)
    {
        var food = await _context.Foods.FindAsync(id);

        if (food == null)
        {
            return NotFound();
        }

        _context.Foods.Remove(food);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Alimento eliminado correctamente" });
    }
}