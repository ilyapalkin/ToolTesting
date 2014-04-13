using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace ToolTesting.EF.HowToDeleteItemsFromCollection
{
	public class DeletingObjectFromCollection : EntityFrameworkTestBase<CustomerContext>
	{
		[Fact]
		public void CustomerPropertiesShouldBeUpdated()
		{
			// Arrange.
			var customer = Context.Customers.First();

			// Act.
			customer.Name = "New Name";
			customer.Address = "New Address";
			Context.SaveChanges();

			// Assert.
			VerifyInTheSeparateContext(context =>
				{
					var result = context.Customers.Find(customer.Id);
					result.Should().NotBeNull();
					result.Name.Should().Be(customer.Name);
					result.Address.Should().Be(customer.Address);
				});
		}

		[Fact]
		public void PurchaseShouldBeAdded()
		{
			// Arrange.
			var customer = Context.Customers.First();
			var purchase = new Purchase()
				{
					PurchaseDate = DateTime.Today.AddDays(10),
					Customer = customer
				};


			// Act.
			customer.Purchases.Add(purchase);
			Context.SaveChanges();

			// Assert.
			VerifyInTheSeparateContext(context =>
				{
					var result = context.Customers.Find(customer.Id);
					result.Should().NotBeNull();
					result.Purchases.Should().HaveCount(2);
					var addedPurchase = result.Purchases.First(it => it.Id == purchase.Id);
					addedPurchase.Should().NotBeNull();
				});
		}

		[Fact]
		public void PurchaseShouldBeDeleted()
		{
			// Arrange.
			var customer = Context.Customers.First();
			var purchase = customer.Purchases.First();

			// Act.
			customer.Purchases.Remove(purchase);
			Context.SaveChanges();

			// Assert.
			VerifyInTheSeparateContext(context =>
				{
					var result = context.Customers.Find(customer.Id);
					result.Purchases.Should().HaveCount(0);
				});
		}

		[Fact]
		public void PurchaseShouldBeEdited()
		{
			// Arrange.
			var customer = Context.Customers.First();
			var purchase = customer.Purchases.First();

			// Act.
			purchase.PurchaseDate = DateTime.Now.AddDays(100);
			Context.SaveChanges();

			// Assert.
			VerifyInTheSeparateContext(context =>
				{
					var result = context.Customers.Find(customer.Id);
					result.Purchases.First().PurchaseDate.Should().BeCloseTo(purchase.PurchaseDate);
				});
		}

		[Fact]
		public void AllPurchasesAreDeleted_WhenCustomerIsDeleted()
		{
			// Arrange.
			var customerToDelete = Context.Customers.First();

			// Act.
			Context.Customers.Remove(customerToDelete);
			Context.SaveChanges();

			// Assert.
			VerifyInTheSeparateContext(context =>
				{
					context.Customers.Should().HaveCount(0);
					context.Purchases.Should().HaveCount(0);
				});
		}

		protected override void Seed(DbContext context)
		{
			var customers = new List<Customer>
				{
					new Customer()
						{
							Name = "Customer"
						}
				};

			customers.ForEach(it =>
				{
					it.Purchases.Add(new Purchase()
						{
							PurchaseDate = DateTime.Today,
							Customer = it
						});
				});

			context.Set<Customer>().AddOrUpdate(
				it => it.Name,
				customers.ToArray());

			context.SaveChanges();
		}
	}
}
