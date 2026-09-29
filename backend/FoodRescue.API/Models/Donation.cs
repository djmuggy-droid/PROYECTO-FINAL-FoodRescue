namespace FoodRescue.API.Models;

public class Donation
{
    public int Id { get; set; }
    public string DonorName { get; set; } = string.Empty;
    public string FoodName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime DonationDate { get; set; }
    public string Status { get; set; } = string.Empty;
}