using AWN.Models;
using System.ComponentModel.DataAnnotations;

namespace AWN.Dtos.AdminDto
{
    public class UpdateCaseDto
    {
        public string? Title { get; set; }
        public string? SubTitle { get; set; }
        [MaxLength(100)]
        public string? Location { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Current amount must be a non-negative value.")]
        public double? CurrentAmount { get; set; }
        public double? TargetAmount { get; set; }
        [Required]
        public DonateCaseState? State { get; set; } = DonateCaseState.InProgress;
        public List<IFormFile>? Photos { get; set; }
    }
}
