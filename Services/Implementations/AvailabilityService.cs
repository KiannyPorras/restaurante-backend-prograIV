using Microsoft.EntityFrameworkCore;
using RestauranteApi.DataBase;
using RestauranteApi.DTOs;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Service.Implementations
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly RestauranteApiDbContext _db;
        private readonly ITurnService _turnService;

        public AvailabilityService(RestauranteApiDbContext db, ITurnService turnService)
        {
            _db = db;
            _turnService = turnService;
        }

        public List<Table> GetAvailableTables(DateOnly date, TimeSpan reservationTime, int guestCount, int? zoneId = null)
        {
            ValidateSearch(date, guestCount);
            // 1. Validar que hay un turno activo para esa hora
            var turn = _turnService.FindActiveTurnForTime(reservationTime);
            if (turn == null) throw new Exception("No hay ningún turno activo que cubra ese horario.");

            // 2. Obtener mesas activas con navegación a sección/zona
            var query = _db.Tables
                .Include(t => t.Section).ThenInclude(s => s.Zone)
                .Include(t => t.Reservations)
                .Include(t => t.LockTables)
                .Where(t => t.IsActive);

            // 3. Filtrar por zona si se especificó
            if (zoneId.HasValue)
                query = query.Where(t => t.Section.ZoneId == zoneId.Value);

            var tables = query.ToList();

            var turnFrom = date.ToDateTime(TimeOnly.FromTimeSpan(turn.StartTime));
            var turnTo = date.ToDateTime(TimeOnly.FromTimeSpan(turn.EndTime));

            // 4-6. Capacidad suficiente, sin reserva activa en ese turno/fecha, sin bloqueo
            return tables
                .Where(t => t.Capacity >= guestCount)
                .Where(t => !t.Reservations.Any(r =>
                    r.Date == date &&
                    r.TurnId == turn.Id &&
                    r.Status == ReservationStatus.Active))
                .Where(t => !t.LockTables.Any(l =>
                    l.IsActive &&
                    l.From < turnTo &&
                    l.To > turnFrom))
                .ToList();
        }

        public TurnAvailabilityResponseDto GetAvailabilityByTurn(DateOnly date, int turnId, int? zoneId = null)
        {
            ValidateDate(date);
            var turn = _turnService.GetById(turnId);
            if (turn == null) throw new Exception($"Turn with id {turnId} not found.");
            if (!turn.IsActive) throw new Exception($"El turno '{turn.Name}' no está activo.");

            var turnFrom = date.ToDateTime(TimeOnly.FromTimeSpan(turn.StartTime));
            var turnTo = date.ToDateTime(TimeOnly.FromTimeSpan(turn.EndTime));

            var query = _db.Tables
                .Include(t => t.Section).ThenInclude(s => s.Zone)
                .Include(t => t.Reservations).ThenInclude(r => r.Client)
                .Include(t => t.LockTables)
                .Where(t => t.IsActive);

            if (zoneId.HasValue)
                query = query.Where(t => t.Section.ZoneId == zoneId.Value);

            var tables = query.ToList();

            var tableStatuses = tables.Select(t => BuildTableAvailabilityDto(t, date, turn, turnFrom, turnTo)).ToList();

            return new TurnAvailabilityResponseDto
            {
                Date = date,
                TurnId = turn.Id,
                TurnName = turn.Name,
                TurnStart = turn.StartTime,
                TurnEnd = turn.EndTime,
                TotalTables = tableStatuses.Count,
                AvailableCount = tableStatuses.Count(x => x.IsAvailable),
                OccupiedCount = tableStatuses.Count(x => !x.IsAvailable && x.ActiveReservation != null),
                LockedCount = tableStatuses.Count(x => !x.IsAvailable && x.ActiveLock != null),
                Tables = tableStatuses
            };
        }

        public TableAvailabilityDto GetTableAvailabilityStatus(int tableId, DateOnly date, TimeSpan reservationTime)
        {
            ValidateDate(date);
            var table = _db.Tables
                .Include(t => t.Section).ThenInclude(s => s.Zone)
                .Include(t => t.Reservations).ThenInclude(r => r.Client)
                .Include(t => t.LockTables)
                .FirstOrDefault(t => t.Id == tableId);

            if (table == null) throw new Exception($"Table with id {tableId} not found.");
            if (!table.IsActive) throw new Exception($"La mesa {table.Number} no está activa.");

            var turn = _turnService.FindActiveTurnForTime(reservationTime);
            if (turn == null) throw new Exception("No hay ningún turno activo que cubra ese horario.");

            var turnFrom = date.ToDateTime(TimeOnly.FromTimeSpan(turn.StartTime));
            var turnTo = date.ToDateTime(TimeOnly.FromTimeSpan(turn.EndTime));

            return BuildTableAvailabilityDto(table, date, turn, turnFrom, turnTo);
        }

        private static TableAvailabilityDto BuildTableAvailabilityDto(
            Table table, DateOnly date, Turn turn, DateTime turnFrom, DateTime turnTo)
        {
            // Verificar reserva activa en ese turno/fecha
            var activeReservation = table.Reservations.FirstOrDefault(r =>
                r.Date == date &&
                r.TurnId == turn.Id &&
                r.Status == ReservationStatus.Active);

            // Verificar bloqueo activo que interseca cualquier parte del turno.
            var activeLock = table.LockTables.FirstOrDefault(l =>
                l.IsActive &&
                l.From < turnTo &&
                l.To > turnFrom);

            bool isAvailable = activeReservation == null && activeLock == null;

            string? reason = null;
            if (activeReservation != null)
                reason = $"Reservada para {activeReservation.GuestCount} persona(s) — turno {turn.Name}";
            else if (activeLock != null)
                reason = $"Bloqueada: {activeLock.Reason}";

            return new TableAvailabilityDto
            {
                TableId = table.Id,
                TableNumber = table.Number,
                Capacity = table.Capacity,
                SectionName = table.Section.Name,
                ZoneName = table.Section.Zone.Name,
                IsAvailable = isAvailable,
                UnavailabilityReason = reason,
                ActiveReservation = activeReservation == null ? null : new ActiveReservationSummaryDto
                {
                    ReservationId = activeReservation.Id,
                    ClientName = activeReservation.Client?.Name ?? "—",
                    GuestCount = activeReservation.GuestCount,
                    ReservationTime = activeReservation.ReservationTime
                },
                ActiveLock = activeLock == null ? null : new ActiveLockSummaryDto
                {
                    LockId = activeLock.Id,
                    Reason = activeLock.Reason,
                    From = activeLock.From,
                    To = activeLock.To
                }
            };
        }

        private static void ValidateSearch(DateOnly date, int guestCount)
        {
            ValidateDate(date);
            if (guestCount <= 0)
                throw new Exception("La cantidad de personas debe ser mayor que cero.");
        }

        private static void ValidateDate(DateOnly date)
        {
            if (date == default)
                throw new Exception("La fecha es obligatoria.");
            if (date < DateOnly.FromDateTime(DateTime.Today))
                throw new Exception("La fecha no puede estar en el pasado.");
        }
    }
}
