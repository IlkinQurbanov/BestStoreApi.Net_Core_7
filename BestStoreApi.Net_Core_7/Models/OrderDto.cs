using System.ComponentModel.DataAnnotations;

namespace BestStoreApi.Net_Core_7.Models
{
    public class OrderDto
    {
        [Required]
        public string ProductIdentifiers { get; set; } = "";
        [Required, MinLength(30), MaxLength(120)]
        public string DeliveryAddress { get; set; } = "";
        [Required]
        public string PaymentMethod { get; set; } = "";

    }
}
