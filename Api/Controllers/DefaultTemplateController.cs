using Api.Messages;
using DataAccess;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using System.Web.OData;

namespace Api.Controllers
{

    [Route("api/DefaultTemplate")]
    public class DefaultTemplateController : ApiController
    {
        private readonly MasterDataContext _context;
        public DefaultTemplateController()
        {
            _context = new MasterDataContext();
            _context.Configuration.ProxyCreationEnabled = false;
        }
        [HttpPost]
        public IHttpActionResult CreateOrUpdateDefaultTemplate(DefaultTemplateEntry defaultTemplateEntry)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            DefaultTemplate defaultTemplate = new DefaultTemplate
            {
                Id = defaultTemplateEntry.Id != null ? (Guid)defaultTemplateEntry.Id : Guid.Empty,
                BodyEmail = defaultTemplateEntry.EmailBody.ToString(),
                Department_Id = defaultTemplateEntry.Department.Id,
                Template_Id = defaultTemplateEntry.DefaultTemplate,
                EffectiveDate = defaultTemplateEntry.StartDate,
                TerminationDate = defaultTemplateEntry.EndDate
            };

            if (defaultTemplate.Template_Id != Guid.Empty && defaultTemplate.Department_Id != Guid.Empty)
            {
                if (IsOverLap(defaultTemplate))
                {
                    return BadRequest("An existing period overlap");
                }

                if (defaultTemplate.Id == Guid.Empty)
                {
                    _context.Entry(defaultTemplate).State = EntityState.Added;
                }
                else
                {

                    _context.Entry(defaultTemplate).State = EntityState.Modified;
                }

                _context.SaveChanges();
                return Ok(defaultTemplate.Id);
            }

            return BadRequest(ModelState);
        }

        private bool IsOverLap(DefaultTemplate defaultTemplate)
        {
            return _context.DefaultTemplates.Where(defaultTemplateDb => defaultTemplateDb.Department_Id == defaultTemplate.Department_Id
                                                                        && defaultTemplateDb.Id != defaultTemplate.Id).Any
                        (x => DbFunctions.TruncateTime(defaultTemplate.EffectiveDate) <= DbFunctions.TruncateTime(x.TerminationDate)
                        && DbFunctions.TruncateTime(x.EffectiveDate) <= DbFunctions.TruncateTime(defaultTemplate.TerminationDate));
        }

        //[HttpGet]
        //public IHttpActionResult Get()
        //{
        //    return Ok(
        //        _context.DefaultTemplates
        //            .Include("Department.BusinessUnit.LegalEntity")
        //            .Include(x => x.Template)
        //            .Select(defaultTemplate =>
        //                new DefaultTemplateViewModel
        //                {
        //                    Id = defaultTemplate.Id,
        //                    EmailBody = defaultTemplate.BodyEmail,
        //                    LegalEntity = _context.OrganizationUnits.FirstOrDefault(d =>
        //                        d.Id == defaultTemplate.Department.BusinessUnit.LegalEntityId).Id,
        //                    BusinessUnit = _context.OrganizationUnits
        //                        .FirstOrDefault(d => d.Id == defaultTemplate.Department.BusinessUnit.Id).Id,
        //                    Department = _context.OrganizationUnits
        //                        .FirstOrDefault(d => d.Id == defaultTemplate.Department_Id).Id,
        //                    StartDate = defaultTemplate.EffectiveDate,
        //                    EndDate = defaultTemplate.TerminationDate,
        //                    DefaultTemplate = defaultTemplate.Template.Id
        //                }));
        //}


        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok(
                _context.DefaultTemplates
                    .Include("Department.BusinessUnit.LegalEntity")
                    .Include(x => x.Template)
                    .Select(defaultTemplate =>
                        new DefaultTemplateViewModel
                        {
                            Id = defaultTemplate.Id,
                            EmailBody = defaultTemplate.BodyEmail,


                            LegalEntity = _context.OrganizationUnits.FirstOrDefault(d =>
                                d.Id == defaultTemplate.Department.BusinessUnit.LegalEntityId).Id,
                            LegalEntityName ="", 
                            LegalEntityLongName = _context.OrganizationUnits.FirstOrDefault(d =>
                                d.Id == defaultTemplate.Department.BusinessUnit.LegalEntityId).LongName,
                            LegalEntityLongName2 = _context.OrganizationUnits.FirstOrDefault(d =>
                               d.Id == defaultTemplate.Department.BusinessUnit.LegalEntityId).LongName2,
                            LegalEntityCode = _context.OrganizationUnits.FirstOrDefault(d =>
                                d.Id == defaultTemplate.Department.BusinessUnit.LegalEntityId).Code,


                            BusinessUnit = _context.OrganizationUnits
                                .FirstOrDefault(d => d.Id == defaultTemplate.Department.BusinessUnit.Id).Id,
                            BusinessUnitName = "",
                            BusinessUnitLongName = _context.OrganizationUnits
                                .FirstOrDefault(d => d.Id == defaultTemplate.Department.BusinessUnit.Id).LongName,
                            BusinessUnitLongName2 = _context.OrganizationUnits
                                .FirstOrDefault(d => d.Id == defaultTemplate.Department.BusinessUnit.Id).LongName,
                            BusinessUnitCode = _context.OrganizationUnits
                                .FirstOrDefault(d => d.Id == defaultTemplate.Department.BusinessUnit.Id).Code,


                            Department = _context.OrganizationUnits
                                .FirstOrDefault(d => d.Id == defaultTemplate.Department_Id).Id,
                            DepartmentName ="",
                            DepartmentLongName = _context.OrganizationUnits
                                .FirstOrDefault(d => d.Id == defaultTemplate.Department_Id).LongName,
                            DepartmentLongName2 = _context.OrganizationUnits
                                .FirstOrDefault(d => d.Id == defaultTemplate.Department_Id).LongName2,
                            DepartmentCode= _context.OrganizationUnits
                                .FirstOrDefault(d => d.Id == defaultTemplate.Department_Id).Code,


                            StartDate = defaultTemplate.EffectiveDate,
                            EndDate = defaultTemplate.TerminationDate,
                            DefaultTemplate = defaultTemplate.Template.Id,
                            DefaultTemplateName = defaultTemplate.Template.TemplateName,



                        }).OrderBy(y => y.LegalEntity).ThenBy(x => x.BusinessUnit).ThenBy(z => z.Department).ThenBy(s => s.DefaultTemplate));
        }



        //[HttpGet]
        //public IHttpActionResult Get()
        //{
        //    return Ok(
        //        _context.DefaultTemplates
        //            .Include("Department.BusinessUnit.LegalEntity")
        //            .Include(x => x.Template)
        //            .Select(defaultTemplate =>
        //                new DefaultTemplateViewModel
        //                {
        //                    Id = defaultTemplate.Id,
        //                    EmailBody = defaultTemplate.BodyEmail,
        //                    LegalEntity = _context.OrganizationUnits.FirstOrDefault(d =>
        //                        d.Id == defaultTemplate.Department.BusinessUnit.LegalEntityId).Id,
        //                    BusinessUnit = _context.OrganizationUnits
        //                        .FirstOrDefault(d => d.Id == defaultTemplate.Department.BusinessUnit.Id).Id,
        //                    Department = _context.OrganizationUnits
        //                        .FirstOrDefault(d => d.Id == defaultTemplate.Department_Id).Id,
        //                    StartDate = defaultTemplate.EffectiveDate,
        //                    EndDate = defaultTemplate.TerminationDate,
        //                    DefaultTemplate = defaultTemplate.Template.Id
        //                }));
        //}

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