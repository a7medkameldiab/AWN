using System.ComponentModel.DataAnnotations;

namespace AWN.Dtos.UserDto
{
    public class RequestJoinDto
    {
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(20)]
        public string Governorate { get; set; }
        [MaxLength(30)]
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        [MaxLength(100)]
        public string ReasonOfJoin { get; set; }
    }
}
