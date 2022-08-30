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
    public class BrandController : BaseController<Brand>
    {
        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(PageSize = 25, MaxExpansionDepth = 5)]
        public override IQueryable<Brand> Get()
        {
            return Context.Set<Brand>();
        }

        [HttpGet]
        //[Auth(AuthActionTypes.Read, AuthRoles.Administrator)]
        [EnableQuery(MaxExpansionDepth = 5)]
        public override SingleResult<Brand> Get([FromODataUri] Guid key)
        {
            return SingleResult.Create(Context.Set<Brand>().Where(e => e.Id == key));
        }

        //[Auth(AuthActionTypes.Create, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Post(Brand entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            entity.Id = Guid.NewGuid();

            if (entity.Models != null)
            {
                foreach (var model in entity.Models)
                {
                    model.Id = Guid.NewGuid();
                    model.BrandId = entity.Id;

                    Context.Entry(model).State = EntityState.Added;

                    if (model.ModelProducts != null)
                    {
                        foreach (var modelProduct in model.ModelProducts)
                        {
                            modelProduct.Id = Guid.NewGuid();
                            modelProduct.ModelId = model.Id;
                            modelProduct.Product = null;

                            Context.Entry(modelProduct).State = EntityState.Added;
                        }
                    }

                    if (model.ModelSpecifications != null)
                    {
                        foreach (var spec in model.ModelSpecifications)
                        {
                            spec.Id = Guid.NewGuid();
                            spec.ModelId = model.Id;

                            Context.Entry(spec).State = EntityState.Added;

                        }
                    }
                }
            }

            return await base.Post(entity);
        }

        //[Auth(AuthActionTypes.Update, AuthRoles.Administrator)]
        public override async Task<IHttpActionResult> Put([FromODataUri] Guid key, Brand entity)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CRUD(entity.Models,
                Context.Models.AsNoTracking().Where(m => m.BrandId == entity.Id).ToList(),
                model =>
                {
                    if (model.ModelSpecifications != null)
                    {
                        foreach (var spec in model.ModelSpecifications)
                        {
                            spec.ModelId = model.Id;
                            Context.Entry(spec).State = EntityState.Added;
                        }
                    }

                    if (model.ModelProducts != null)
                    {
                        foreach (var modelProduct in model.ModelProducts)
                        {
                            modelProduct.ModelId = model.Id;
                            modelProduct.Product = null;

                            Context.Entry(modelProduct).State = EntityState.Added;
                        }
                    }

                    model.Id = Guid.NewGuid();
                    model.BrandId = entity.Id;
                    Context.Entry(model).State = EntityState.Added;
                },
                model =>
                {
                    CRUD(model.ModelSpecifications,
                        Context.ModelSpecifications.AsNoTracking().Where(ms => ms.ModelId == model.Id).ToList(),
                        spec =>
                        {
                            spec.Id = Guid.NewGuid();
                            spec.ModelId = model.Id;
                            spec.Specification = null;

                            Context.Entry(spec).State = EntityState.Added;
                        },
                        spec =>
                        {
                            spec.ModelId = model.Id;
                            spec.Specification = null;

                            Context.Entry(spec).State = EntityState.Modified;
                        },
                        spec =>
                        {
                            spec.Inactive = true;
                            spec.Specification = null;

                            Context.Entry(spec).State = EntityState.Modified;
                        });

                    CRUD(model.ModelProducts,
                        Context.ModelProducts.AsNoTracking().Where(mp => mp.ModelId == model.Id).ToList(),
                        modelProduct =>
                        {
                            modelProduct.Id = Guid.NewGuid();
                            modelProduct.ModelId = model.Id;
                            modelProduct.Product = null;

                            Context.Entry(modelProduct).State = EntityState.Added;
                        },
                        modelProduct =>
                        {
                            modelProduct.ModelId = model.Id;
                            modelProduct.Product = null;

                            Context.Entry(modelProduct).State = EntityState.Modified;
                        },
                        modelProduct =>
                        {
                            modelProduct.Inactive = true;
                            modelProduct.Product = null;

                            Context.Entry(modelProduct).State = EntityState.Modified;
                        });

                    model.BrandId = entity.Id;
                    Context.Entry(model).State = EntityState.Modified;
                },
                model =>
                {

                    foreach (var spec in Context.ModelSpecifications.Where(spec => spec.ModelId == model.Id).ToList())
                    {
                        spec.Inactive = false;
                    }

                    foreach (var modelProduct in Context.ModelProducts.Where(spec => spec.ModelId == model.Id).ToList())
                    {
                        modelProduct.Inactive = false;
                    }

                    model.Inactive = true;
                    Context.Entry(model).State = EntityState.Modified;

                });

            return await base.Put(key, entity);
        }     
    }
}