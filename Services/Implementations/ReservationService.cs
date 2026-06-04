using Microsoft.EntityFrameworkCore;
using RestauranteApi.DataBase;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;
namespace RestauranteApi.Service.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly RestauranteApiDbContext _RestauranteApiDbContext;
        private readonly ITurnService _turnService;

        public ReservationService(RestauranteApiDbContext RestauranteApiDbContext, ITurnService turnService)
        {
            _RestauranteApiDbContext = RestauranteApiDbContext;
            _turnService = turnService;
        }

        public List<Reservation> GetAll()
        {
            return _RestauranteApiDbContext.Reservations
                .Include(r => r.Client)
                .Include(r => r.Table).ThenInclude(t => t.Section).ThenInclude(s => s.Zone)
                .Include(r => r.Turn)
                .ToList();
        }

        public Reservation? GetById(int id)
        {
            return _RestauranteApiDbContext.Reservations
                .Include(r => r.Client)
                .Include(r => r.Table).ThenInclude(t => t.Section).ThenInclude(s => s.Zone)
                .Include(r => r.Turn)
                .FirstOrDefault(r => r.Id == id);
        }

        public List<Reservation> GetByClientId(int clientId)
        {
            return _RestauranteApiDbContext.Reservations
                .Include(r => r.Client)
                .Include(r => r.Table).ThenInclude(t => t.Section).ThenInclude(s => s.Zone)
                .Include(r => r.Turn)
                .Where(r => r.ClientId == clientId)
                .ToList();
        }

        public Reservation Create(int clientId, int tableId, DateOnly date, TimeSpan reservationTime, int guestCount)
        {
            ValidateGuestCount(guestCount);
            ValidateReservationDate(date);

            // R1: El cliente debe existir
            var client = _RestauranteApiDbContext.Clients.Find(clientId);
            if (client == null) throw new Exception($"Client with id {clientId} not found.");

            // R2: La mesa debe existir y estar activa
            var table = _RestauranteApiDbContext.Tables.Find(tableId);
            if (table == null || !table.IsActive)
                throw new Exception($"Table with id {tableId} not available.");

            // R3: Debe haber un turno activo para ese horario
            var turn = _turnService.FindActiveTurnForTime(reservationTime);
            if (turn == null)
                throw new Exception("No hay ningún turno activo que cubra ese horario.");

            // R4: La mesa debe tener capacidad suficiente
            if (table.Capacity < guestCount)
                throw new Exception($"La mesa solo tiene capacidad para {table.Capacity} personas.");

            // R8: No puede haber otra reserva Activa en la misma mesa, fecha y turno
            var conflict = _RestauranteApiDbContext.Reservations
                .Any(r => r.TableId == tableId
                       && r.Date == date
                       && r.TurnId == turn.Id
                       && r.Status == ReservationStatus.Active);

            if (conflict)
                throw new Exception("La mesa ya tiene una reserva activa en ese turno y fecha.");

            // R9: La mesa no puede estar bloqueada en ese momento
            var turnFrom = date.ToDateTime(TimeOnly.FromTimeSpan(turn.StartTime));
            var turnTo = date.ToDateTime(TimeOnly.FromTimeSpan(turn.EndTime));
            var blocked = _RestauranteApiDbContext.LockTables
                .Any(l => l.TableId == tableId
                       && l.IsActive
                       && l.From < turnTo
                       && l.To > turnFrom);

            if (blocked)
                throw new Exception("La mesa está bloqueada en ese horario.");

            var reservation = new Reservation
            {
                ClientId = clientId,
                TableId = tableId,
                TurnId = turn.Id,
                Date = date,
                ReservationTime = reservationTime,
                GuestCount = guestCount,
                Status = ReservationStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _RestauranteApiDbContext.Reservations.Add(reservation);
            _RestauranteApiDbContext.SaveChanges();
            return reservation;
        }

        public Reservation Update(int id, DateOnly date, TimeSpan reservationTime, int guestCount)
        {
            ValidateGuestCount(guestCount);
            ValidateReservationDate(date);

            var reservation = _RestauranteApiDbContext.Reservations.Find(id);
            if (reservation == null) throw new Exception($"Reservation with id {id} not found.");
            if (reservation.Status != ReservationStatus.Active)
                throw new Exception("Solo se puede modificar una reserva Activa.");

            // Validar turno para el nuevo horario
            var turn = _turnService.FindActiveTurnForTime(reservationTime);
            if (turn == null) throw new Exception("No hay ningún turno activo que cubra ese horario.");

            var table = _RestauranteApiDbContext.Tables.Find(reservation.TableId);
            if (table == null || !table.IsActive)
                throw new Exception($"Table with id {reservation.TableId} not available.");
            if (table.Capacity < guestCount)
                throw new Exception($"La mesa solo tiene capacidad para {table.Capacity} personas.");

            // Verificar conflicto con otras reservas (excluyendo la actual)
            var conflict = _RestauranteApiDbContext.Reservations
                .Any(r => r.TableId == reservation.TableId
                       && r.Date == date
                       && r.TurnId == turn.Id
                       && r.Status == ReservationStatus.Active
                       && r.Id != id);

            if (conflict) throw new Exception("La mesa ya tiene una reserva activa en ese turno y fecha.");

            var turnFrom = date.ToDateTime(TimeOnly.FromTimeSpan(turn.StartTime));
            var turnTo = date.ToDateTime(TimeOnly.FromTimeSpan(turn.EndTime));
            var blocked = _RestauranteApiDbContext.LockTables
                .Any(l => l.TableId == reservation.TableId
                       && l.IsActive
                       && l.From < turnTo
                       && l.To > turnFrom);

            if (blocked)
                throw new Exception("La mesa está bloqueada en ese turno.");

            reservation.Date = date;
            reservation.ReservationTime = reservationTime;
            reservation.TurnId = turn.Id;
            reservation.GuestCount = guestCount;

            _RestauranteApiDbContext.SaveChanges();
            return reservation;
        }

        public void Cancel(int id)
        {
            var reservation = _RestauranteApiDbContext.Reservations.Find(id);
            if (reservation == null) throw new Exception($"Reservation with id {id} not found.");
            if (reservation.Status != ReservationStatus.Active)
                throw new Exception("Solo se puede cancelar una reserva Activa.");

            // Guardar en historial antes de cambiar estado (R5)
            var history = new ReservationHistory
            {
                ReservationId = id,
                PrevState = reservation.Status,
                NewState = ReservationStatus.Cancelled,
                ChangedAt = DateTime.UtcNow
            };

            reservation.Status = ReservationStatus.Cancelled;
            _RestauranteApiDbContext.ReservationsHistory.Add(history);
            _RestauranteApiDbContext.SaveChanges();
        }

        public void MarkAsAttended(int id)
        {
            var reservation = _RestauranteApiDbContext.Reservations.Find(id);
            if (reservation == null) throw new Exception($"Reservation with id {id} not found.");
            if (reservation.Status != ReservationStatus.Active)
                throw new Exception("Solo se puede marcar como Atendida una reserva Activa.");

            // Guardar en historial (R6)
            var history = new ReservationHistory
            {
                ReservationId = id,
                PrevState = reservation.Status,
                NewState = ReservationStatus.Attended,
                ChangedAt = DateTime.UtcNow
            };

            reservation.Status = ReservationStatus.Attended;
            _RestauranteApiDbContext.ReservationsHistory.Add(history);
            _RestauranteApiDbContext.SaveChanges();
        }

        public List<ReservationHistory> GetHistory(int reservationId)
        {
            return _RestauranteApiDbContext.ReservationsHistory
                .Where(h => h.ReservationId == reservationId)
                .ToList();
        }

        private static void ValidateGuestCount(int guestCount)
        {
            if (guestCount <= 0)
                throw new Exception("La cantidad de personas debe ser mayor que cero.");
        }

        private static void ValidateReservationDate(DateOnly date)
        {
            if (date == default)
                throw new Exception("La fecha de la reserva es obligatoria.");
            if (date < DateOnly.FromDateTime(DateTime.Today))
                throw new Exception("No se puede crear o modificar una reserva para una fecha pasada.");
        }
    }
}
