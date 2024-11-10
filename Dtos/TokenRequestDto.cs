using System.ComponentModel.DataAnnotations;

namespace KPMGTask.Dtos
{
    public class TokenRequestDto
    {

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
