using System.ComponentModel.DataAnnotations;

namespace AWN.Models
{
    public class Photos
    {
        public int Id { get; set; }
        public string PhotoUrl {  get; set; }
        public int DonateCaseId { get; set; }
        public DonateCase DonateCase { get; set; }
    }
}
