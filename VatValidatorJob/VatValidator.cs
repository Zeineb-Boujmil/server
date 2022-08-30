using System;
using System.Data.Entity;
using System.Linq;
using DataAccess;
using VatValidatorJob.VatNumberCheckService;

namespace VatValidatorJob
{
    public class VatValidator
    {
        private readonly WcfServiceCedVatNumberCheckClient _checkVatService;
        protected readonly MasterDataContext Context = new MasterDataContext();

        public VatValidator(WcfServiceCedVatNumberCheckClient checkVatService)
        {
            _checkVatService = checkVatService;
        }

        public void CheckAllClientsVat()
        {
            var previousJobRunTime = Context
                .OrganizationVatValidations
                .Where(v => v.EmployeeId != null)
                .OrderByDescending(v => v.CreatedDate)
                .FirstOrDefault()
                ?.CreatedDate ?? DateTime.MinValue;

            var clientsModifiedAfterPreviousJobRun = Context
                .OrganizationUnits
                .Include(o => o.OrganizationAddresses)
                .Where(o => o.Client != null && o.VatNumber != null)
                .ToList()
                .Where(o => o.LastModifiedDate > previousJobRunTime || o.OrganizationAddresses.Any(a => a.LastModifiedDate > previousJobRunTime));

            foreach (var client in clientsModifiedAfterPreviousJobRun)
            {
                bool isVatValid;
                if (client.VatNumber.Length < 8)
                    isVatValid = false;
                else
                {
                    var response = _checkVatService.CheckVatNumber(new CheckVatNumberRequest { VatNumber = client.VatNumber.Substring(2), Iso2CountryCode = client.VatNumber.Substring(0, 2) });
                    isVatValid = response.Result != null && 
                                 response.Result.IsValid && 
                                 response.Result.CompanyName == client.LongName;
                }
                client.OrganizationVatValidations.Add(new OrganizationVatValidation { Id = Guid.NewGuid(), IsValid = isVatValid });
            }

            Context.SaveChanges();
        }
    }
}