using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuestRooms6_Model
{
    public class AplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [NotMapped]
        public string StreetAddress { get; set; } = string.Empty;

        [NotMapped]
        public string City { get; set; } = string.Empty;

        [NotMapped]
        public string State { get; set; } = string.Empty;
        [NotMapped]
        public string PostalCode { get; set; } = string.Empty;
    }
}
