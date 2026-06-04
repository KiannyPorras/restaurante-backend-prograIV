using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/waiting-list")]
    [Route("api/waitinglist")]
    public class WaitingListController : ControllerBase
    {
        private readonly IWaitingListService _waitingListService;

        public WaitingListController(IWaitingListService waitingListService)
        {
            _waitingListService = waitingListService;
        }

        private WaitingListResponseDto MapToDto(Entities.WaitingList w) => new WaitingListResponseDto
        {
            Id = w.Id,
            ReqDate = w.ReqDate,
            DesiredDay = w.DesiredDay,
            DesiredTime = w.DesiredTime,
            GuestCount = w.GuestCount,
            PreferZone = w.PreferZone,
            Status = w.Status,
            ClientId = w.ClientId,
            ClientName = w.Client != null ? $"{w.Client.Name} {w.Client.LastName}" : string.Empty
        };

        // GET /api/waitinglist  [Admin]
        [Authorize]
        [HttpGet]
        public IActionResult GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var all = _waitingListService.GetAll().ToList();
            var totalCount = all.Count;
            var data = all.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).Select(MapToDto);
            return Ok(new PagedResult<WaitingListResponseDto>
            {
                Page = pagination.Page, PageSize = pagination.PageSize, TotalCount = totalCount, Data = data
            });
        }

        // GET /api/waitinglist/{id}  [Admin]
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var entrance = _waitingListService.GetById(id);
            if (entrance == null) return NotFound(new { message = $"Entrada con id {id} no encontrada." });
            return Ok(MapToDto(entrance));
        }

        // GET /api/waitinglist/pending  [Admin]
        [Authorize]
        [HttpGet("pending")]
        public IActionResult GetPending()
        {
            var list = _waitingListService.GetPending().Select(MapToDto).ToList();
            return Ok(list);
        }

        // POST /api/waitinglist  [público — clientes se agregan a la lista]
        [HttpPost]
        public IActionResult Create([FromBody] WaitingListCreateDto dto)
        {
            try
            {
                var entrance = _waitingListService.Create(
                    dto.ClientId, dto.DesiredDay, dto.DesiredTime, dto.GuestCount, dto.PreferZone);
                entrance = _waitingListService.GetById(entrance.Id)!;
                return CreatedAtAction(nameof(GetById), new { id = entrance.Id }, MapToDto(entrance));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH /api/waitinglist/{id}/assign  [Admin]
        [Authorize]
        [HttpPatch("{id}/assign")]
        public IActionResult Assign(int id, [FromBody] WaitingListAssignDto dto)
        {
            try
            {
                var reservation = _waitingListService.AssignTable(id, dto.TableId);
                return Ok(new ReservationResponseDto
                {
                    Id = reservation.Id,
                    Date = reservation.Date,
                    ReservationTime = reservation.ReservationTime,
                    GuestCount = reservation.GuestCount,
                    Status = reservation.Status,
                    CreatedAt = reservation.CreatedAt,
                    ClientId = reservation.ClientId,
                    ClientName = reservation.Client != null ? $"{reservation.Client.Name} {reservation.Client.LastName}" : string.Empty,
                    TableId = reservation.TableId,
                    TableNumber = reservation.Table?.Number ?? string.Empty,
                    ZoneName = reservation.Table?.Section?.Zone?.Name ?? string.Empty,
                    TurnId = reservation.TurnId,
                    TurnName = reservation.Turn?.Name ?? string.Empty
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH /api/waitinglist/{id}/cancel  [Admin]
        [Authorize]
        [HttpPatch("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                _waitingListService.Cancel(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found")
                    ? NotFound(new { message = ex.Message })
                    : BadRequest(new { message = ex.Message });
            }
        }
    }
}
