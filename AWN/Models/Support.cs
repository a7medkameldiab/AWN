namespace AWN.Models
{
    public class Support
    {
        public int Id { get; set; }
        public string Name { get; set; }     
        public string PhoneNumber { get; set; }
        public string Problem { get; set; }
        public Account Account { get; set; }
        public string AccountId { get; set; }
    }
}
