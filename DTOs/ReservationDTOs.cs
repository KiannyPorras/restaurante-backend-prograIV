using RestauranteApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace RestauranteApi.DTOs
{
    public class ReservationCreateDto
    {
        [Range(1, int.MaxValue)]
        public int ClientId { get; set; }
        [Range(1, int.MaxValue)]
        public int TableId { get; set; }
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public TimeSpan ReservationTime { get; set; }
        [Range(1, int.MaxValue)]
        public int GuestCount { get; set; }
    }

    public class ReservationUpdateDto
    {
        [Required]
        public DateOnly Date { get; set; }
        [Required]
        public TimeSpan ReservationTime { get; set; }
        [Range(1, int.MaxValue)]
        public int GuestCount { get; set; }
    }

    public class ReservationResponseDto
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int GuestCount { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ClientId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int TableId { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public string ZoneName { get; set; } = string.Empty;
        public int TurnId { get; set; }
        public string TurnName { get; set; } = string.Empty;
    }

    public class ReservationHistoryResponseDto
    {
        public int Id { get; set; }
        public ReservationStatus PrevState { get; set; }
        public ReservationStatus NewState { get; set; }
        public DateTime ChangedAt { get; set; }
        public int ReservationId { get; set; }
    }
}
