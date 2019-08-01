using FullStackSample.Api.Requests;
using FullStackSample.Server.DomainLayer.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FullStackSample.Server.DomainLayer.RequestHandlers
{
	public class ClientIsNameTakenQueryHandler : IRequestHandler<ClientIsNameTakenQuery, ClientIsNameTakenResponse>
	{
		private readonly FullStackDbContext DbContext;

		public ClientIsNameTakenQueryHandler(FullStackDbContext dbContext)
		{
			DbContext = dbContext;
		}

		public async Task<ClientIsNameTakenResponse> Handle(ClientIsNameTakenQuery query, CancellationToken cancellationToken)
		{
			IQueryable<Entities.Client> dbQuery = DbContext.Clients
				.Where(x => x.Name == query.Name);
			if (query.ClientIdToIgnore.HasValue)
				dbQuery = dbQuery.Where(x => x.Id != query.ClientIdToIgnore.Value);

			bool found = await dbQuery.AnyAsync();
			return new ClientIsNameTakenResponse(isTaken: found);
		}
	}
}
