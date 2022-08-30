using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using CsvHelper.Configuration;
using DataAccess;

namespace Api.Importing
{
    public class CostAgreementImportedFromCsv
    {
        public Guid? Id { get; set; }
        public string AgreementTypeId { get; set; }
        public string AgreementCurrencyCode { get; set; }
        public string AgreementEffectiveDate { get; set; }
        public string AgreementTerminationDate { get; set; }
        public string AgreementIncludingTax { get; set; }
        public string AgreementCode { get; set; }
        public string AgreementName { get; set; }
        public decimal FixedCostAgreementCostAmount { get; set; }

        public static List<CostAgreementImportedFromCsv> FromStream(Stream stream)
        {
            return new CsvReader(new StreamReader(stream), new Configuration { Delimiter = ";", HeaderValidated = null, MissingFieldFound = null })
                .GetRecords<CostAgreementImportedFromCsv>()
                .ToList();
        }

        public bool IsValid(out List<KeyValuePair<string, string>> validationErrors)
        {
            validationErrors = new List<KeyValuePair<string, string>>();

            if (string.IsNullOrWhiteSpace(AgreementTypeId))
                validationErrors.Add(new KeyValuePair<string, string>("AgreementTypeId", "Required."));
            if (string.IsNullOrWhiteSpace(AgreementCurrencyCode))
                validationErrors.Add(new KeyValuePair<string, string>("AgreementCurrencyCode", "Required."));
            if (string.IsNullOrWhiteSpace(AgreementEffectiveDate))
                validationErrors.Add(new KeyValuePair<string, string>("AgreementEffectiveDate", "Required."));
            if (string.IsNullOrWhiteSpace(AgreementTerminationDate))
                validationErrors.Add(new KeyValuePair<string, string>("AgreementTerminationDate", "Required."));
            if (string.IsNullOrWhiteSpace(AgreementIncludingTax))
                validationErrors.Add(new KeyValuePair<string, string>("AgreementIncludingTax", "Required."));
            if (string.IsNullOrWhiteSpace(AgreementCode))
                validationErrors.Add(new KeyValuePair<string, string>("AgreementCode", "Required."));
            if (string.IsNullOrWhiteSpace(AgreementName))
                validationErrors.Add(new KeyValuePair<string, string>("AgreementNames", "Required."));

            return !validationErrors.Any();
        }

        public CostAgreement ToCostAgreement()
        {
            return new CostAgreement
            {
                Id = Id ?? Guid.NewGuid(),
                CostAgreementTypeId = new Guid(AgreementTypeId),
                CurrencyCode = AgreementCurrencyCode,
                EffectiveDate = Convert.ToDateTime(AgreementEffectiveDate),
                TerminationDate = Convert.ToDateTime(AgreementTerminationDate),
                IncludingTax = AgreementIncludingTax == "1",
                AgreementCode = AgreementCode,
                AgreementName = AgreementName,
                FixedCostAgreement = new FixedCostAgreement
                {
                    FixedCostAmount = FixedCostAgreementCostAmount
                }
            };
        }
    }
}