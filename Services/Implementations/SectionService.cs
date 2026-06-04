using Microsoft.EntityFrameworkCore;
using RestauranteApi.DataBase;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Service.Implementations
{
    public class SectionService : ISectionService
    {
        public readonly RestauranteApiDbContext _RestauranteApiDbContext;

        public SectionService(RestauranteApiDbContext RestauranteApiDbContext)
        {
            _RestauranteApiDbContext = RestauranteApiDbContext;
        }

        public List<Section> GetAll()
        {
            return _RestauranteApiDbContext.Sections.Include(s => s.Zone).ToList();
        }

        public Section? GetById(int id)
        {
            return _RestauranteApiDbContext.Sections.Include(s => s.Zone).FirstOrDefault(s => s.Id == id);
        }

        public List<Section> GetByZoneId(int zoneId)
        {
            return _RestauranteApiDbContext.Sections
                .Include(s => s.Zone)
                .Where(s => s.ZoneId == zoneId)
                .ToList();
        }
        public Section Create(string name, int zoneId)
        {
            ValidateName(name);

            var zone = _RestauranteApiDbContext.Zones.Find(zoneId);
            if (zone == null) throw new Exception($"Zone with id {zoneId} not found");

            var section = new Section
            {
                Name = name,
                ZoneId = zoneId,
            };
            _RestauranteApiDbContext.Sections.Add(section);
            _RestauranteApiDbContext.SaveChanges();
            section.Zone = zone;
            return section;
        }

        public Section Update(int id, string name, int zoneId)
        {
            ValidateName(name);

            var section = _RestauranteApiDbContext.Sections.Find(id);
            if (section == null) throw new Exception($"Section with id {id} not found");
            var zone = _RestauranteApiDbContext.Zones.Find(zoneId);
            if (zone == null) throw new Exception($"Zone with id {zoneId} not found");
            section.Name = name;
            section.ZoneId = zoneId;
            section.Zone = zone;
            _RestauranteApiDbContext.SaveChanges();
            return section;
        }

        public void Delete(int id)
        {
            var section = _RestauranteApiDbContext.Sections.Find(id);
            if (section == null) throw new Exception($"Section with id {id} not found");
            _RestauranteApiDbContext.Sections.Remove(section);
            _RestauranteApiDbContext.SaveChanges();
        }

        private static void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception("El nombre de la sección es obligatorio.");
        }
    }
}
