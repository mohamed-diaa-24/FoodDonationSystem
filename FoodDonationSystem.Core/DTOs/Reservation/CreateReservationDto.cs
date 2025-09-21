namespace FoodDonationSystem.Core.DTOs.Reservation
{
	public class CreateReservationDto
	{
		public int DonationId { get; set; }
		public string? Notes { get; set; }
		public DateTime? PickupTime { get; set; }
		public string? PickupPersonName { get; set; }
		public string? PickupPersonPhone { get; set; }
	}
}
