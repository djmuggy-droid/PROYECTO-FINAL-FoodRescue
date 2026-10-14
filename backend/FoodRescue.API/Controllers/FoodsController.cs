using FoodRescue.API.Data;
using FoodRescue.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodRescue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var foods = await _context.Foods
            .OrderBy(f => f.LotCode)
            .ToListAsync();

        return Ok(foods);
    }

    // GET: api/Foods/1
    [HttpGet("{id}")]
    public async Task<ActionResult<Food>> GetFood(int id)
    {
        var food = await _context.Foods.FindAsync(id);

        if (food == null)
        {
            return NotFound(new
            {
                message = "Alimento no encontrado"
            });
        }

        return Ok(food);
    }

    // POST: api/Foods
    [HttpPost]
    public async Task<ActionResult<Food>> CreateFood(Food food)
    {
        if (string.IsNullOrWhiteSpace(food.LotCode))
        {
            return BadRequest(new
            {
                message = "El código de lote es obligatorio"
            });
        }

        if (string.IsNullOrWhiteSpace(food.Product))
        {
            return BadRequest(new
            {
                message = "El producto es obligatorio"
            });
        }

        if (food.Quantity <= 0)
        {
            return BadRequest(new
            {
                message = "La cantidad debe ser mayor a 0"
            });
        }

        if (food.UnitPrice < 0)
        {
            return BadRequest(new
            {
                message = "El precio no puede ser negativo"
            });
        }

        bool loteExiste = await _context.Foods
            .AnyAsync(f => f.LotCode == food.LotCode);

        if (loteExiste)
        {
            return BadRequest(new
            {
                message = "El código de lote ya existe"
            });
        }

        food.Version = 1;
        food.Status = CalculateStatus(food.ExpirationDate);
        food.CreatedAt = DateTime.UtcNow;
        food.UpdatedAt = DateTime.UtcNow;

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
        var existingFood = await _context.Foods.FindAsync(id);

        if (existingFood == null)
        {
            return NotFound(new
            {
                message = "Alimento no encontrado"
            });
        }

        if (string.IsNullOrWhiteSpace(food.LotCode))
        {
            return BadRequest(new
            {
                message = "El código de lote es obligatorio"
            });
        }

        if (string.IsNullOrWhiteSpace(food.Product))
        {
            return BadRequest(new
            {
                message = "El producto es obligatorio"
            });
        }

        if (food.Quantity <= 0)
        {
            return BadRequest(new
            {
                message = "La cantidad debe ser mayor a 0"
            });
        }

        if (food.UnitPrice < 0)
        {
            return BadRequest(new
            {
                message = "El precio no puede ser negativo"
            });
        }

        bool loteDuplicado = await _context.Foods
            .AnyAsync(f =>
                f.LotCode == food.LotCode &&
                f.Id != id
            );

        if (loteDuplicado)
        {
            return BadRequest(new
            {
                message = "El código de lote ya está siendo utilizado"
            });
        }

        // Guardar la versión anterior
        var history = new FoodLotHistory
        {
            FoodId = existingFood.Id,
            LotCode = existingFood.LotCode,
            Version = existingFood.Version,
            Product = existingFood.Product,
            Quantity = existingFood.Quantity,
            UnitPrice = existingFood.UnitPrice,
            ExpirationDate = existingFood.ExpirationDate,
            FoodType = existingFood.FoodType,
            Status = existingFood.Status,
            Responsible = existingFood.Responsible,
            ModifiedAt = DateTime.UtcNow
        };

        _context.FoodLotHistories.Add(history);

        // Actualizar lote
        existingFood.LotCode = food.LotCode;
        existingFood.Product = food.Product;
        existingFood.Quantity = food.Quantity;
        existingFood.UnitPrice = food.UnitPrice;
        existingFood.ExpirationDate = food.ExpirationDate;
        existingFood.FoodType = food.FoodType;
        existingFood.Responsible = food.Responsible;

        existingFood.Version++;
        existingFood.Status = CalculateStatus(existingFood.ExpirationDate);
        existingFood.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(existingFood);
    }

    // GET: api/Foods/1/history
    [HttpGet("{id}/history")]
    public async Task<ActionResult<IEnumerable<FoodLotHistory>>> GetHistory(int id)
    {
        var foodExists = await _context.Foods.AnyAsync(f => f.Id == id);

        if (!foodExists)
        {
            return NotFound(new
            {
                message = "Alimento no encontrado"
            });
        }

        var history = await _context.FoodLotHistories
            .Where(h => h.FoodId == id)
            .OrderByDescending(h => h.Version)
            .ToListAsync();

        return Ok(history);
    }

    // DELETE: api/Foods/1
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> DeleteFood(int id)
    {
        var food = await _context.Foods.FindAsync(id);

        if (food == null)
        {
            return NotFound(new
            {
                message = "Alimento no encontrado"
            });
        }

        _context.Foods.Remove(food);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Alimento eliminado correctamente"
        });
    }

    private string CalculateStatus(DateTime expirationDate)
    {
        int days =
            (expirationDate.Date - DateTime.UtcNow.Date).Days;

        if (days < 0)
        {
            return "No disponible";
        }

        if (days <= 3)
        {
            return "Consumo prioritario";
        }

        return "Disponible";
    }
}