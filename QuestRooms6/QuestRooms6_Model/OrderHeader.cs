using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuestRooms6_Model
{
    public class OrderHeader
    {

        [Key]
        public int Id { get; set; }



        public string? CreatedByUserId { get; set; }
        [ForeignKey("CreatedByUserId")]
        public AplicationUser? CreatedBy { get; set; }



        [Required]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        [Required]
        public int CountRoom { get; set; }
        [Required]
        public double FinalTotalPrice { get; set; }


        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;

        public static explicit operator OrderHeader(List<OrderHeader> v)
        {
            throw new NotImplementedException();
        }
    }
}
