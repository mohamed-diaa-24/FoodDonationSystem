namespace FoodDonationSystem.Core.DTOs.Restaurant
{
    public class CreateRestaurantDto : CreateRestaurantRequest
    {
        public string? LicensePath { get; set; } = "";
        public string? RegisterPath { get; set; } = "";

    }
}
