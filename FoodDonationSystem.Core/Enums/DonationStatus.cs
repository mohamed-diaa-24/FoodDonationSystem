using System.ComponentModel;

namespace FoodDonationSystem.Core.Enums
{
    public enum DonationStatus
    {
        [Description("متاح")]
        Available = 1,

        [Description("محجوز")]
        Reserved = 2,

        [Description("مكتمل")]
        Completed = 3,

        [Description("منتهي الصلاحية")]
        Expired = 4,

        [Description("ملغي")]
        Cancelled = 5
    }
}
