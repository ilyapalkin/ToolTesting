using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using EntityFramework.Filters;
using FluentAssertions;
using Xunit;

namespace ToolTesting.EF.Filters
{
	public class FiltersTest
	{
		public FiltersTest()
		{
			Database.SetInitializer(new DropCreateDatabaseAlways<ExampleContext>());
			SeedDb();
			Database.SetInitializer(new CreateDatabaseIfNotExists<ExampleContext>());
		}

		[Fact(Skip = "TODO")]
		public void Seeds_with_correct_data()
		{
			using (var context = new ExampleContext())
			{
				var user = context.Users.Find(1);

				var company1 = context.Companies.Find(1);
				company1.Orders.Count.Should().Be(2);
				company1.Employees.Count.Should().Be(1);

				var company2 = context.Companies.Find(2);
				company2.Orders.Count.Should().Be(2);

				context.CurrentCompany = company1;
				context.EnableFilter("Company");

				var orders = context.Orders.ToList();
				orders.Should().HaveCount(2);
			}
		}

		[Fact]
		public void Should_filter_based_on_global_value()
		{
			using (var context = new ExampleContext())
			{
				var company = context.Companies.Find(1);
				context.CurrentCompany = company;
				context.EnableFilter("Company")
					.SetParameter("companyId", company.Id);

				Assert.Equal(2, context.Orders.Count());
			}
		}

		[Fact(Skip = "Expression compilation not working quite yet")]
		public void Should_filter_based_on_specific_value()
		{
			using (var context = new ExampleContext())
			{
				context.EnableFilter("systemOrder");

				var orders = context.Orders
					.ToList();

				Assert.Equal(1, orders.Count);
			}
		}

		private static void SeedDb()
		{
			var configuration = new MigrationsConfiguration();
			var migrator = new DbMigrator(configuration);
			migrator.Update();
		}
	}
}
