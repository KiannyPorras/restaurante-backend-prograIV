using RestauranteApi.Entities;

namespace RestauranteApi.Service.Interfaces
{
    public interface ILockTableService
    {
        List<LockTable> GetAllList();
        LockTable? GetById(int id);
        List<LockTable> GetByTableId(int tableId);
        LockTable Create(int tableId, string reason, DateTime from, DateTime to);
        LockTable Update(LockTable lockTable);
        void Deactivate(int id);
        void Delete(int id);
    }
}
