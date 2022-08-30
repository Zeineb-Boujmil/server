using DataAccess.Interfaces;

namespace DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly MasterDataContext _context;

        public GenericRepository()
        {
            _context = new MasterDataContext();
        }

        public void Create(T entity)
        {
             _context.Set<T>().Add(entity);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
