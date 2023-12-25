using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestStoreApi.Net_Core_7.Models
{
    public class Order
    {

        public int Id { get; set; }
        public int UserId {  get; set; }
        public DateTime CreatedAt { get; set; }

        [Precision(16,2)]
        public decimal ShippingFee {  get; set; }
        [MaxLength(150)]
        public string DeliveryAddress { get; set; } = "";
        [MaxLength(40)]
        public string PaymentMethod { get; set; } = "";
        [MaxLength(40)]
        public string PaymentStatus { get; set; } = "";
        [MaxLength(40)]
        public string OrderStatus { get; set; } = "";

        //navigate properties

        public User User { get; set; } = null!;

        public List<OrderItem> OrderItems { get; set; }
    }
}
