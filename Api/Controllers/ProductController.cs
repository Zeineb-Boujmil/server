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
    public class ProductController : BaseController<Product>
    {
        public Product Entity { get; set; }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override IQueryable<Product> Get()
        {
            return Context.Set<Product>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<Product> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<Product>().Where(e => e.Id == key));
        }

        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(Product entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            entity.Id = Guid.NewGuid();

            foreach (var activity in entity.ProductWorkActivities)
            {
                activity.ProductId = entity.Id;
                Context.ProductWorkActivities.Add(activity);

                if (activity.WorkActivity != null)
                    Context.Entry(activity.WorkActivity).State = EntityState.Detached;
            }

            foreach (var insuranceObject in entity.ProductInsuranceObjects)
            {
                insuranceObject.ProductId = entity.Id;
                Context.ProductInsuranceObjects.Add(insuranceObject);

                if (insuranceObject.InsuranceObject != null)
                    Context.Entry(insuranceObject.InsuranceObject).State = EntityState.Detached;
            }

            foreach (var insuranceCoverage in entity.ProductInsuranceCoverages)
            {
                insuranceCoverage.ProductId = entity.Id;
                Context.ProductInsuranceCoverages.Add(insuranceCoverage);

                if (insuranceCoverage.InsuranceCoverage != null)
                    Context.Entry(insuranceCoverage.InsuranceCoverage).State = EntityState.Detached;
            }

            foreach (var damageReason in entity.ProductDamageReasons)
            {
                damageReason.ProductId = entity.Id;
                Context.ProductDamageReasons.Add(damageReason);

                if (damageReason.DamageReason != null)
                    Context.Entry(damageReason.DamageReason).State = EntityState.Detached;
            }

            foreach (var application in entity.ProductApplications)
            {
                application.ProductId = entity.Id;
                Context.ProductApplications.Add(application);

                if (application.Application != null)
                    Context.Entry(application.Application).State = EntityState.Detached;
            }

            foreach (var exclusion in entity.ProductExclusions)
            {
                exclusion.ProductId = entity.Id;
                Context.Entry(exclusion).State = EntityState.Added;
            }

            Context.Products.Add(entity);
            await Context.SaveChangesAsync();

            return Created(entity);
        }

        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put(Guid key, Product entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Entity = entity;

            var isProductClassificationValid = await IsProductClassificationValid();
            if (!isProductClassificationValid)
            {
                return BadRequest("The selected product classification is invalid.");
            }

            await UpdateActivities();

            await UpdateInsuranceObjects();

            await UpdateInsuranceCoverages();

            await UpdateDamageReasons();

            await UpdateApplications();

            await UpdateExclusions();

            Context.Entry(entity).State = EntityState.Modified;

            await Context.SaveChangesAsync();

            return Updated(entity);
        }

        private async Task<bool> IsProductClassificationValid()
        {
            var isValid = true;
            var productClassification = await Context.ProductClassifications.FirstOrDefaultAsync(p => p.Id == Entity.ProductClassification_Id);
            var hasServices = await Context.Services.AnyAsync(s => !s.Inactive && s.ProductId == Entity.Id);
            if ((productClassification?.Code == "SERVICE" && !hasServices)
             || (productClassification?.Code != "SERVICE" && hasServices))
            {
                isValid = false;
            }
            return isValid;
        }

        private async Task UpdateExclusions()
        {
            CRUD(Entity.ProductExclusions,
                await Context.ProductExclusions.AsNoTracking().Where(ex => ex.ProductId == Entity.Id).ToListAsync(),
                exclusion =>
                {
                    exclusion.Id = Guid.NewGuid();
                    exclusion.ProductId = Entity.Id;

                    Context.Entry(exclusion).State = EntityState.Added;
                },
                exclusion =>
                {
                    exclusion.ProductId = Entity.Id;

                    Context.Entry(exclusion).State = EntityState.Modified;
                },
                exclusion =>
                {
                    exclusion.Inactive = true;

                    Context.Entry(exclusion).State = EntityState.Modified;
                });
        }

        private async Task UpdateApplications()
        {
            CRUD(newEntities: Entity.ProductApplications,
                 oldEntities: await Context.ProductApplications.AsNoTracking().Where(ex => ex.ProductId == Entity.Id).ToListAsync(),
                 create: newProductApplication =>
                 {
                     newProductApplication.Id = Guid.NewGuid();
                     newProductApplication.ProductId = Entity.Id;
                     Context.Entry(newProductApplication).State = EntityState.Added;
                     if (newProductApplication.Application != null)
                         Context.Entry(newProductApplication.Application).State = EntityState.Detached;
                 },
                 update: newProductApplication =>
                 {
                     Context.ProductApplications.Attach(newProductApplication);
                     Context.Entry(newProductApplication).State = EntityState.Modified;
                     if (newProductApplication.Application != null)
                         Context.Entry(newProductApplication.Application).State = EntityState.Detached;
                 },
                 delete: oldProductApplication =>
                 {
                     Context.ProductApplications.Attach(oldProductApplication);
                     oldProductApplication.Inactive = true;
                     Context.Entry(oldProductApplication).State = EntityState.Modified;
                 });
        }

        private async Task UpdateDamageReasons()
        {
            var oldDamageReasons = await Context.ProductDamageReasons.AsNoTracking().Where(e => e.ProductId == Entity.Id).ToListAsync();
            var newDamageReasons = Entity.ProductDamageReasons;

            foreach (var damageReason in newDamageReasons.Where(n => n.Id == Guid.Empty).ToList())
            {
                damageReason.ProductId = Entity.Id;
                Context.ProductDamageReasons.Add(damageReason);

                if (damageReason.DamageReason != null)
                {
                    Context.Entry(damageReason.DamageReason).State = EntityState.Detached;
                }
            }

            foreach (var damageReason in oldDamageReasons.Where(o => newDamageReasons.All(n => n.Id != o.Id)).ToList())
            {
                Context.ProductDamageReasons.Attach(damageReason);
                Context.ProductDamageReasons.Remove(damageReason);
            }
        }

        private async Task UpdateInsuranceCoverages()
        {
            var oldInsuranceCoverages = await Context.ProductInsuranceCoverages.AsNoTracking().Where(e => e.ProductId == Entity.Id).ToListAsync();
            var newInsuranceCoverages = Entity.ProductInsuranceCoverages;

            foreach (var insuranceCoverage in newInsuranceCoverages.Where(n => n.Id == Guid.Empty).ToList())
            {
                insuranceCoverage.ProductId = Entity.Id;
                Context.ProductInsuranceCoverages.Add(insuranceCoverage);

                if (insuranceCoverage.InsuranceCoverage != null)
                {
                    Context.Entry(insuranceCoverage.InsuranceCoverage).State = EntityState.Detached;
                }
            }

            foreach (var insuranceCoverage in oldInsuranceCoverages.Where(o => newInsuranceCoverages.All(n => n.Id != o.Id)).ToList())
            {
                Context.ProductInsuranceCoverages.Attach(insuranceCoverage);
                Context.ProductInsuranceCoverages.Remove(insuranceCoverage);
            }
        }

        private async Task UpdateInsuranceObjects()
        {
            var oldInsuranceObject = await Context.ProductInsuranceObjects.AsNoTracking().Where(e => e.ProductId == Entity.Id).ToListAsync();
            var newInsuranceObject = Entity.ProductInsuranceObjects;

            foreach (var insuranceObject in newInsuranceObject.Where(n => n.Id == Guid.Empty).ToList())
            {
                insuranceObject.ProductId = Entity.Id;
                Context.ProductInsuranceObjects.Add(insuranceObject);

                if (insuranceObject.InsuranceObject != null)
                {
                    Context.Entry(insuranceObject.InsuranceObject).State = EntityState.Detached;
                }
            }

            foreach (var insuranceObject in oldInsuranceObject.Where(o => newInsuranceObject.All(n => n.Id != o.Id)).ToList())
            {
                Context.ProductInsuranceObjects.Attach(insuranceObject);
                Context.ProductInsuranceObjects.Remove(insuranceObject);
            }
        }

        private async Task UpdateActivities()
        {
            var oldActivities = await Context.ProductWorkActivities.AsNoTracking().Where(e => e.ProductId == Entity.Id).ToListAsync();
            var newActivities = Entity.ProductWorkActivities;

            foreach (var activity in newActivities.Where(n => n.Id == Guid.Empty).ToList())
            {
                activity.ProductId = Entity.Id;
                Context.ProductWorkActivities.Add(activity);

                if (activity.WorkActivity != null)
                {
                    Context.Entry(activity.WorkActivity).State = EntityState.Detached;
                }
            }

            foreach (var activity in oldActivities.Where(o => newActivities.All(n => n.Id != o.Id)).ToList())
            {
                Context.ProductWorkActivities.Attach(activity);
                Context.ProductWorkActivities.Remove(activity);
            }
        }
    }
}