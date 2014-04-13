using System;
using System.Data.Entity;

namespace ToolTesting.EF
{
	public abstract class EntityFrameworkTestBase<TContext> : IDisposable
		where TContext : DbContext, new()
	{
		protected readonly TContext Context;

		protected EntityFrameworkTestBase()
			: this(new TContext())
		{
		}

		protected EntityFrameworkTestBase(TContext context)
		{
			this.Context = context;
			// TODO: How to seed data per test fixture.
			Seed(context);
		}

		protected abstract void Seed(DbContext context);

		protected void VerifyInTheSeparateContext(Action<TContext> verify)
		{
			using (var context = new TContext())
			{
				verify(context);
			}
		}

		public void Dispose()
		{
			// TODO: How to delete data base per assembly or test fixture.
			Context.Database.Delete();
			Context.Dispose();
		}
	}
}