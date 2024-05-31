using System.ComponentModel.DataAnnotations;

namespace AWN.Models
{
    public class RequestJoin
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(20)]
        public string Governorate { get; set; }
        [MaxLength(30)]
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public byte? Age { get; set; }
        [MaxLength(100)]
        public string ReasonOfJoin { get; set; }
        public bool IsAccepted { get; set; } = false;
        public string AccountId { get; set; }
        public Account Account { get; set; }

    }
}
