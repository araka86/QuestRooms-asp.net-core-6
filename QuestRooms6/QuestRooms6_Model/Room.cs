using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestRooms6_Model
{
   
    public enum FearLevel { Scary, Unscary }
    public enum DifficultLevel {Hard, Medium, Easy }
    public class Room
    {
        [Key]
        public int Id { get; set; }   
        
        public string Image { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [Range(0, 5)]
        [Required]
        public int CountPlayers { get; set; }
        [Required]
        public string DifltLevel { get; set; } = "a";
        [Required]
        public string FLevel { get; set; } = "a";

        [Required]
        [Range(1, int.MaxValue)]
        public double Price { get; set; }

        [NotMapped]
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOrderRoom { get; set; }

        [NotMapped]
        public string TimeOrderRoom { get; set; } = String.Empty;




        [NotMapped]
        public DifficultLevel DifficultLevel { get; set; }
        [NotMapped]
        public FearLevel FearLevel { get; set; }

        [NotMapped]
        public string IsReset { get; set; } = string.Empty;



        public byte[] BytesImage { get; set; } = new byte[0];
    }
}
