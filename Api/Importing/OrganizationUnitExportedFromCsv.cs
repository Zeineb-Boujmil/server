using DataAccess;
using System.Collections.Generic;
using System.Linq;
using Api.Importing;

namespace Api.Exporting
{
    public class OrganizationUnitExportedFromCsv: OrganizationUnitImportExportBaseClass
    {

        public static OrganizationUnitExportedFromCsv FromOrganizationUnit(OrganizationUnit organizationUnit, List<ContactType> contactTypes)
        {
            var organizationAddresses = organizationUnit.OrganizationAddresses.Where(oa => oa.Address != null).ToList();
            var organizationPostOfficeBoxes = organizationUnit.OrganizationAddresses.Where(oa => oa.PostOfficeBox != null).ToList();

            var phoneContacts = organizationUnit.OrganizationContacts.Where(oc => oc.ContactType != null && oc.ContactType.IsLandline()).ToList();
            var mobileContacts = organizationUnit.OrganizationContacts.Where(oc => oc.ContactType != null && oc.ContactType.IsMobile()).ToList();
            var faxContacts = organizationUnit.OrganizationContacts.Where(oc => oc.ContactType != null && oc.ContactType.IsFax()).ToList();
            var emailContacts = organizationUnit.OrganizationContacts.Where(oc => oc.ContactType != null && oc.ContactType.IsEmail()).ToList();

            return new OrganizationUnitExportedFromCsv()
            {
                Id = organizationUnit.Id.ToString(),
                RelationCode = !string.IsNullOrEmpty(organizationUnit.Code) ? organizationUnit.Code : organizationUnit.OrganizationCodes.FirstOrDefault()?.Code,
                ShortName = organizationUnit.ShortName,
                LongName = organizationUnit.LongName,
                Address_TypeCode = string.Join("|", organizationAddresses.Select(oa => oa.AddressType.Code)),
                Address_StreetName = string.Join("|", organizationAddresses.Select(oa => oa.Address.StreetName)),
                Address_HouseNo = string.Join("|", organizationAddresses.Select(oa => oa.Address.HouseNo)),
                Address_HouseNoAddition = string.Join("|", organizationAddresses.Select(oa => oa.Address.HouseNoAddition)),
                Address_PostalCode = string.Join("|", organizationAddresses.Select(oa => oa.Address.PostalCode)),
                Address_City = string.Join("|", organizationAddresses.Select(oa => oa.Address.City)),
                Address_Province = string.Join("|", organizationAddresses.Select(oa => oa.Address.Province)),
                Address_CountryCode = string.Join("|", organizationAddresses.Select(oa => oa.Address.CountryCode)),

                Address_FreeField1 = string.Join("|", organizationAddresses.Select(oa => oa.Address.FreeField1)),
                Address_FreeField2 = string.Join("|", organizationAddresses.Select(oa => oa.Address.FreeField2)),
                Address_FreeField3 = string.Join("|", organizationAddresses.Select(oa => oa.Address.FreeField3)),

                Contact_landline_value = string.Join("|", phoneContacts.Select(pc => pc.ContactValue)),
                Contact_landline_label = string.Join("|", phoneContacts.Select(pc => pc.Notes)),

                Contact_mobile_value = string.Join("|", mobileContacts.Select(pc => pc.ContactValue)),
                Contact_mobile_label = string.Join("|", mobileContacts.Select(pc => pc.Notes)),

                Contact_fax_value = string.Join("|", faxContacts.Select(pc => pc.ContactValue)),
                Contact_fax_label = string.Join("|", faxContacts.Select(pc => pc.Notes)),

                Contact_email_value = string.Join("|", emailContacts.Select(pc => pc.ContactValue)),
                Contact_email_label = string.Join("|", emailContacts.Select(pc => pc.Notes)),

                Label = string.Join("|", organizationUnit.OrganizationLabels.Select(l => l.Label)),
                LabelTypeCode = string.Join("|", organizationUnit.OrganizationLabels.Select(l => l.OrganizationLabelType.Code)),

                Notes = string.Join("|", organizationUnit.OrganizationNotes.Select(n => n.Notes)),
                Preferred_Language = organizationUnit.Supplier?.CultureCodePreferredLanguage,

                Website = organizationUnit.Website,
                PostOfficeBox_BoxNo = string.Join("|", organizationPostOfficeBoxes.Select(pob => pob.PostOfficeBox.BoxNo)),
                PostOfficeBox_PostalCode = string.Join("|", organizationPostOfficeBoxes.Select(pob => pob.PostOfficeBox.PostalCode)),
                PostOfficeBox_City = string.Join("|", organizationPostOfficeBoxes.Select(pob => pob.PostOfficeBox.City)),
                PostOfficeBox_Provice = string.Join("|", organizationPostOfficeBoxes.Select(pob => pob.PostOfficeBox.Province)),
                PostOfficeBox_CountryCode = string.Join("|", organizationPostOfficeBoxes.Select(pob => pob.PostOfficeBox.CountryCode)),

                VatNumber = organizationUnit.VatNumber,
                ChamberOfCommerceNumber = organizationUnit.ChamberOfCommerceNumber
            };
        }
    }
}