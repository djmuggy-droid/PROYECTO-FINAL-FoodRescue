namespace FoodRescue.API.Models;

public class Food
{
    public int Id { get; set; }

    // Código único del lote
    public string LotCode { get; set; } = string.Empty;

    // Versión actual del lote
    public int Version { get; set; } = 1;

    public string Product { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalValue => Quantity * UnitPrice;

    public DateTime ExpirationDate { get; set; }

    public string FoodType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Responsible { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}