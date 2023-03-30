using System.Linq;
using EntityFrameworkCore.Triggered.IntegrationTests.OwnedEntity.Models;
using ScenarioTests;
using Xunit;

namespace EntityFrameworkCore.Triggered.IntegrationTests.OwnedEntity
{
    public partial class TestScenario
    {
        [Scenario(NamingPolicy = ScenarioTestMethodNamingPolicy.Test)]
        public void TestScenario1(ScenarioContext scenario)
        {
            var dbContext = new ApplicationDbContext(scenario.TargetName);

            var user = new User {
              UserName = "joe-biden",
              Name = new() {
                FirstName = "Joe",
                LastName = "Trump"
              },
              FullName = "Joe Trump"
            };

            // step 1: Populate database with a user
            dbContext.Users.Add(user);

            dbContext.SaveChanges();

            scenario.Fact("Database saved user", () => {
                var usersCount = dbContext.Users.Count();
                Assert.Equal(1, usersCount);
            });

            // step 2: Update Name.LastName
            user.Name.LastName = "Biden";

            dbContext.SaveChanges();

            scenario.Fact("FullName should be updated", () => {
                dbContext.Entry(user).Reload();
                Assert.Equal("Joe Biden", user.FullName);
            });
        }
    }
}
