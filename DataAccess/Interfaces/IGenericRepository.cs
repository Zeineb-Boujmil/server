namespace DataAccess.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        void Create(T entity);
        void Save();
    }
}
