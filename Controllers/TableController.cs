using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/tables")]
    public class TableController : ControllerBase
    {
        private readonly ITableService _tableService;
        private readonly IAvailabilityService _availabilityService;

        public TableController(ITableService tableService, IAvailabilityService availabilityService)
        {
            _tableService = tableService;
            _availabilityService = availabilityService;
        }

        private TableResponseDto MapToDto(Table t) => new TableResponseDto
        {
            Id = t.Id,
            Number = t.Number,
            Capacity = t.Capacity,
            IsActive = t.IsActive,
            SectionId = t.SectionId,
            SectionName = t.Section.Name,
            ZoneName = t.Section.Zone.Name
        };

        // GET /api/tables
        [HttpGet]
        public IActionResult GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var all = _tableService.GetAll().ToList();
            var totalCount = all.Count;
            var data = all.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).Select(MapToDto);
            return Ok(new PagedResult<TableResponseDto>
            {
                Page = pagination.Page, PageSize = pagination.PageSize, TotalCount = totalCount, Data = data
            });
        }

        // GET /api/tables/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var table = _tableService.GetById(id);
            if (table == null) return NotFound(new { message = $"Mesa con id {id} no encontrada." });
            return Ok(MapToDto(table));
        }

        // GET /api/tables/section/{sectionId}
        [HttpGet("section/{sectionId}")]
        public IActionResult GetBySection(int sectionId)
        {
            var tables = _tableService.GetBySectionId(sectionId).Select(MapToDto).ToList();
            return Ok(tables);
        }

        // GET /api/tables/zone/{zoneId}
        [HttpGet("zone/{zoneId}")]
        public IActionResult GetByZone(int zoneId)
        {
            var tables = _tableService.GetByZoneId(zoneId).Select(MapToDto).ToList();
            return Ok(tables);
        }

        // GET /api/tables/available?date=&time=&capacity=&zoneId=
        [HttpGet("available")]
        public IActionResult GetAvailable([FromQuery] DateOnly date, [FromQuery] TimeSpan time,
            [FromQuery] int capacity, [FromQuery] int? zoneId = null)
        {
            try
            {
                var tables = _availabilityService.GetAvailableTables(date, time, capacity, zoneId);
                return Ok(tables.Select(MapToDto).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/tables/availability/turn/{turnId}?date=&zoneId=
        [HttpGet("availability/turn/{turnId}")]
        public IActionResult GetAvailabilityByTurn(int turnId, [FromQuery] DateOnly date, [FromQuery] int? zoneId = null)
        {
            try
            {
                return Ok(_availabilityService.GetAvailabilityByTurn(date, turnId, zoneId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/tables/{id}/availability?date=&time=
        [HttpGet("{id}/availability")]
        public IActionResult GetTableAvailability(int id, [FromQuery] DateOnly date, [FromQuery] TimeSpan time)
        {
            try
            {
                return Ok(_availabilityService.GetTableAvailabilityStatus(id, date, time));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/tables
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] TableCreateDto dto)
        {
            try
            {
                var table = _tableService.Create(dto.Number, dto.Capacity, dto.SectionId);
                return CreatedAtAction(nameof(GetById), new { id = table.Id }, MapToDto(table));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/tables/{id}
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TableCreateDto dto)
        {
            try
            {
                var table = _tableService.Update(id, dto.Number, dto.Capacity, dto.SectionId);
                return Ok(MapToDto(table));
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found")
                    ? NotFound(new { message = ex.Message })
                    : BadRequest(new { message = ex.Message });
            }
        }

        // PATCH /api/tables/{id}/activate
        [Authorize]
        [HttpPatch("{id}/activate")]
        public IActionResult Activate(int id)
        {
            try
            {
                _tableService.Activate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // PATCH /api/tables/{id}/deactivate
        [Authorize]
        [HttpPatch("{id}/deactivate")]
        public IActionResult Deactivate(int id)
        {
            try
            {
                _tableService.Deactivate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE /api/tables/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _tableService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
