using System.Data.Entity.Migrations;
using WebApiApplication.Models;

namespace WebApiApplication.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<WebApiDBContext>
    {
        public Configuration()
        {
            //自動遷移是否啟用
            AutomaticMigrationsEnabled = true;

            //自動遷移是否允許數據丟失
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(WebApiDBContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.
        }
    }
}
