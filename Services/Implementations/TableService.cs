using Microsoft.EntityFrameworkCore;
using RestauranteApi.DataBase;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Service.Implementations
{
    public class TableService : ITableService
    {
        private readonly RestauranteApiDbContext _RestauranteApiDbContext;

        public TableService(RestauranteApiDbContext RestauranteApiDbContext)
        {
            _RestauranteApiDbContext = RestauranteApiDbContext;
        }

        public List<Table> GetAll()
        {
            return _RestauranteApiDbContext.Tables
                .Include(t => t.Section).ThenInclude(s => s.Zone)
                .ToList();
        }

        public Table? GetById(int id)
        {
            return _RestauranteApiDbContext.Tables
                .Include(t => t.Section).ThenInclude(s => s.Zone)
                .FirstOrDefault(t => t.Id == id);
        }

        public List<Table> GetBySectionId(int sectionId)
        {
            return _RestauranteApiDbContext.Tables
                .Include(t => t.Section).ThenInclude(s => s.Zone)
                .Where(t => t.SectionId == sectionId)
                .ToList();
        }

        public List<Table> GetByZoneId(int zoneId)
        {
            return _RestauranteApiDbContext.Tables
                .Include(t => t.Section).ThenInclude(s => s.Zone)
                .Where(t => t.Section.ZoneId == zoneId)
                .ToList();
        }

        public Table Create(string number, int capacity, int sectionId)
        {
            Validate(number, capacity);

            var section = _RestauranteApiDbContext.Sections
                .Include(s => s.Zone)
                .FirstOrDefault(s => s.Id == sectionId);
            if (section == null) throw new Exception($"Section with id {sectionId} not found.");

            var table = new Table
            {
                Number = number,
                Capacity = capacity,
                SectionId = sectionId,
                IsActive = true,
                Section = section
            };
            _RestauranteApiDbContext.Tables.Add(table);
            _RestauranteApiDbContext.SaveChanges();
            return table;
        }

        public Table Update(int id, string number, int capacity, int sectionId)
        {
            Validate(number, capacity, id);

            var table = _RestauranteApiDbContext.Tables
                .Include(t => t.Section).ThenInclude(s => s.Zone)
                .FirstOrDefault(t => t.Id == id);
            if (table == null) throw new Exception($"Table with id {id} not found.");

            var section = _RestauranteApiDbContext.Sections
                .Include(s => s.Zone)
                .FirstOrDefault(s => s.Id == sectionId);
            if (section == null) throw new Exception($"Section with id {sectionId} not found.");

            table.Number = number;
            table.Capacity = capacity;
            table.SectionId = sectionId;
            table.Section = section;
            _RestauranteApiDbContext.SaveChanges();
            return table;
        }

        public void Deactivate(int id)
        {
            var table = _RestauranteApiDbContext.Tables.Find(id);
            if (table == null) throw new Exception($"Table with id {id} not found.");

            table.IsActive = false;
            _RestauranteApiDbContext.SaveChanges();
        }

        public void Activate(int id)
        {
            var table = _RestauranteApiDbContext.Tables.Find(id);
            if (table == null) throw new Exception($"Table with id {id} not found.");

            table.IsActive = true;
            _RestauranteApiDbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var table = _RestauranteApiDbContext.Tables.Find(id);
            if (table == null) throw new Exception($"Table with id {id} not found.");

            _RestauranteApiDbContext.Tables.Remove(table);
            _RestauranteApiDbContext.SaveChanges();
        }

        private void Validate(string number, int capacity, int? excludedTableId = null)
        {
            if (string.IsNullOrWhiteSpace(number)) throw new Exception("El número de mesa es obligatorio.");
            if (capacity <= 0) throw new Exception("La capacidad de la mesa debe ser mayor que cero.");

            var duplicatedNumber = _RestauranteApiDbContext.Tables
                .Any(t => t.Number == number && t.Id != excludedTableId);
            if (duplicatedNumber) throw new Exception($"Ya existe una mesa con el número '{number}'.");
        }
    }
}
