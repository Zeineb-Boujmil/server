using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Api.Extensions;
using CsvHelper;
using CsvHelper.Configuration;
using DataAccess;

namespace Api.Importing
{
    public class CostSettlementImportedFromCsv
    {
        public Guid? Id { get; set; }
        public string SettlementName { get; set; }
        public Guid? LegalEntityId { get; set; }
        public Guid? SupplierId { get; set; }
        public string LineNumbers { get; set; }
        public string LineServiceIds { get; set; }
        public string AgreementCodes { get; set; }

        public static List<CostSettlementImportedFromCsv> FromStream(Stream stream)
        {
            return new CsvReader(new StreamReader(stream), new Configuration { Delimiter = ";", HeaderValidated = null, MissingFieldFound = null })
                .GetRecords<CostSettlementImportedFromCsv>()
                .ToList();
        }

        public CostSettlement ToCostSettlement(List<CostAgreement> agreements)
        {
            return new CostSettlement
            {
                Id = Id ?? Guid.NewGuid(),
                SettlementName = SettlementName,
                LegalEntityId = LegalEntityId,
                SupplierId = SupplierId,
                CostSettlementLines = !LineNumbers.IsNullOrEmpty()
                    ? LineNumbers
                        .Split('|')
                        .Select((lineNumber, i) => new CostSettlementLine
                        {
                            Id = Guid.NewGuid(),
                            LineNumber = Convert.ToInt32(lineNumber),
                            ServiceId = new Guid(LineServiceIds.Split('|')[i]),
                            CostAgreement = agreements
                                .FirstOrDefault(x => x.AgreementCode == AgreementCodes.Split('|')[i])
                        })
                        .ToList()
                    : new List<CostSettlementLine>()
            };
        }

        public bool IsValid(out List<KeyValuePair<string, string>> validationErrors)
        {
            validationErrors = new List<KeyValuePair<string, string>>();

            if (string.IsNullOrWhiteSpace(SettlementName))
                validationErrors.Add(new KeyValuePair<string, string>("SettlementName", "Required."));
            if (LegalEntityId == null)
                validationErrors.Add(new KeyValuePair<string, string>("LegalEntityId", "Required."));
            if (SupplierId == null)
                validationErrors.Add(new KeyValuePair<string, string>("SupplierId", "Required."));
            if (string.IsNullOrWhiteSpace(LineNumbers))
                validationErrors.Add(new KeyValuePair<string, string>("LineNumbers", "Required."));
            if (string.IsNullOrWhiteSpace(LineServiceIds))
                validationErrors.Add(new KeyValuePair<string, string>("LineServiceIds", "Required."));
            if (string.IsNullOrWhiteSpace(AgreementCodes))
                validationErrors.Add(new KeyValuePair<string, string>("AgreementCodes", "Required."));


            var allFieldsHaveSameAmountOfPipeCharacters = ! new List<int>
            {
                LineNumbers.ToCharArray().Count(c => c == '|'),
                LineServiceIds.ToCharArray().Count(c => c == '|'),
                AgreementCodes.ToCharArray().Count(c => c == '|'),
            }.Distinct()
             .Skip(1)
             .Any();

            if (!allFieldsHaveSameAmountOfPipeCharacters)
                validationErrors.Add(new KeyValuePair<string, string>("SettlementLines", "All settlement line fields must have same number of | characters"));

            return !validationErrors.Any();
        }
    }
}