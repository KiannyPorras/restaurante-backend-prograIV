

using RestauranteApi.DTOs;
using RestauranteApi.Entities;

namespace RestauranteApi.Service.Interfaces
{
    public interface IAvailabilityService
    {
        /// <summary>
        /// Devuelve las mesas libres para una fecha/hora y cantidad de comensales.
        /// Opcionalmente filtra por zona.
        /// </summary>
        List<Table> GetAvailableTables(DateOnly date, TimeSpan reservationTime, int guestCount, int? zoneId = null);

        /// <summary>
        /// Devuelve el estado de disponibilidad detallado de TODAS las mesas activas
        /// para una fecha y turno específico, indicando si están libres, reservadas o bloqueadas.
        /// </summary>
        TurnAvailabilityResponseDto GetAvailabilityByTurn(DateOnly date, int turnId, int? zoneId = null);

        /// <summary>
        /// Devuelve el estado de disponibilidad de una sola mesa para una fecha/hora.
        /// </summary>
        TableAvailabilityDto GetTableAvailabilityStatus(int tableId, DateOnly date, TimeSpan reservationTime);
    }
}
