using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/sections")]
    public class SectionController : ControllerBase
    {
        private readonly ISectionService _sectionService;

        public SectionController(ISectionService sectionService)
        {
            _sectionService = sectionService;
        }

        private SectionResponseDto MapToDto(Section s) => new SectionResponseDto
        {
            Id = s.Id,
            Name = s.Name,
            ZoneId = s.ZoneId,
            ZoneName = s.Zone.Name
        };

        // GET /api/sections
        [HttpGet]
        public IActionResult GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var all = _sectionService.GetAll().ToList();
            var totalCount = all.Count;
            var data = all.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).Select(MapToDto);
            return Ok(new PagedResult<SectionResponseDto>
            {
                Page = pagination.Page, PageSize = pagination.PageSize, TotalCount = totalCount, Data = data
            });
        }

        // GET /api/sections/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var section = _sectionService.GetById(id);
            if (section == null) return NotFound(new { message = $"Sección con id {id} no encontrada." });
            return Ok(MapToDto(section));
        }

        // GET /api/sections/zone/{zoneId}
        [HttpGet("zone/{zoneId}")]
        public IActionResult GetByZone(int zoneId)
        {
            var sections = _sectionService.GetByZoneId(zoneId).Select(MapToDto).ToList();
            return Ok(sections);
        }

        // POST /api/sections
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] SectionCreateDto dto)
        {
            try
            {
                var section = _sectionService.Create(dto.Name, dto.ZoneId);
                return CreatedAtAction(nameof(GetById), new { id = section.Id }, MapToDto(section));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/sections/{id}
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] SectionCreateDto dto)
        {
            try
            {
                var section = _sectionService.Update(id, dto.Name, dto.ZoneId);
                return Ok(MapToDto(section));
            }
            catch (Exception ex)
            {
                return ex.Message.Contains("not found")
                    ? NotFound(new { message = ex.Message })
                    : BadRequest(new { message = ex.Message });
            }
        }

        // DELETE /api/sections/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _sectionService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
