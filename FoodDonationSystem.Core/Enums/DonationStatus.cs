using System.ComponentModel;

namespace FoodDonationSystem.Core.Enums
{
    public enum DonationStatus
    {
        [Description("متاح")]
        Available = 1,

        [Description("محجوز")]
        Reserved = 2,

        [Description("قيد التوصيل")]
        InProgress = 3,

        [Description("مكتمل")]
        Completed = 4,

        [Description("منتهي الصلاحية")]
        Expired = 5,

        [Description("ملغي")]
        Cancelled = 6
    }
}
