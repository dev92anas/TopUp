namespace TopUp.Domain.Entities
{
    public class Beneficiary
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Nickname { get; set; }
        public string PhoneNumber { get; set; }
    }
}
