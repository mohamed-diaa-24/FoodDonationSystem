namespace FoodDonationSystem.Core.DTOs.Charity
{
    public class CreateCharityDto : CreateCharityRequest
    {
        public string? LicensePath { get; set; }

        public string? ProofPath { get; set; }
    }

}
