using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Booking.API.Services;
using Booking.Application.Booking.Commands.CreateBooking;
using Booking.Application.Booking.Commands.UpdateBooking;
using Booking.Application.Booking.Queries.DTO;
//using Booking.Application.Booking.Commands.CreateBooking;
//using Booking.Application.Booking.Commands.UpdateBooking;
using Booking.Application.Booking.Queries.GetBooking;
using Booking.Domain.AggregatesModel.BookingAggregate;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : BaseController
    {
        private readonly IMapper mapper;
        private readonly IMessageProducer _messageProducer;
        private readonly IMessageConsumer _messageConsumer;
        public BookingController(IMessageProducer messageProducer,IMapper mapper,IMessageConsumer messageConsumer)
        {
            this.mapper = mapper;
            _messageProducer = messageProducer;
            _messageConsumer = messageConsumer;
        }

        //[Produces("application/json")]
        [HttpGet("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(string id)
        {
            try
            {
                BookingOrder resultSet = await Mediator.Send(new GetBookingQuery() { BookingId = id });
                if (resultSet == null)
                {
                    return NotFound();
                }
                var bookingDTO = mapper.Map<BookingOrderDTO>(resultSet);
                return Ok(bookingDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Sorry Some problem Occured");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var bookingOrderId = await Mediator.Send(command);
                //We can replace this with CreatedAtAction as well
                return StatusCode(StatusCodes.Status201Created, bookingOrderId);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Sorry We are Unable to Create Booking.");
            }
        }

        // PUT: api/Booking/5
        [HttpPut("{bookingOrderId}")]
        public async Task Put(string bookingOrderId, [FromBody] UpdateBooking command)
        {
            command.BookingOrderId = bookingOrderId;
            await Mediator.Send(command);
        }
    }
}
