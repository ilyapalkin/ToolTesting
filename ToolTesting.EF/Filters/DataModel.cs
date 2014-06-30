using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ToolTesting.EF.Filters
{
	public interface ICompanySpecific
	{
		int CompanyId { get; set; }

		Company Company { get; set; }
	}

	public interface IUserSpecific
	{
		int UserId { get; set; }

		User User { get; set; }
	}

	public class User : ICompanySpecific
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		
		public int CompanyId { get; set; }

		public Company Company { get; set; }
	}

	public class Order : ICompanySpecific, IUserSpecific
	{
		public int Id { get; set; }
		public string Number { get; set; }

		public int CompanyId { get; set; }

		public Company Company { get; set; }

		public int UserId { get; set; }

		public User User { get; set; }
	}

	public class Company
	{
		public Company()
		{
			Employees = new Collection<User>();
			Orders = new Collection<Order>();
		}

		public int Id { get; set; }

		public string Name { get; set; }

		public ICollection<Order> Orders { get; set; }

		public ICollection<User> Employees { get; set; }
	}
}
