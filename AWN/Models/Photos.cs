using System.ComponentModel.DataAnnotations;

namespace AWN.Models
{
    public class Photos
    {
        public int Id { get; set; }
        [Required]
        public byte[] Photo {  get; set; }
        public int DonateCaseId { get; set; }
        public DonateCase DonateCase { get; set; }
    }
}
