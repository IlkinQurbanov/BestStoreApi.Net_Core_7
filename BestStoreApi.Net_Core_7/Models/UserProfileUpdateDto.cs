using System.ComponentModel.DataAnnotations;

namespace BestStoreApi.Net_Core_7.Models
{
    public class UserProfileUpdateDto
    {
        [MaxLength(100)]
        public string FirstName { get; set; } = "";
        [MaxLength(100)]
        public string LastName { get; set; } = "";
        [MaxLength(100)]
        public string Email { get; set; } = ""; //Unique in the database
        [MaxLength(100)]
        public string Address { get; set; } = "";
        [MaxLength(20)]
        public string Phone { get; set; } = "";
    }
}
