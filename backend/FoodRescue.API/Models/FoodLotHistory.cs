namespace FoodRescue.API.Models;

public class FoodLotHistory
{
    public int Id { get; set; }

    public int FoodId { get; set; }

    public string LotCode { get; set; } = string.Empty;

    public int Version { get; set; }

    public string Product { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public DateTime ExpirationDate { get; set; }

    public string FoodType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Responsible { get; set; } = string.Empty;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}