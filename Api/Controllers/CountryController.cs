using Api.Attributes;
using Api.Constants;
using DataAccess;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class CountryController : ODataController
    {
        private readonly MasterDataContext _context = new MasterDataContext();

        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        //[EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public IQueryable<Country> Get()
        {
            return _context.Countries;
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public SingleResult<Country> Get([FromODataUri] string key)
        {
            return SingleResult.Create(_context.Countries.Where(e => e.Id == key));
        }

        [HttpPost]
       // [Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public async Task<IHttpActionResult> Post(Country country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (country.CurrencyCountries != null && country.CurrencyCountries.Count > 0)
            {
                foreach (var currencyCountry in country.CurrencyCountries)
                {
                    _context.CurrencyCountries.Add(currencyCountry);
                }
            }

            _context.Countries.Add(country);

            await _context.SaveChangesAsync();

            return Created(country);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public async Task<IHttpActionResult> Put([FromODataUri] string key, Country country)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (country.SepaCountry == null)
            {
                var sepaCountry = await _context.SepaCountries.FirstOrDefaultAsync(e => e.Id == key);
                if (sepaCountry != null)
                {
                    _context.SepaCountries.Remove(sepaCountry);
                }
            }
            else
            {
                _context.SepaCountries.AddOrUpdate(country.SepaCountry);
            }

            if (country.CurrencyCountries != null && country.CurrencyCountries.Count > 0)
            {
                var countryCurrenciesIds = country.CurrencyCountries.Select(e => e.Id).ToList();
                var countryCurrenciesForRemove = _context.CurrencyCountries
                    .Where(e => e.CountryId == country.Id)
                    .Where(e => !countryCurrenciesIds.Contains(e.Id));
                _context.CurrencyCountries.RemoveRange(countryCurrenciesForRemove);
                foreach (var currencyCountry in country.CurrencyCountries)
                {
                    _context.CurrencyCountries.AddOrUpdate(currencyCountry);
                }
            }
            else
            {
                var countryCurrenciesForRemove = _context.CurrencyCountries.Where(e => e.CountryId == country.Id);
                _context.CurrencyCountries.RemoveRange(countryCurrenciesForRemove);
            }

            _context.Countries.AddOrUpdate(country);
            await _context.SaveChangesAsync();

            return Updated(country);
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