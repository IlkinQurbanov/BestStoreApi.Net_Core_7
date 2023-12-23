using System.ComponentModel.DataAnnotations;

namespace BestStoreApi.Net_Core_7.Models
{
    public class UserProfileDto
    {
        public int Id { get; set; }
       
        public string FirstName { get; set; } = "";
       
        public string LastName { get; set; } = "";
       
        public string Email { get; set; } = ""; //Unique in the database
       
        public string Address { get; set; } = "";
       
        public string Phone { get; set; } = "";
    
        public string Role { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
