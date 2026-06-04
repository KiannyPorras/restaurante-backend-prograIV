using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;
using RestauranteApi.DataBase;
using Microsoft.EntityFrameworkCore;

namespace RestauranteApi.Service.Implementations
{
    public class LockTableService : ILockTableService
    {
        private readonly RestauranteApiDbContext _RestauranteApiDbContext;

        public LockTableService(RestauranteApiDbContext RestauranteApiDbContext)
        {
            _RestauranteApiDbContext = RestauranteApiDbContext;
        }

        public List<LockTable> GetAllList()
        {
            return _RestauranteApiDbContext.LockTables.Include(l => l.Table).ToList();
        }

        public LockTable? GetById(int id)
        {
            return _RestauranteApiDbContext.LockTables.Include(l => l.Table).FirstOrDefault(l => l.Id == id);
        }

        public List<LockTable> GetByTableId(int tableId)
        {
            return _RestauranteApiDbContext.LockTables
                .Include(l => l.Table)
                .Where(l => l.TableId == tableId)
                .ToList();
        }
        public LockTable Create(int tableId, string reason, DateTime from, DateTime to)
        {
            Validate(tableId, reason, from, to);

            var lockTable = new LockTable
            {
                TableId = tableId,
                Reason = reason,
                From = from,
                To = to,
                IsActive = true
            };

            _RestauranteApiDbContext.LockTables.Add(lockTable);
            _RestauranteApiDbContext.SaveChanges();
            return lockTable;
        }

        public void Deactivate(int id)
        {
            var lockTable = _RestauranteApiDbContext.LockTables.Find(id);
            if (lockTable == null) throw new Exception($"LockTable with id {id} not found.");

            lockTable.IsActive = false;
            _RestauranteApiDbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var lockTable = _RestauranteApiDbContext.LockTables.Find(id);
            if (lockTable == null) throw new Exception($"LockTable with id {id} not found.");

            _RestauranteApiDbContext.LockTables.Remove(lockTable);
            _RestauranteApiDbContext.SaveChanges();

        }

        

        public LockTable Update(LockTable lockTable)
        {
            var result = _RestauranteApiDbContext.LockTables.Find(lockTable.Id);
            if (result == null) throw new Exception($"LockTable with id {lockTable.Id} not found.");

            Validate(lockTable.TableId, lockTable.Reason, lockTable.From, lockTable.To, lockTable.Id);

            result.TableId = lockTable.TableId;
            result.Reason = lockTable.Reason;
            result.From = lockTable.From;
            result.To = lockTable.To;
            result.IsActive = lockTable.IsActive;

            _RestauranteApiDbContext.SaveChanges();
            return result;
        }

        private void Validate(int tableId, string reason, DateTime from, DateTime to, int? excludedLockId = null)
        {
            var table = _RestauranteApiDbContext.Tables.Find(tableId);
            if (table == null) throw new Exception($"Table with id {tableId} not found.");
            if (!table.IsActive) throw new Exception($"Table with id {tableId} is not active.");
            if (string.IsNullOrWhiteSpace(reason)) throw new Exception("La razón del bloqueo es obligatoria.");
            if (from >= to) throw new Exception("La fecha de inicio debe ser anterior a la fecha de fin.");

            var overlap = _RestauranteApiDbContext.LockTables
                .Where(l => l.TableId == tableId && l.IsActive && l.Id != excludedLockId)
                .Any(l => l.From < to && l.To > from);

            if (overlap) throw new Exception("Ya existe un bloqueo activo en ese intervalo para la mesa.");

            var reservations = _RestauranteApiDbContext.Reservations
                .Include(r => r.Turn)
                .Where(r => r.TableId == tableId && r.Status == ReservationStatus.Active)
                .ToList();

            var conflictsWithReservation = reservations.Any(r =>
            {
                var reservationFrom = r.Date.ToDateTime(TimeOnly.FromTimeSpan(r.Turn.StartTime));
                var reservationTo = r.Date.ToDateTime(TimeOnly.FromTimeSpan(r.Turn.EndTime));
                return reservationFrom < to && reservationTo > from;
            });

            if (conflictsWithReservation)
                throw new Exception("No se puede bloquear la mesa porque tiene una reserva activa en ese intervalo.");
        }
    }
}
