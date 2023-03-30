using EntityFrameworkCore.Triggered.IntegrationTests.OwnedEntity.Models;
using Microsoft.EntityFrameworkCore;

namespace EntityFrameworkCore.Triggered.IntegrationTests.OwnedEntity
{
    public class ApplicationDbContext : DbContext
    {
        readonly string _databaseName;

        public ApplicationDbContext(string databaseName)
        {
            _databaseName = databaseName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(_databaseName);
            optionsBuilder.UseTriggers(triggerOptions => {
                triggerOptions.AddTrigger<Triggers.Users.UpdateUserFullName>();
            });
        }

        public DbSet<User> Users { get; set; }
    }
}
