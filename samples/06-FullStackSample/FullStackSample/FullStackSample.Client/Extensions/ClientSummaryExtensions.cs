using FullStackSample.Api.Models;
using FullStackSample.Client.Store.EntityStateEvents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FullStackSample.Client.Extensions
{
	public static class ClientSummaryExtensions
	{
		public static ClientSummary UpdateState(
			this ClientSummary clientSummary,
			ClientStateNotification modifiedState)
		{
			if (clientSummary.Id != modifiedState.Id)
				return clientSummary;
			if (modifiedState.StateUpdateKind == StateUpdateKind.Created)
				return clientSummary;
			if (modifiedState.StateUpdateKind == StateUpdateKind.Deleted)
				return null;

			return new ClientSummary(
				id: clientSummary.Id,
				name: modifiedState.Name.UpdatedValue(clientSummary.Name));
		}

		public static IEnumerable<ClientSummary> UpdateState(
			this IEnumerable<ClientSummary> source,
			ClientStateNotification modifiedState)
		{
			if (source == null)
				return null;

			switch (modifiedState.StateUpdateKind)
			{
				case StateUpdateKind.Created:
					source = source.Append(new ClientSummary(
						id: modifiedState.Id,
						name: modifiedState.Name.GetValueOrDefault()));
					break;

				case StateUpdateKind.Deleted:
					source = source.Where(x => x.Id != modifiedState.Id);
					break;

				case StateUpdateKind.Modified:
					source = source.Select(x => x.UpdateState(modifiedState));
					break;

				default:
					throw new NotImplementedException(modifiedState.StateUpdateKind.ToString());
			}
			return source.ToList();
		}
	}
}
