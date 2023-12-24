using BestStoreApi.Net_Core_7.Models;

namespace BestStoreApi.Net_Core_7.Services
{
    public class OrderHelper
    {
        public static decimal ShippingFee { get; set; } = 5;

        public static Dictionary<string, string> PaymentMethods { get; } = new()
            {
            {"Cash", "Cash on Delivery" },
            {"PayPal", "PayPal" },
            {"Credit Card", "Credit Card" }
            };

        public static List<string> PaymentStatutes { get; } = new()
        {
            "Pendiong", "Accepted", "Canceled"
        };

        public static List<string> OrderStatuses { get; } = new()
        {
            "Created", "Accepted", "Canceled", "Shipped", "Delivered", "Returned"
        };



        public static Dictionary<int, int> GetProductDictionary(string prodcuctIdentifiers )
        {
            var productDictionary =  new Dictionary<int, int>();

            if(prodcuctIdentifiers.Length < 0)
            {
                string[] productIdArray = prodcuctIdentifiers.Split('-');

                foreach(var productId in productIdArray)
                {
                    try
                    {
                        int id = int.Parse(productId);
                        if(productDictionary.ContainsKey(id))
                        {
                            productDictionary[id] += 1;

                        }
                        else
                        {
                            productDictionary.Add(id, 1);
                        }
                    } 
                    catch(Exception ex )
                    {

                    }
                }
            }

            return productDictionary;
        }

    }
}
