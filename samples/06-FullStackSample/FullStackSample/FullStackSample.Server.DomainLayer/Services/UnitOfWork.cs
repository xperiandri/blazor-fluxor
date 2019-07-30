using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FullStackSample.Server.DomainLayer.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FullStackSample.Server.DomainLayer.Services
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly FullStackDbContext DbContext;

		public UnitOfWork(FullStackDbContext dbContext)
		{
			DbContext = dbContext;
		}

		public async Task<UnitOfWorkResult> CommitAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				await DbContext.SaveChangesAsync(cancellationToken);
				return UnitOfWorkResult.Success;
			}
			catch (DbUpdateException e) when (e.IsUniqueIndexViolation())
			{
				KeyValuePair<string, string> violationInfo = e.GetViolationInfo();
				return new UnitOfWorkResult($"{violationInfo.Value} must be unique.");
			}
		}
	}
}
