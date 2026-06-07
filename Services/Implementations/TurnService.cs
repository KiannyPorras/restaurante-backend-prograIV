using RestauranteApi.DataBase;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Service.Implementations
{
    public class TurnService : ITurnService
    {
        private readonly RestauranteApiDbContext _RestauranteApiDbContext;

        public TurnService(RestauranteApiDbContext RestauranteApiDbContext)
        {
            _RestauranteApiDbContext = RestauranteApiDbContext;
        }

        public List<Turn> GetAll()
        {
            return _RestauranteApiDbContext.Turns.ToList();
        }

        public Turn? GetById(int id)
        {
            return _RestauranteApiDbContext.Turns.Find(id);
        }

        public Turn Create(string name, TimeSpan startTime, TimeSpan endTime)
        {
            Validate(name, startTime, endTime);

            var turn = new Turn
            {
                Name = name,
                StartTime = startTime,
                EndTime = endTime,
                IsActive = true
            };
            _RestauranteApiDbContext.Turns.Add(turn);
            _RestauranteApiDbContext.SaveChanges();
            return turn;
        }

        public Turn Update(int id, string name, TimeSpan startTime, TimeSpan endTime)
        {
            var turn = _RestauranteApiDbContext.Turns.Find(id);
            if (turn == null) throw new Exception($"Turn with id {id} not found.");

            Validate(name, startTime, endTime, id);

            turn.Name = name;
            turn.StartTime = startTime;
            turn.EndTime = endTime;
            _RestauranteApiDbContext.SaveChanges();
            return turn;
        }

        public void Delete(int id)
        {
            var turn = _RestauranteApiDbContext.Turns.Find(id);
            if (turn == null) throw new Exception($"Turn with id {id} not found.");

            _RestauranteApiDbContext.Turns.Remove(turn);
            _RestauranteApiDbContext.SaveChanges();
        }

        public void Activate(int id)
        {
            var turn = _RestauranteApiDbContext.Turns.Find(id);
            if (turn == null) throw new Exception($"Turn with id {id} not found.");

            Validate(turn.Name, turn.StartTime, turn.EndTime, turn.Id);

            turn.IsActive = true;
            _RestauranteApiDbContext.SaveChanges();
        }

        public void Deactivate(int id)
        {
            var turn = _RestauranteApiDbContext.Turns.Find(id);
            if (turn == null) throw new Exception($"Turn with id {id} not found.");

            turn.IsActive = false;
            _RestauranteApiDbContext.SaveChanges();
        }

        public Turn? FindActiveTurnForTime(TimeSpan time)
        {
            return _RestauranteApiDbContext.Turns
                .Where(t => t.IsActive)
                .AsEnumerable()
                .Where(t => t.StartTime <= time && time < t.EndTime)
                .FirstOrDefault();
        }

        private void Validate(string name, TimeSpan startTime, TimeSpan endTime, int? excludedTurnId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception("El nombre del turno es obligatorio.");
            if (startTime >= endTime) throw new Exception("La hora de inicio debe ser anterior a la hora de fin.");

            var overlaps = _RestauranteApiDbContext.Turns
                .AsEnumerable()
                .Any(t => t.IsActive
                       && t.Id != excludedTurnId
                       && t.StartTime < endTime
                       && t.EndTime > startTime);

            if (overlaps) throw new Exception("El turno se superpone con otro turno activo.");
        }
    }
}
