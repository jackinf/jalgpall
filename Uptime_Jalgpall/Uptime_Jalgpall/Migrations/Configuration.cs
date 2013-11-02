using Uptime_Jalgpall.Models;

namespace Uptime_Jalgpall.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Uptime_Jalgpall.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Uptime_Jalgpall.Models.ApplicationDbContext context)
        {
            context.Teams.AddOrUpdate(
                c => c.Name,
                new Team { Name = "Manchester United"},
                new Team { Name = "Real Madrid"},
                new Team { Name = "Barcelona"},
                new Team { Name = "CSKA"},
                new Team { Name = "Bayern Munich"},
                new Team { Name = "Chelsea"},
                new Team { Name = "Borussia Dortmund" },
                new Team { Name = "Manchester City" },
                new Team { Name = "Juventus" },
                new Team { Name = "AC Milan" },
                new Team { Name = "Arsenal"});

            context.Tournaments.AddOrUpdate(
                c => c.Name,
                new Tournament {Name = "UEFA Championship 2012"},
                new Tournament { Name = "FIFA World Cup"});

        }
    }
}
