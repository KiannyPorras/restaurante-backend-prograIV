using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/zones")]
    public class ZonesController : ControllerBase
    {
        private readonly IZoneService _zoneService;

        public ZonesController(IZoneService zoneService)
        {
            _zoneService = zoneService;
        }

        private ZoneResponseDto MapToDto(Zone z) => new ZoneResponseDto
        {
            Id = z.Id,
            Name = z.Name
        };

        // GET /api/zones
        [HttpGet]
        public IActionResult GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var all = _zoneService.GetAll().ToList();
            var totalCount = all.Count;
            var data = all.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).Select(MapToDto);
            return Ok(new PagedResult<ZoneResponseDto>
            {
                Page = pagination.Page, PageSize = pagination.PageSize, TotalCount = totalCount, Data = data
            });
        }

        // GET /api/zones/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var zone = _zoneService.GetById(id);
            if (zone == null) return NotFound(new { message = $"Zona con id {id} no encontrada." });
            return Ok(MapToDto(zone));
        }

        // POST /api/zones
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] ZoneCreateDto dto)
        {
            try
            {
                var zone = _zoneService.Create(dto.Name);
                return CreatedAtAction(nameof(GetById), new { id = zone.Id }, MapToDto(zone));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/zones/{id}
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ZoneCreateDto dto)
        {
            try
            {
                var zone = _zoneService.Update(id, dto.Name);
                return Ok(MapToDto(zone));
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE /api/zones/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _zoneService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
