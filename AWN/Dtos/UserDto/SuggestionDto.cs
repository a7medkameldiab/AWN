using System.ComponentModel.DataAnnotations;
using AWN.Models;

namespace AWN.Dtos.UserDto
{
    public class SuggestionDto
    {
        [MaxLength(100)]
        public string Address { get; set; }
        public string Details { get; set; }
        public string PhoneNumber { get; set; }
        public SuggestionSort Sort { get; set; }
    }
}
