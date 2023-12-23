using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestStoreApi.Net_Core_7.Models
{
    [Index("Email", IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
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
        [MaxLength(100)]
        public string Password { get; set; } = "";
        [MaxLength(50)]
        public string Role { get; set; } = "";
        public DateTime Created { get; set; } = DateTime.Now;
    }
}
