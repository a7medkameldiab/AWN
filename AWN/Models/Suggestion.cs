using System.ComponentModel.DataAnnotations;

namespace AWN.Models
{
    public enum SuggestionSort
    {
        DonateOtherThanMoney = 0,
        SuggestCase = 1
    }
    public class Suggestion
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Address { get; set; }
        public string Details { get; set; }
        public string PhoneNumber { get; set; }
        public SuggestionSort Sort { get; set; }
        public bool IsAccepted { get; set; } = false;
        public string AccountId { get; set; }   
        public Account Account { get; set; }
    }
}
