using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AWN.Models
{
    public enum DonateCaseState
    {
        Done       = 0,
        Collected  = 1,
        InProgress = 2
    }
    public class DonateCase
    {
        public int Id { get; set; }
        [MaxLength(100)] 
        public string Title { get; set; }
        public string SubTitle { get; set; }
        [MaxLength(100)]
        public string Location { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Current amount must be a non-negative value.")]
        public double CurrentAmount
        {
            get => _currentAmount;
            set
            {
                _currentAmount = value;
                if(CurrentAmount >= TargetAmount)
                {
                    State = DonateCaseState.Collected;
                }
            }
        }
        private double _currentAmount;
        [Range(0, double.MaxValue, ErrorMessage = "Target amount must be a non-negative value.")]
        public double TargetAmount { get; set; }
        [Required]
        public DonateCaseState State { get; set; } = DonateCaseState.InProgress;
        public DateTime TimesTamp {  get; set; }
        public ICollection<Photos> Photos { get; set; }  = new List<Photos>();
        public ICollection<Account> accounts { get; set; }
        public ICollection<Payment> Payments { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public double ExcessAmount { get;  set; }
    }
}
