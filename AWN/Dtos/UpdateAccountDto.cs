using System.ComponentModel.DataAnnotations;

namespace AWN.Dtos
{
    public class UpdateAccountDto
    {
        [StringLength(100)]
        public string? UserName { get; set; }
        [StringLength(200)]
        public string? Password { get; set; }
        [StringLength(150)]
        public string? Email { get; set; }
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
