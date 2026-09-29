namespace FoodRescue.API.Models;

public class Food
{
    public int Id { get; set; }

    public string Product { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public DateTime ExpirationDate { get; set; }

    public string FoodType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Responsible { get; set; } = string.Empty;
}