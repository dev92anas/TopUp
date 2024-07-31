namespace TopUp.Domain.Entities
{
    public class Lookup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int LookupId { get; set; }
    }
}
