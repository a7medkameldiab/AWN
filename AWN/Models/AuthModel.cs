namespace AWN.Models
{
    public class AuthModel
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; } // user can assign to more than role
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
    }
}
