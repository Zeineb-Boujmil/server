using Api.Messages;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Api.Controllers
{
    [Route("api/DebtorDefaultTemplate")]
    public class DebtorDefaultTemplateController : ApiController
    {
        MasterDataContext _context;
        public DebtorDefaultTemplateController()
        {
            _context = new MasterDataContext();
            _context.Configuration.ProxyCreationEnabled = false;
        }

        [HttpPost]
        public IHttpActionResult CreateOrUpdateDebtorDefaultTemplate(DebtorTemplate debtorTemplate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else
            {
                if (_context.DebtorTemplates.Where(debtorTemplateDB => ( debtorTemplate.Id != debtorTemplateDB.Id ) && ( debtorTemplateDB.Debtor_Id == debtorTemplate.Debtor_Id
          && debtorTemplateDB.BusinessUnit_Id == debtorTemplate.BusinessUnit_Id && debtorTemplateDB.Department_Id == debtorTemplate.Department_Id)).Any
      (x => DbFunctions.TruncateTime(debtorTemplate.EffectiveDate) <= DbFunctions.TruncateTime(x.TerminationDate)
      && DbFunctions.TruncateTime(x.EffectiveDate) <= DbFunctions.TruncateTime(debtorTemplate.TerminationDate)))
                {
                    return BadRequest("An existing period overlap");
                }
                else
                {
                    if (debtorTemplate.Id == Guid.Empty)
                    {
                        _context.Entry(debtorTemplate).State = EntityState.Added;
                    }
                    else
                    {
                        _context.Entry(debtorTemplate).State = EntityState.Modified;
                    }

                    _context.SaveChanges();
                    return Ok(debtorTemplate.Id);
                }
            }




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