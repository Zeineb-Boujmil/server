using Api.Attributes;
using Api.Constants;
using Api.Controllers.Abstract;
using DataAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{
    public class BankAccountController : BaseController<BankAccount>
    {
        [HttpGet]
       // [Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<BankAccount> Get()
        {
            return Context.Set<BankAccount>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.OrganizationUnit, AuthRoles.Finance)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<BankAccount> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<BankAccount>().Where(e => e.Id == key));
        }

        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Finance)]
        public override async Task<IHttpActionResult> Post(BankAccount entity)
        {
            // TODO: Error handling

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

	        var country = Context.Countries.Single(c => c.Id == entity.CountryCode);

	        if (country.SepaCountry != null)
	        {
		        if (string.IsNullOrEmpty(entity.IBAN))
		        {
			        return BadRequest("IBAN is required for SEPA Countries");
		        }
	        }

	        if (!string.IsNullOrEmpty(entity.IBAN))
	        {
		        var bankAccount = Context.BankAccounts.FirstOrDefault(b => b.IBAN == entity.IBAN);
		        if (bankAccount != null)
		        {
			        return BadRequest($"BankAccount with IBAN {entity.IBAN} already exists");
		        }
	        }

	        if (!string.IsNullOrEmpty(entity.AccountNumber))
	        {
		        var bankAccount = Context.BankAccounts.FirstOrDefault(b =>
			        b.AccountNumber == entity.AccountNumber && b.CountryCode == entity.CountryCode);
		        if (bankAccount != null)
		        {
			        return BadRequest($"BankAccount with AccountNumber {entity.AccountNumber} and country code {entity.CountryCode} already exists");
				}
	        }

            Context.Set<BankAccount>().Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        [HttpPut]
        //[Auth(AuthActionTypes.Update, AuthRoles.Finance)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, BankAccount entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Context.Entry(entity).State = EntityState.Modified;

            await Context.SaveChangesAsync();

            return Updated(entity);
        }
    }
}