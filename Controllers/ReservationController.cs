using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/reservations")]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        private ReservationResponseDto MapToDto(Entities.Reservation r) => new ReservationResponseDto
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
        };

        // GET /api/reservations  [Admin]
        [Authorize]
        [HttpGet]
        public IActionResult GetAll(
            [FromQuery] PaginationQueryDto pagination,
            [FromQuery] DateOnly? date,
            [FromQuery] ReservationStatus? status,
            [FromQuery] int? clientId,
            [FromQuery] int? tableId,
            [FromQuery] int? turnId)
        {
            var query = _reservationService.GetAll().AsEnumerable();

            if (date.HasValue)
                query = query.Where(r => r.Date == date.Value);
            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);
            if (clientId.HasValue)
                query = query.Where(r => r.ClientId == clientId.Value);
            if (tableId.HasValue)
                query = query.Where(r => r.TableId == tableId.Value);
            if (turnId.HasValue)
                query = query.Where(r => r.TurnId == turnId.Value);

            var all = query.ToList();
            var totalCount = all.Count;
            var data = all.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).Select(MapToDto);
            return Ok(new PagedResult<ReservationResponseDto>
            {
                Page = pagination.Page, PageSize = pagination.PageSize, TotalCount = totalCount, Data = data
            });
        }

        // GET /api/reservations/{id}  [Admin]
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var reservation = _reservationService.GetById(id);
            if (reservation == null) return NotFound(new { message = $"Reserva con id {id} no encontrada." });
            return Ok(MapToDto(reservation));
        }

        // GET /api/reservations/{id}/historical  [Admin]
        [Authorize]
        [HttpGet("{id}/history")]
        [HttpGet("{id}/historical")]
        public IActionResult GetHistory(int id)
        {
            var reservation = _reservationService.GetById(id);
            if (reservation == null) return NotFound(new { message = $"Reserva con id {id} no encontrada." });

            var historical = _reservationService.GetHistory(id)
                .Select(h => new ReservationHistoryResponseDto
                {
                    Id = h.Id,
                    PrevState = h.PrevState,
                    NewState = h.NewState,
                    ChangedAt = h.ChangedAt,
                    ReservationId = h.ReservationId
                }).ToList();

            return Ok(historical);
        }

        // POST /api/reservations  [público — clientes hacen reservas]
        [HttpPost]
        public IActionResult Create([FromBody] ReservationCreateDto dto)
        {
            try
            {
                var reservation = _reservationService.Create(
                    dto.ClientId, dto.TableId, dto.Date, dto.ReservationTime, dto.GuestCount);
                var complete = _reservationService.GetById(reservation.Id)!;
                return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, MapToDto(complete));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/reservations/{id}  [Admin]
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ReservationUpdateDto dto)
        {
            try
            {
                var reservation = _reservationService.Update(id, dto.Date, dto.ReservationTime, dto.GuestCount);
                var complete = _reservationService.GetById(reservation.Id)!;
                return Ok(MapToDto(complete));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH /api/reservations/{id}/cancel  [Admin]
        [Authorize]
        [HttpPatch("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                _reservationService.Cancel(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found")
                    ? NotFound(new { message = ex.Message })
                    : Conflict(new { message = ex.Message });
            }
        }

        // PATCH /api/reservations/{id}/attend  [Admin]
        [Authorize]
        [HttpPatch("{id}/attend")]
        public IActionResult MarkAsAttended(int id)
        {
            try
            {
                _reservationService.MarkAsAttended(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found")
                    ? NotFound(new { message = ex.Message })
                    : Conflict(new { message = ex.Message });
            }
        }
    }
}
