using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BestStoreApi.Net_Core_7.Models
{
    [Index("Email", IsUnique = true)]
    public class PasswordReset
    {

        public int Id { get; set; }
        [MaxLength(100)]
        public string Email { get; set; } = "";
        [MaxLength(120)]
        public string Token { get; set; } = "";
        public DateTime CreatedAt { get; set; } =  DateTime.Now;

    }
}
