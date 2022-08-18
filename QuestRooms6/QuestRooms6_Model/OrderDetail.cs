using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuestRooms6_Model
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }



        
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public OrderHeader OrderHeader { get; set; }


        [Required]
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }



        public double Price { get; set; }

      
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOrderRoom { get; set; }
        [Required]
        public string TimeOrderRoom { get; set; } = DateTime.Now.ToString();





      



   
    }
}