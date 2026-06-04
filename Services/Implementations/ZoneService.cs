using RestauranteApi.DataBase;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Service.Implementations
{
    public class ZoneService : IZoneService
    {
        private readonly RestauranteApiDbContext _RestauranteApiDbContext;

        public ZoneService(RestauranteApiDbContext RestauranteApiDbContext)
        {
            _RestauranteApiDbContext = RestauranteApiDbContext;
        }

        public List<Zone> GetAll()
        {
            return _RestauranteApiDbContext.Zones.ToList();
        }

        public Zone? GetById(int id)
        {
            return _RestauranteApiDbContext.Zones.Find(id);
        }

        public Zone Create(string name)
        {
            Validate(name);

            var zone = new Zone { Name = name };
            _RestauranteApiDbContext.Zones.Add(zone);
            _RestauranteApiDbContext.SaveChanges();
            return zone;
        }

        public Zone Update(int id, string name)
        {
            Validate(name);

            var zone = _RestauranteApiDbContext.Zones.Find(id);
            if (zone == null) throw new Exception($"Zona con id {id} no encontrada.");

            zone.Name = name;
            _RestauranteApiDbContext.SaveChanges();
            return zone;
        }

        public void Delete(int id)
        {
            var zone = _RestauranteApiDbContext.Zones.Find(id);
            if (zone == null) throw new Exception($"Zona con id {id} no encontrada.");

            _RestauranteApiDbContext.Zones.Remove(zone);
            _RestauranteApiDbContext.SaveChanges();
        }

        private static void Validate(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception("El nombre de la zona es obligatorio.");
        }
    }
}
