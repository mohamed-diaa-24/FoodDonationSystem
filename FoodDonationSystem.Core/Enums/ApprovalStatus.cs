using System.ComponentModel;

namespace FoodDonationSystem.Core.Enums
{
    public enum ApprovalStatus
    {

        [Description("Pending")]
        Pending = 1,

        [Description("Approved")]
        Approved = 2,

        [Description("Rejected")]
        Rejected = 3

    }
}
