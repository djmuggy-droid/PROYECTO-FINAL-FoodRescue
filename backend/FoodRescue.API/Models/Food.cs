namespace FoodRescue.API.Models;

public class Food
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime ExpirationDate { get; set; }
}