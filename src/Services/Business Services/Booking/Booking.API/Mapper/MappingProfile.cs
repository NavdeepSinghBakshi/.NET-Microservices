using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Booking.Domain.AggregatesModel.BookingAggregate;
using Booking.Application.Booking.Queries.DTO;

namespace Booking.API.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<BookingOrder, BookingOrderDTO>().ReverseMap();
            CreateMap<BookingOrderDetail, BookingOrderDetailDTO>().ReverseMap();
        }
    }
}
