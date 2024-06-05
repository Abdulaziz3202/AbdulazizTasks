using System.ComponentModel.DataAnnotations;

namespace MVCRESTAPI.Modles
{
    public class Command
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [MaxLength(250)]
        public string HowTo { get; set; }
        
        [Required]
        public string Line { get; set; }

        [MaxLength(250)]
        public string Platform { get; set; }
    }
}
