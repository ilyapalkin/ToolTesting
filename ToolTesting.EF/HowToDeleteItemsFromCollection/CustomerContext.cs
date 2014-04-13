using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace ToolTesting.EF.HowToDeleteItemsFromCollection
{
	public class CustomerContext : BaseContext
	{
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Purchase>()
				.HasRequired(it => it.Customer)
				.WithMany(it => it.Purchases)
				.HasForeignKey(it => it.CustomerId);

			modelBuilder.Entity<Purchase>().HasKey(it => new
				{
					Id = it.Id,
					CustomerId = it.CustomerId
				});

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Customer> Customers { get; set; }
		public DbSet<Purchase> Purchases { get; set; }
	}

	public class Customer
	{
		public Customer()
		{
			Name = string.Empty;
			Purchases = new Collection<Purchase>();
			Address = string.Empty;
		}

		public int Id { get; set; }

		public string Name { get; set; }

		public string Address { get; set; }

		public virtual ICollection<Purchase> Purchases { get; private set; }
	}

		public class Purchase
		{
			[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
			public int Id { get; set; }

			public Customer Customer { get; set; }

			public int CustomerId { get; set; }

			public DateTime PurchaseDate { get; set; }
		}
}