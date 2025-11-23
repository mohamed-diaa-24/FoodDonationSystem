using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDonationSystem.Core.DTOs.Restaurant
{
    public class RestaurantImageDto
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
    }
}
