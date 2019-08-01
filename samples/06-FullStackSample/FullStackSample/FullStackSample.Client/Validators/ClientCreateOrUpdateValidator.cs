using FluentValidation;
using FluentValidation.Validators;
using FullStackSample.Api.Models;
using FullStackSample.Api.Requests;
using FullStackSample.Client.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FullStackSample.Client.Validators
{
	public class ClientCreateOrUpdateValidator : AbstractValidator<ClientCreateOrUpdate>
	{
		private readonly IApiService ApiService;

		public ClientCreateOrUpdateValidator(IApiService apiService)
		{
			When(x => !string.IsNullOrEmpty(x.Name), () =>
			{
				RuleFor(x => x.Name)
					.MustAsync(HaveUniqueName)
					.WithMessage("Name must be unique");
			});
			ApiService = apiService;
		}

		private async Task<bool> HaveUniqueName(
			ClientCreateOrUpdate client,
			string name,
			PropertyValidatorContext context,
			CancellationToken cancellationToken)
		{
			System.Diagnostics.Debug.WriteLine("Client validate name: Start");
			int? clientIdToIgnore = 
				client.Id == 0
				? (int?)null
				: client.Id;

			var apiQuery = new ClientIsNameTakenQuery(
				clientIdToIgnore: clientIdToIgnore,
				name: name);
			var apiResponse = 
				await ApiService.Execute<ClientIsNameTakenQuery, ClientIsNameTakenResponse>(apiQuery);
			System.Diagnostics.Debug.WriteLine("Client validate name: End");
			return !apiResponse.IsTaken;
		}
	}
}
