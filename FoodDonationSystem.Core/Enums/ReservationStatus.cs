using System.ComponentModel;

namespace FoodDonationSystem.Core.Enums
{
    public enum ReservationStatus
    {
        [Description("معلق")]
        Pending = 1,
        [Description("مؤكد")]
        Confirmed = 2,
        [Description("مكتمل")]
        Completed = 3,
        [Description("ملغي")]
        Cancelled = 4,
    }
}
