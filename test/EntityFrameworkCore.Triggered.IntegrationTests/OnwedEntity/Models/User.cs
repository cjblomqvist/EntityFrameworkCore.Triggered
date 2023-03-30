namespace EntityFrameworkCore.Triggered.IntegrationTests.OwnedEntity.Models
{
    public class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public Name Name { get; set; } = new();

        public string FullName { get; set; }
    }
}
