using System.ComponentModel.DataAnnotations;

namespace RestauranteApi.DTOs
{
    public class LockTableCreateDto
    {
        [Range(1, int.MaxValue)]
        public int TableId { get; set; }
        [Required, StringLength(200)]
        public string Reason { get; set; } = string.Empty;
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }
    }

    public class LockTableResponseDto
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool IsActive { get; set; }
        public int TableId { get; set; }
        public string TableNumber { get; set; } = string.Empty;
    }
}
