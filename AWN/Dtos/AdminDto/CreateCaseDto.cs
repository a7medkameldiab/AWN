using AWN.Models;
using NuGet.Protocol.Plugins;
using System.ComponentModel.DataAnnotations;

namespace AWN.Dtos.AdminDto
{
    public class CreateCaseDto
    {
        [Required]
        public string Title { get; set; }
        public string SubTitle { get; set; }
        [MaxLength(100)]
        public string Location { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Current amount must be a non-negative value.")]
        public double CurrentAmount { get; set; } = 0;
        [Range(0, double.MaxValue, ErrorMessage = "Target amount must be a non-negative value.")]
        public double TargetAmount { get; set; }
        public DonateCaseState State { get; set; } = DonateCaseState.InProgress;
        public List<IFormFile> Photos { get; set; }
        public string Category { get; set; }
    }
}
