using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using WebApiApplication.Models;

namespace WebApiApplication.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<WebApiDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebApiDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
