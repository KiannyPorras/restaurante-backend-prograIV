using System.ComponentModel.DataAnnotations;

namespace RestauranteApi.DTOs
{
    public class TableCreateDto
    {
        [Required, StringLength(30)]
        public string Number { get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }
        [Range(1, int.MaxValue)]
        public int SectionId { get; set; }
    }

    public class TableResponseDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string ZoneName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Estado de disponibilidad de una mesa para una fecha/turno concreto.
    /// </summary>
    public class TableAvailabilityDto
    {
        public int TableId { get; set; }
        public string TableNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string ZoneName { get; set; } = string.Empty;

        /// <summary>True si la mesa puede recibir una reserva nueva.</summary>
        public bool IsAvailable { get; set; }

        /// <summary>Razón por la que no está disponible (null cuando sí lo está).</summary>
        public string? UnavailabilityReason { get; set; }

        /// <summary>Detalle de la reserva activa, si existe.</summary>
        public ActiveReservationSummaryDto? ActiveReservation { get; set; }

        /// <summary>Detalle del bloqueo activo, si existe.</summary>
        public ActiveLockSummaryDto? ActiveLock { get; set; }
    }

    public class ActiveReservationSummaryDto
    {
        public int ReservationId { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public int GuestCount { get; set; }
        public TimeSpan ReservationTime { get; set; }
    }

    public class ActiveLockSummaryDto
    {
        public int LockId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    /// <summary>
    /// Resumen de disponibilidad de todas las mesas para un turno/fecha.
    /// </summary>
    public class TurnAvailabilityResponseDto
    {
        public DateOnly Date { get; set; }
        public int TurnId { get; set; }
        public string TurnName { get; set; } = string.Empty;
        public TimeSpan TurnStart { get; set; }
        public TimeSpan TurnEnd { get; set; }
        public int TotalTables { get; set; }
        public int AvailableCount { get; set; }
        public int OccupiedCount { get; set; }
        public int LockedCount { get; set; }
        public List<TableAvailabilityDto> Tables { get; set; } = new();
    }
}
