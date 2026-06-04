using System.ComponentModel.DataAnnotations;

namespace RestauranteApi.DTOs
{
    public class WaitingListCreateDto
    {
        [Range(1, int.MaxValue)]
        public int ClientId { get; set; }
        [Required]
        public DateOnly DesiredDay { get; set; }
        [Required]
        public TimeSpan DesiredTime { get; set; }
        [Range(1, int.MaxValue)]
        public int GuestCount { get; set; }
        [StringLength(80)]
        public string? PreferZone { get; set; }
    }

    public class WaitingListAssignDto
    {
        [Range(1, int.MaxValue)]
        public int TableId { get; set; }
    }

    public class WaitingListResponseDto
    {
        public int Id { get; set; }
        public DateTime ReqDate { get; set; }
        public DateOnly DesiredDay { get; set; }
        public TimeSpan DesiredTime { get; set; }
        public int GuestCount { get; set; }
        public string PreferZone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
    }
}
