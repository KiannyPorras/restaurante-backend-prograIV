using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IReservationService _reservationService;

        public ClientController(IClientService clientService, IReservationService reservationService)
        {
            _clientService = clientService;
            _reservationService = reservationService;
        }

        private ClientResponseDto MapToDto(Client c) => new ClientResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            LastName = c.LastName,
            Phone = c.Phone
        };

        // GET /api/clients  [Admin]
        [Authorize]
        [HttpGet]
        public IActionResult GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var all = _clientService.GetAll().ToList();
            var totalCount = all.Count;
            var data = all.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).Select(MapToDto);
            return Ok(new PagedResult<ClientResponseDto>
            {
                Page = pagination.Page, PageSize = pagination.PageSize, TotalCount = totalCount, Data = data
            });
        }

        // GET /api/clients/{id}  [Admin]
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var client = _clientService.GetById(id);
            if (client == null) return NotFound(new { message = $"Cliente con id {id} no encontrado." });
            return Ok(MapToDto(client));
        }

        // GET /api/clients/{id}/reservations  [Admin]
        [Authorize]
        [HttpGet("{id}/reservations")]
        public IActionResult GetReservations(int id)
        {
            var client = _clientService.GetById(id);
            if (client == null) return NotFound(new { message = $"Cliente con id {id} no encontrado." });

            var reservas = _reservationService.GetByClientId(id)
                .Select(r => new ReservationResponseDto
                {
                    Id = r.Id,
                    Date = r.Date,
                    ReservationTime = r.ReservationTime,
                    GuestCount = r.GuestCount,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    ClientId = r.ClientId,
                    ClientName = r.Client != null ? $"{r.Client.Name} {r.Client.LastName}" : string.Empty,
                    TableId = r.TableId,
                    TableNumber = r.Table.Number,
                    ZoneName = r.Table.Section.Zone.Name,
                    TurnId = r.TurnId,
                    TurnName = r.Turn.Name
                }).ToList();

            return Ok(reservas);
        }

        // POST /api/clients  [público — registro de clientes]
        [HttpPost]
        public IActionResult Create([FromBody] ClientCreateDto dto)
        {
            try
            {
                var cliente = _clientService.Create(dto.Name, dto.LastName, dto.Phone);
                return CreatedAtAction(nameof(GetById), new { id = cliente.Id }, MapToDto(cliente));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/clients/{id}  [Admin]
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ClientCreateDto dto)
        {
            try
            {
                var client = _clientService.Update(id, dto.Name, dto.LastName, dto.Phone);
                return Ok(MapToDto(client));
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE /api/clients/{id}  [Admin]
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _clientService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
