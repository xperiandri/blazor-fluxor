using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using FullStackSample.Api.Requests;
using FullStackSample.Server.DomainLayer.Extensions;
using FullStackSample.Server.DomainLayer.Services;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace FullStackSample.Server.DomainLayer.RequestHandlers
{
	public class ClientCreateCommandHandler : IRequestHandler<ClientCreateCommand, ClientCreateResponse>
	{
		private readonly FullStackDbContext DbContext;
		private readonly IMapper Mapper;
		private readonly IValidator<Api.Models.ClientCreateDto> Validator;
		private readonly IUnitOfWork UnitOfWork;

		public ClientCreateCommandHandler(
			FullStackDbContext dbContext,
			IMapper mapper,
			IValidator<Api.Models.ClientCreateDto> validator,
			IUnitOfWork unitOfWork)
		{
			DbContext = dbContext;
			Mapper = mapper;
			Validator = validator;
			UnitOfWork = unitOfWork;
		}

		public async Task<ClientCreateResponse> Handle(ClientCreateCommand request, CancellationToken cancellationToken)
		{
			ValidationResult validationResult =
				await Validator.ValidateAsync(request.Client, cancellationToken);

			if (!validationResult.IsValid)
				return new ClientCreateResponse(null, validationResult.ToResponseErrors());

			var clientEntity = Mapper.Map<Entities.Client>(request.Client);
			DbContext.Clients.Add(clientEntity);

			UnitOfWorkResult updateResult = await UnitOfWork.CommitAsync();
			if (updateResult.HasError)
				return new ClientCreateResponse(
					errorMessage: updateResult.Error,
					validationErrors: null);

			var apiClient = Mapper.Map<Api.Models.ClientCreateDto>(clientEntity);
			return new ClientCreateResponse(clientEntity.Id, apiClient);
		}
	}
}
