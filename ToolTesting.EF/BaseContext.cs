using System.Data.Entity;
using System.Diagnostics;

namespace ToolTesting.EF
{
	public abstract class BaseContext : DbContext
	{
		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
#if DEBUG
			LogGeneratedSql();
#endif
			base.OnModelCreating(modelBuilder);
		}

		private void LogGeneratedSql()
		{
			Database.Log = log => Debug.Write(log);
		}
	}
}