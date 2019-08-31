using FluentValidation;

namespace FullStackSample.Api.Models
{
	public class ClientCreateDto
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int RegistrationNumber { get; set; }

		public ClientCreateDto() { }

		public ClientCreateDto(int id, string name, int registrationNumber) : this()
		{
			Id = id;
			Name = name;
			RegistrationNumber = registrationNumber;
		}
	}

	public class ClientCreateDtoValidator : AbstractValidator<ClientCreateDto>
	{
		public ClientCreateDtoValidator()
		{
			RuleFor(x => x.Name)
				.NotEmpty()
				.WithMessage("Name required");
			RuleFor(x => x.RegistrationNumber)
				.GreaterThan(0)
				.WithMessage("Registration number must be greater than 0");
		}
	}
}
