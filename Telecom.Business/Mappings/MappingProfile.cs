using AutoMapper;
using Telecom.Business.DTOs.Ticket;
using Telecom.Domain.Entities;

namespace Telecom.Business.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateTicketDto, Ticket>();
        CreateMap<Ticket, TicketResponseDto>();
    }
}
