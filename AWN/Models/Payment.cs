namespace AWN.Models
{
    public enum PaymentMethod
    {
        Cash = 0,
        Card = 1
    }
    public class Payment
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime TimesTamp { get; set; }
        public PaymentMethod PaymentMethod { get; set;} = PaymentMethod.Cash;
        public string AccountId { get; set; }
        public Account Account { get; set; }
        public int DonateCaseId { get; set; }
        public DonateCase DonateCase { get; set; }
    }
}
