using Api.Attributes;
using Api.Constants;
using DataAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class ContractController : ODataController
    {
        private readonly MasterDataContext _context = new MasterDataContext();

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public IQueryable<MainContract> Get()
        {
            return _context.MainContracts;
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit)]
        public SingleResult<MainContract> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(_context.MainContracts.Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.OrganizationUnit)]
        public async Task<IHttpActionResult> Post(MainContract contract)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            contract.Id = Guid.NewGuid();

            if (contract.SubContracts != null && contract.SubContracts.Count > 0)
            {
                foreach (var subContract in contract.SubContracts)
                {
                    subContract.Id = Guid.NewGuid();
                    subContract.MainContractId = contract.Id;

                    if (subContract.HandlingCountries != null && subContract.HandlingCountries.Count > 0)
                    {
                        foreach (var country in subContract.HandlingCountries)
                        {
                            country.SubContractId = subContract.Id;
                            _context.HandlingCountries.Add(country);
                        }
                    }

                    _context.SubContracts.Add(subContract);

                }
            }

            _context.MainContracts.Add(contract);


            await _context.SaveChangesAsync();

            return Created(contract);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.OrganizationUnit)]
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, MainContract contract)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (contract.SubContracts != null && contract.SubContracts.Count > 0)
            {
                // Update
                foreach (var subContract in contract.SubContracts.Where(e => e.Id != Guid.Empty))
                {
                    subContract.MainContractId = contract.Id;

                    if (subContract.HandlingCountries != null && subContract.HandlingCountries.Count > 0)
                    {
                        foreach (var country in subContract.HandlingCountries.Where(e => e.Id != Guid.Empty))
                        {
                            // Update
                            country.SubContractId = subContract.Id;
                            _context.Entry(country).State = EntityState.Modified;
                        }
                        foreach (var country in subContract.HandlingCountries.Where(e => e.Id == Guid.Empty))
                        {
                            // Create
                            country.SubContractId = subContract.Id;
                            _context.HandlingCountries.Add(country);
                        }
                    }

                    _context.Entry(subContract).State = EntityState.Modified;
                }

                // Create
                foreach (var subContract in contract.SubContracts.Where(e => e.Id == Guid.Empty))
                {
                    subContract.Id = Guid.NewGuid();
                    subContract.MainContractId = contract.Id;

                    if (subContract.HandlingCountries != null && subContract.HandlingCountries.Count > 0)
                    {
                        foreach (var country in subContract.HandlingCountries)
                        {
                            country.SubContractId = subContract.Id;
                            _context.HandlingCountries.Add(country);
                        }
                    }

                    _context.SubContracts.Add(subContract);
                }
            }

            _context.Entry(contract).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Updated(contract);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}