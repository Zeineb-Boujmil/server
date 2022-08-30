using System;
using System.Data.Entity;
using System.Linq;

namespace DataAccess
{
    public partial class OrganizationUnit
    {
        public bool IsEqualTo(OrganizationUnit other)
        {
            if (!CompareStrings(ShortName, other.ShortName) ||
                !CompareStrings(LongName, other.LongName) ||
                !CompareStrings(Website, other.Website) ||
                !CompareStrings(VatNumber, other.VatNumber) ||
                !CompareStrings(ChamberOfCommerceNumber, other.ChamberOfCommerceNumber))
                return false;

            if (OrganizationContacts.Count != other.OrganizationContacts.Where(x => x.ContactTypeId != Guid.Parse("05fd006b-20f9-4030-ab92-a27473ec75cd")).ToList().Count)
                return false;

            if (!OrganizationContacts
                    .ToList()
                    .All(first => other.OrganizationContacts.Any(second => CompareStrings(second.ContactValue, first.ContactValue) && CompareStrings(second.Notes, first.Notes))))
                return false;

            if (OrganizationNotes.Count != other.OrganizationNotes.Count)
                return false;

            if (!OrganizationNotes
                .ToList()
                .All(first => other.OrganizationNotes.Any(second => CompareStrings(second.Notes, first.Notes))))
                return false;

            if (OrganizationLabels.Count != other.OrganizationLabels.Count)
                return false;

            if (!OrganizationLabels
                    .ToList()
                    .All(first => other.OrganizationLabels.Any(second => CompareStrings(second.Label, first.Label) && second.OrganizationLabelTypeId == first.OrganizationLabelTypeId)))
                return false;

            if (OrganizationAddresses.Count != other.OrganizationAddresses.Count)
                return false;

            if (!OrganizationAddresses
                .Where(a => a.Address != null)
                .ToList()
                .All(first =>
                    other
                        .OrganizationAddresses
                        .Where(a => a.Address != null)
                        .Any(second => CompareStrings(first.Address.City, second.Address.City)
                                    && CompareStrings(first.Address.CountryCode, second.Address.CountryCode)
                                    && CompareStrings(first.Address.HouseNo, second.Address.HouseNo)
                                    && CompareStrings(first.Address.HouseNoAddition, second.Address.HouseNoAddition)
                                    && CompareStrings(first.Address.PostalCode, second.Address.PostalCode)
                                    && CompareStrings(first.Address.Province, second.Address.Province)
                                    && CompareStrings(first.Address.StreetName, second.Address.StreetName)
                                    && CompareStrings(first.Address.FreeField1, second.Address.FreeField1)
                                    && CompareStrings(first.Address.FreeField2, second.Address.FreeField2)
                                    && CompareStrings(first.Address.FreeField3, second.Address.FreeField3))
                ))
                return false;

            if (!OrganizationAddresses
                .Where(a => a.PostOfficeBox != null)
                .ToList()
                .All(first =>
                    other
                        .OrganizationAddresses
                        .Where(a => a.PostOfficeBox != null)
                        .Any(second => CompareStrings(first.PostOfficeBox.BoxNo, second.PostOfficeBox.BoxNo)
                                    && CompareStrings(first.PostOfficeBox.City, second.PostOfficeBox.City)
                                    && CompareStrings(first.PostOfficeBox.CountryCode, second.PostOfficeBox.CountryCode)
                                    && CompareStrings(first.PostOfficeBox.PostalCode, second.PostOfficeBox.PostalCode)
                                    && CompareStrings(first.PostOfficeBox.Province, second.PostOfficeBox.Province))
                ))
                return false;

            if (Client != null && other.Client != null && Client.CultureCodePreferredLanguage != other.Client.CultureCodePreferredLanguage)
                return false;
            if (Repairer != null && other.Repairer != null && Repairer.CultureCodePreferredLanguage != other.Repairer.CultureCodePreferredLanguage)
                return false;
            if (Partner != null && other.Partner != null && Partner.CultureCodePreferredLanguage != other.Partner.CultureCodePreferredLanguage)
                return false;
            if (Supplier != null && other.Supplier != null && Supplier.CultureCodePreferredLanguage != other.Supplier.CultureCodePreferredLanguage)
                return false;

            return true;
        }

        public bool HasPostOfficeBox()
        {
            return !string.IsNullOrWhiteSpace(PostOfficeBox.BoxNo)
                   || !string.IsNullOrWhiteSpace(PostOfficeBox.PostalCode)
                   || !string.IsNullOrWhiteSpace(PostOfficeBox.City)
                   || !string.IsNullOrWhiteSpace(PostOfficeBox.Province)
                   || !string.IsNullOrWhiteSpace(PostOfficeBox.CountryCode);
        }

        private static bool CompareStrings(string first, string second)
        {
            first = string.IsNullOrEmpty(first) ? null : first;
            second = string.IsNullOrEmpty(second) ? null : second;

            return first == second;
        }
    }
}