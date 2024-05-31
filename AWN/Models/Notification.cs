using System.ComponentModel.DataAnnotations;

namespace AWN.Models
{
    public class Notification
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Content { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime TimesTamp { get; set; }
        public ICollection<Account> accounts { get; set; }
    }
}
