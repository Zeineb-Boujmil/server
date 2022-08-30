using System;
using System.Linq;
using System.Web.Http;
using Api.KvkValidatorService;
using DataAccess;

namespace Api.Controllers
{
    [RoutePrefix("api/ChamberOfCommerceNumber")]
    public class ChamberOfCommerceNumberController : ApiController
    {
        [HttpPost]
        [Route("Validate")]
        public bool Validate(OrganizationUnit unit)
        {
            try
            {
                if (string.IsNullOrEmpty(unit.ChamberOfCommerceNumber))
                    return false;

                var address = unit.OrganizationAddresses?.FirstOrDefault(a => a.Address != null)?.Address ?? unit.Address;

                if (address == null)
                    return false;

                var request = new GetCompanyInformationRequest()
                {
                    CountryCode = address.CountryCode
                };

                if (unit.LongName != null)
                    request.Name = unit.LongName;
                else
                {
                    request.City = address.City;
                    request.HouseNumber = address.HouseNo;
                    request.PostalCode = address.PostalCode;
                    request.Street = address.StreetName;
                }

                GetCompanyInformationResponse response;
                try
                {
                    response = new WcfService_Ced_Coc_IntegrationClient().GetCompanyInformation(request);
                }
                catch (TimeoutException ex)
                {
                    throw new ChamberOfCommerceNumberServiceException("Chamber of commerce number service timeout", ex);
                }
                catch (Exception ex)
                {
                    throw new ChamberOfCommerceNumberServiceException("Chamber of commerce number service error", ex);
                }

                return response
                    .Companies?
                    .FirstOrDefault()
                   ?.CoCNumber == unit.ChamberOfCommerceNumber;
            }
            catch (ChamberOfCommerceNumberServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Server error", ex);
            }
        }

        private class ChamberOfCommerceNumberServiceException : Exception
        {
            public ChamberOfCommerceNumberServiceException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
