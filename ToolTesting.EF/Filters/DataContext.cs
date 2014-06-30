using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using EntityFramework.Filters;
using FluentAssertions;
using ToolTesting.EF.HowToDeleteItemsFromCollection;
using Xunit;

namespace ToolTesting.EF.Filters
{
	public class MigrationsConfiguration : DbMigrationsConfiguration<ExampleContext>
	{
		public MigrationsConfiguration()
		{
			AutomaticMigrationsEnabled = true;
			AutomaticMigrationDataLossAllowed = true;
		}

		protected override void Seed(ExampleContext context)
		{
			var company1 = new Company
			{
				Name = "Company 1"
			};
			
			context.Companies.Add(company1);
			context.SaveChanges();

			context.CurrentCompany = company1;

			var user = new User
			{
				FirstName = "John",
				LastName = "Doe"
			};
			context.Users.Add(user);
			context.SaveChanges();

			context.CurrentUser = user;

			company1.Orders.Add(new Order
			{
				Number = "First No",
				User = user,
				Company = company1
			});
			company1.Orders.Add(new Order
			{
				Number = "First No",
				User = user,
				Company = company1
			});
			context.SaveChanges();

			var company2 = new Company
			{
				Name = "Company 2"
			};

			context.Companies.Add(company2);
			context.SaveChanges();

			context.CurrentCompany = company2;

			company2.Orders.Add(new Order
			{
				Number = "Third No (system)",
				User = user,
				Company = company2
			});
			company2.Orders.Add(new Order
			{
				Number = "Forth No",
				User = user,
				Company = company2
			});
			context.SaveChanges();
		}
	}

	public class ExampleConfiguration : DbConfiguration
	{
		public ExampleConfiguration()
		{
			AddInterceptor(new FilterInterceptor());
		}
	}

	public class ExampleContext : BaseContext
	{
		public DbSet<Company> Companies { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Order> Orders { get; set; }

		public Company CurrentCompany { get; set; }
		public User CurrentUser { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>()
				.HasRequired(user => user.Company)
				.WithMany(company => company.Employees)
				.WillCascadeOnDelete(false);

			modelBuilder.Entity<Order>()
				.HasRequired(order => order.Company)
				.WithMany(company => company.Orders)
				.WillCascadeOnDelete(false);
			modelBuilder.Entity<Order>()
				.Filter("systemOrder", fc => fc.Condition(bo => bo.Number.Contains("system")))
				.HasRequired(order => order.User)
				.WithMany()
				.WillCascadeOnDelete(false);

			modelBuilder.Conventions.Add(FilterConvention.Create<ICompanySpecific, int>("Company", (e, companyId) => e.CompanyId == companyId));
			modelBuilder.Conventions.Add(FilterConvention.Create<IUserSpecific, int>("User", (e, userId) => e.UserId == userId));
		}

		public override int SaveChanges()
		{
			var companyEntities = ChangeTracker.Entries<ICompanySpecific>().ToArray();
			foreach (var item in companyEntities.Where(t => t.State == EntityState.Added))
			{
				item.Entity.Company = CurrentCompany;
			}

			var userEntities = ChangeTracker.Entries<IUserSpecific>().ToArray();
			foreach (var item in userEntities.Where(t => t.State == EntityState.Added))
			{
				item.Entity.User = CurrentUser;
			}
			return base.SaveChanges();
		}
	}
}
