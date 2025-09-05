using System.ComponentModel;

namespace FoodDonationSystem.Core.Enums
{
    public enum CharityType
    {
        [Description("دار أيتام")]
        Orphanage = 1,

        [Description("دار مسنين")]
        ElderlyHome = 2,

        [Description("ملجأ")]
        Shelter = 3,

        [Description("بنك طعام")]
        FoodBank = 4,

        [Description("غير محدد")]
        Other = 5
    }
}
