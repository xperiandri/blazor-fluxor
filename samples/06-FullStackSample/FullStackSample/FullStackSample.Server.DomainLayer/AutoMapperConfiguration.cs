using AutoMapper;

namespace FullStackSample.Server.DomainLayer
{
	public class AutoMapperConfiguration : Profile
	{
		public AutoMapperConfiguration()
		{
			CreateMap<Entities.Client, Api.Models.ClientSummaryDto>();
			CreateMap<Api.Models.ClientCreateDto, Entities.Client>().ReverseMap();
		}
	}
}
