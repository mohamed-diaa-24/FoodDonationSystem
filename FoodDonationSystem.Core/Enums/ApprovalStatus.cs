using System.ComponentModel;

namespace FoodDonationSystem.Core.Enums
{
    public enum ApprovalStatus
    {

        [Description("في انتظار الموافقة")]
        Pending = 1,

        [Description("موافق عليه")]
        Approved = 2,

        [Description("مرفوض")]
        Rejected = 3

    }
}
