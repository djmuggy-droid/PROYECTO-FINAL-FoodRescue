using FoodRescue.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodRescue.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly FoodRescueDbContext _context;

    public DashboardController(FoodRescueDbContext context)
    {
        _context = context;
    }

    // GET: api/Dashboard
    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var today = DateTime.UtcNow.Date;

        var foods = await _context.Foods
            .OrderBy(f => f.LotCode)
            .ToListAsync();

        var totalAlimentos = foods.Count;

        var totalInventario = foods.Sum(f => f.Quantity);

        var valorInventario = foods.Sum(
            f => f.Quantity * f.UnitPrice
        );

        var disponibles = foods.Count(f =>
            (f.ExpirationDate.Date - today).Days > 3
        );

        var prioritarios = foods.Count(f =>
        {
            int dias = (f.ExpirationDate.Date - today).Days;
            return dias >= 0 && dias <= 3;
        });

        var vencidos = foods.Count(f =>
            f.ExpirationDate.Date < today
        );

        var alertas = foods
            .Select(f => new
            {
                f.Id,
                f.LotCode,
                f.Product,
                f.Quantity,
                f.ExpirationDate,

                DaysRemaining =
                    (f.ExpirationDate.Date - today).Days
            })
            .Where(f =>
                f.DaysRemaining >= 0 &&
                f.DaysRemaining <= 3
            )
            .OrderBy(f => f.DaysRemaining)
            .ToList();

        return Ok(new
        {
            totalAlimentos,
            totalInventario,
            valorInventario,
            disponibles,
            prioritarios,
            vencidos,
            alertas
        });
    }
}