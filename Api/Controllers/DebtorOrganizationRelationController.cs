using Api.Attributes;
using Api.Constants;
using Api.Messages;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Api.Controllers
{
    public class DebtorOrganizationRelationController : ApiController
    {
        private readonly MasterDataContext _context;

        public DebtorOrganizationRelationController()
        {
            _context = new MasterDataContext();
        }

        [Route("api/debtororganizationrelationproducts")]
        [HttpPost]
        //[Auth(AuthActionTypes.Create, AuthRoles.Finance, AuthRoles.OrganizationUnit)]
        public IHttpActionResult CreateOrUpdateDebtorOrganizationRelationProducts([FromBody] DebtorOrganizationRelationProductEntry debtorOrganizationRelationProductEntry)
        {
            if (debtorOrganizationRelationProductEntry == null)
            {
                return BadRequest();
            }

            try
            {
                var debtorId = debtorOrganizationRelationProductEntry.DebtorId;
                var clientProducts = debtorOrganizationRelationProductEntry.ClientProducts ?? new List<ClientProductsEntry>();
                foreach (var entry in clientProducts)
                {
                    var productIds = entry.ProductIds ?? new List<Guid>();
                    var debtorOrganizationRelations = _context.DebtorOrganizationRelations.Where(d => d.OrganizationUnitId == entry.ClientId && d.DebtorId == debtorId).ToList();

                    if (!debtorOrganizationRelations.Any())
                    {
                        var debtorOrganizationRelation = CreateDebtorOrganizationRelation(debtorId, entry.ClientId, null);
                        debtorOrganizationRelations.Add(debtorOrganizationRelation);
                    }

                    if (!productIds.Any())
                    {
                        // this means that all products are allowed, instead of specific ones --> set the product id to null
                        var debtorOrganizationRelation = debtorOrganizationRelations.First();
                        debtorOrganizationRelation.Product_Id = null;

                        //remove the remaining relations, if any
                        debtorOrganizationRelations.RemoveRange(0, 1);
                        debtorOrganizationRelations.ForEach(d => _context.DebtorOrganizationRelations.Remove(d));
                    }
                    else
                    {
                        foreach (var productId in productIds.Distinct())
                        {
                            CreateOrUpdateDebtorOrganizationRelationProduct(debtorOrganizationRelations, productId, debtorId, entry.ClientId);
                        }

                        //remove debtor organization relation for which the productid is not present in the request
                        var removableDebtorOrganizationRelations = debtorOrganizationRelations.Where(d => d.Product_Id != null && !productIds.Contains(d.Product_Id.Value)).ToList();
                        removableDebtorOrganizationRelations.ForEach(d => _context.DebtorOrganizationRelations.Remove(d));
                    }
                }
                _context.SaveChanges();
                return Ok();
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        private void CreateOrUpdateDebtorOrganizationRelationProduct(List<DebtorOrganizationRelation> debtorOrganizationRelations, Guid productId, Guid debtorId, Guid clientId)
        {
            var debtorOrganizationRelation = debtorOrganizationRelations.FirstOrDefault(d => d.Product_Id == productId);
            if (debtorOrganizationRelation == null)
            {
                debtorOrganizationRelation = debtorOrganizationRelations.FirstOrDefault(d => d.Product_Id == null);
            }
            if (debtorOrganizationRelation == null)
            {
                CreateDebtorOrganizationRelation(debtorId, clientId, productId);
            }
            else
            {
                debtorOrganizationRelation.Product_Id = productId;
            }
        }

        private DebtorOrganizationRelation CreateDebtorOrganizationRelation(Guid debtorId, Guid clientId, Guid? productId)
        {
            var debtorOrganizationRelation = new DebtorOrganizationRelation
            {
                DebtorId = debtorId,
                OrganizationUnitId = clientId,
                Product_Id = productId,
                EffectiveDate = DateTime.UtcNow,
                Id = Guid.NewGuid()
            };
            _context.DebtorOrganizationRelations.Add(debtorOrganizationRelation);
            return debtorOrganizationRelation;
        }
    }
}