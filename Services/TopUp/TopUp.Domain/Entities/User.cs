using System.ComponentModel.DataAnnotations;

namespace TopUp.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public bool IsVerified { get; set; }
    }
}
