namespace BestStoreApi.Net_Core_7.Models
{
    public class CartItemDto
    {
        public Product Product { get; set; } = new Product();
        public int Quantity { get; set;}

    }
}
