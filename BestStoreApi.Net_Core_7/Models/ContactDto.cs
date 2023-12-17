
using System.ComponentModel.DataAnnotations;

namespace BestStoreApi.Net_Core_7.Models
{
    public class ContactDto
    {
        [Required, MaxLength(120)]
        public string FirstName { get; set; } = "";
        [Required,MaxLength(120)]
        public string LastName { get; set; } = "";
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = "";
        [MaxLength(100)]
        public string Phone { get; set; } = "";
       
        public int SubjectId { get; set; }
        [Required, MinLength(30), MaxLength(4000)]
        public string Message { get; set; } = "";
    }
}
