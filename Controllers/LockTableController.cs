using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Controllers
{
    [ApiController]
    [Route("api/table-locks")]
    [Route("api/lockstable")]
    [Authorize]
    public class LockTableController : ControllerBase
    {
        private readonly ILockTableService _lockTableService;

        public LockTableController(ILockTableService lockTableService)
        {
            _lockTableService = lockTableService;
        }

        private LockTableResponseDto MapToDto(Entities.LockTable l) => new LockTableResponseDto
        {
            Id = l.Id,
            Reason = l.Reason,
            From = l.From,
            To = l.To,
            IsActive = l.IsActive,
            TableId = l.TableId,
            TableNumber = l.Table?.Number ?? string.Empty
        };

        // GET /api/lockstable  [Admin]
        [HttpGet]
        public IActionResult GetAll([FromQuery] PaginationQueryDto pagination)
        {
            var all = _lockTableService.GetAllList().ToList();
            var totalCount = all.Count;
            var data = all.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).Select(MapToDto);
            return Ok(new PagedResult<LockTableResponseDto>
            {
                Page = pagination.Page, PageSize = pagination.PageSize, TotalCount = totalCount, Data = data
            });
        }

        // GET /api/lockstable/{id}  [Admin]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var lockTable = _lockTableService.GetById(id);
            if (lockTable == null) return NotFound(new { message = $"Bloqueo con id {id} no encontrado." });
            return Ok(MapToDto(lockTable));
        }

        // GET /api/lockstable/table/{tableId}  [Admin]
        [HttpGet("table/{tableId}")]
        public IActionResult GetByTable(int tableId)
        {
            var lockTables = _lockTableService.GetByTableId(tableId).Select(MapToDto).ToList();
            return Ok(lockTables);
        }

        // POST /api/lockstable  [Admin]
        [HttpPost]
        public IActionResult Create([FromBody] LockTableCreateDto dto)
        {
            try
            {
                var lockTable = _lockTableService.Create(dto.TableId, dto.Reason, dto.From, dto.To);
                lockTable = _lockTableService.GetById(lockTable.Id)!;
                return CreatedAtAction(nameof(GetById), new { id = lockTable.Id }, MapToDto(lockTable));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT /api/lockstable/{id}  [Admin]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] LockTableCreateDto dto)
        {
            try
            {
                var lockTable = _lockTableService.Update(new LockTable
                {
                    Id = id,
                    TableId = dto.TableId,
                    Reason = dto.Reason,
                    From = dto.From,
                    To = dto.To,
                    IsActive = true
                });
                lockTable = _lockTableService.GetById(lockTable.Id)!;
                return Ok(MapToDto(lockTable));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PATCH /api/lockstable/{id}/cancel  [Admin]
        [HttpPatch("{id}/cancel")]
        public IActionResult Cancel(int id)
        {
            try
            {
                _lockTableService.Deactivate(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // DELETE /api/lockstable/{id}  [Admin]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _lockTableService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
