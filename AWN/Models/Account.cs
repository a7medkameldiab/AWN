using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;

namespace AWN.Models
{
    public class Account : IdentityUser
    {
        public string? DonationNumber { get; set; }
        public string? PhotoUrl { get; set; }
        public ICollection<DonateCase> donateCases { get; set; }
        public RequestJoin requestJoins { get; set; }
        public ICollection<Notification> notifications { get; set; }
        public List<Suggestion> suggestions { get; set; }  
        public List<Payment> payments { get; set; }
        public  List<Support> support { get; set; }
    }
}
