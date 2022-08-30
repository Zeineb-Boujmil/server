using System.Linq;
using CED.Framework.Interfaces;

namespace DataAccess.Repositories
{
    public class PingRepository : IPingRepository
    {
        private readonly MasterDataContext _context;

        public PingRepository()
        {
            _context = new MasterDataContext();
        }

        public void PingDatabase()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            _context.AddressTypes.ToList();
        }
    }
}