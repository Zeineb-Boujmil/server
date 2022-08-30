using System;
using System.Collections.Generic;
using System.Linq;
using Api.Extensions;
using Api.ViewModels;
using DataAccess;

namespace Api.Importing
{
    public class OrganizationUnitImportedFromCSV: OrganizationUnitImportExportBaseClass
    {
        public string InternationalAssistanceGroup_TypeCode { get; set; }

        public bool IsValid(out List<KeyValuePair<string, string>> validationErrors, List<OrganizationUnit> existingOrganizationUnits, List<AddressType> addressTypes, List<OrganizationLabelType> labelTypes, List<InternationalAssistanceGroupType> internationalAssistanceGroupTypes)
        {
            validationErrors = new List<KeyValuePair<string, string>>();

            if (!string.IsNullOrWhiteSpace(Id) && existingOrganizationUnits.All(ou => ou.Id != Guid.Parse(Id)))
                validationErrors.Add(new KeyValuePair<string, string>("Id", "No matching Id in database."));

            if (!string.IsNullOrWhiteSpace(RelationCode) && RelationCode.Length > 20)
                validationErrors.Add(new KeyValuePair<string, string>("RelationCode", "Cannot be larger than 20 characters."));

            if (!string.IsNullOrWhiteSpace(ShortName) && ShortName.Length > 20)
                validationErrors.Add(new KeyValuePair<string, string>("ShortName", "Cannot be larger than 20 characters."));

            if (string.IsNullOrWhiteSpace(LongName))
                validationErrors.Add(new KeyValuePair<string, string>("LongName", "Required."));
            else if (LongName.Length > 250)
                validationErrors.Add(new KeyValuePair<string, string>("LongName", "Cannot be larger than 250 characters."));

            if (!string.IsNullOrWhiteSpace(Website) && Website.Length > 250)
                validationErrors.Add(new KeyValuePair<string, string>("Website", "Cannot be larger than 250 characters."));

            if (!string.IsNullOrWhiteSpace(VatNumber) && VatNumber.Length > 50)
                validationErrors.Add(new KeyValuePair<string, string>("VatNumber", "Cannot be larger than 50 characters."));

            if (!string.IsNullOrWhiteSpace(ChamberOfCommerceNumber) && ChamberOfCommerceNumber.Length > 50)
                validationErrors.Add(new KeyValuePair<string, string>("ChamberOfCommerceNumber", "Cannot be larger than 50 characters."));

            validationErrors.AddRange(
                Contact_landline_value
                    .Split('|')
                    .Where(v => !string.IsNullOrWhiteSpace(v) && v.Length > 20)
                    .Select(v => new KeyValuePair<string, string>("Contact_landline_value", "Cannot be larger than 20 characters.")));

            validationErrors.AddRange(
                Contact_mobile_value
                    .Split('|')
                    .Where(v => !string.IsNullOrWhiteSpace(v) && v.Length > 20)
                    .Select(v => new KeyValuePair<string, string>("Contact_mobile_value", "Cannot be larger than 20 characters.")));

            validationErrors.AddRange(
                Contact_email_value
                    .Split('|')
                    .Where(v => !string.IsNullOrWhiteSpace(v) && v.Length > 250)
                    .Select(v => new KeyValuePair<string, string>("Contact_email_value", "Cannot be larger than 250 characters.")));

            validationErrors.AddRange(
                Address_StreetName
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 250)
                    .Select(v => new KeyValuePair<string, string>("Address_StreetName", "Cannot be larger than 250 characters.")));

            validationErrors.AddRange(
                Address_HouseNo
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 10)
                    .Select(v => new KeyValuePair<string, string>("Address_HouseNo", "Cannot be larger than 10 characters.")));

            validationErrors.AddRange(
                Address_HouseNoAddition
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 250)
                    .Select(v => new KeyValuePair<string, string>("Address_HouseNoAddition", "Cannot be larger than 250 characters.")));

            validationErrors.AddRange(
                Address_PostalCode
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 10)
                    .Select(v => new KeyValuePair<string, string>("Address_PostalCode", "Cannot be larger than 10 characters.")));

            validationErrors.AddRange(
                Address_City
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 250)
                    .Select(v => new KeyValuePair<string, string>("Address_City", "Cannot be larger than 250 characters.")));

            validationErrors.AddRange(
                Address_Province
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 250)
                    .Select(v => new KeyValuePair<string, string>("Address_Province", "Cannot be larger than 250 characters.")));

            validationErrors.AddRange(
                Address_CountryCode
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 3)
                    .Select(v => new KeyValuePair<string, string>("Address_CountryCode", "Cannot be larger than 3 characters.")));

            validationErrors.AddRange(
                PostOfficeBox_BoxNo
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 10)
                    .Select(v => new KeyValuePair<string, string>("PostOfficeBox_BoxNo", "Cannot be larger than 10 characters.")));

            validationErrors.AddRange(
                PostOfficeBox_PostalCode
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 10)
                    .Select(v => new KeyValuePair<string, string>("PostOfficeBox_PostalCode", "Cannot be larger than 10 characters.")));

            validationErrors.AddRange(
                PostOfficeBox_City
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 250)
                    .Select(v => new KeyValuePair<string, string>("PostOfficeBox_City", "Cannot be larger than 250 characters.")));

            validationErrors.AddRange(
                PostOfficeBox_Provice
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 250)
                    .Select(v => new KeyValuePair<string, string>("PostOfficeBox_Provice", "Cannot be larger than 250 characters.")));

            validationErrors.AddRange(
                PostOfficeBox_CountryCode
                    .Split('|')
                    .Where(a => !string.IsNullOrWhiteSpace(a) && a.Length > 3)
                    .Select(v => new KeyValuePair<string, string>("PostOfficeBox_CountryCode", "Cannot be larger than 3 characters.")));

            validationErrors.AddRange(
                Address_TypeCode
                    .Split('|')
                    .Where(a => addressTypes.All(a1 => !string.IsNullOrWhiteSpace(a) && a1.Code != a))
                    .Select(v => new KeyValuePair<string, string>("Address_TypeCode", v + " is invalid adddress type code")));
            validationErrors.AddRange(
                LabelTypeCode
                    .Split('|')
                    .Where(a => labelTypes.All(a1 => !string.IsNullOrWhiteSpace(a) && a1.Code != a))
                    .Select(v => new KeyValuePair<string, string>("LabelTypeCode", v + " is invalid Label Type Code")));
            if(!string.IsNullOrEmpty(InternationalAssistanceGroup_TypeCode))
                validationErrors.AddRange(
                    InternationalAssistanceGroup_TypeCode
                        .Split('|')
                        .Where(a => internationalAssistanceGroupTypes.All(a1 => !string.IsNullOrWhiteSpace(a) && a1.Code != a))
                        .Select(v => new KeyValuePair<string, string>("InternationalAssistanceGroup_TypeCode", v + " is invalid International Assistance Group Type Code")));

            return !validationErrors.Any();
        }

        public OrganizationUnit ToOrganizationUnit(UploadConfiguration config, List<ContactType> contactTypes, List<AddressType> addressTypes, List<OrganizationLabelType> labelTypes, List<InternationalAssistanceGroupType> internationalAssistanceGroupTypes, List<Application> applications)
        {
            var phones = !Contact_landline_value.IsNullOrEmpty()
                            ? Contact_landline_value
                                .Split('|')
                                .Select((phoneValue, i) => new OrganizationContact { ContactValue = phoneValue, Notes = Contact_landline_label.IsNullOrEmpty() ? null : Contact_landline_label.Split('|')[i], ContactTypeId = contactTypes.First(c => c.IsLandline()).Id })
                                .ToList()
                            : new List<OrganizationContact>();

            var mobiles = !Contact_mobile_value.IsNullOrEmpty()
                ? Contact_mobile_value
                    .Split('|')
                    .Select((phoneValue, i) => new OrganizationContact { ContactValue = phoneValue, Notes = Contact_mobile_label.IsNullOrEmpty() ? null : Contact_mobile_label.Split('|')[i], ContactTypeId = contactTypes.First(c => c.IsMobile()).Id })
                    .ToList()
                : new List<OrganizationContact>();

            var faxes = !Contact_fax_value.IsNullOrEmpty()
                            ? Contact_fax_value.Split('|')
                                .Select((faxValue, i) => new OrganizationContact { ContactValue = faxValue, Notes = Contact_fax_label.IsNullOrEmpty() ? null : Contact_fax_label.Split('|')[i], ContactTypeId = contactTypes.First(c => c.IsFax()).Id })
                                .ToList()
                            : new List<OrganizationContact>();
            var emails = !Contact_email_value.IsNullOrEmpty()
                            ? Contact_email_value.Split('|')
                                .Select((emailValue, i) => new OrganizationContact { ContactValue = emailValue, Notes = Contact_email_label.IsNullOrEmpty() ? null : Contact_email_label.Split('|')[i], ContactTypeId = contactTypes.First(c => c.IsEmail()).Id })
                                .ToList()
                            : new List<OrganizationContact>();

            if (phones.Any())
                phones.First().IsPrimary = true;
            if (mobiles.Any())
                mobiles.First().IsPrimary = true;
            if (faxes.Any())
                faxes.First().IsPrimary = true;
            if (emails.Any())
                emails.First().IsPrimary = true;

            var labels = !Label.IsNullOrEmpty()
                ? Label
                    .Split('|')
                    .Select((label, i) => new OrganizationLabel
                    {
                        OrganizationLabelTypeId = labelTypes.First(l => l.Code == LabelTypeCode.Split('|')[i]).Id,
                        Label = label
                    })
                    .ToList()
                : new List<OrganizationLabel>();

            var addresses = new List<OrganizationAddress>();

            if (!Address_TypeCode.IsNullOrEmpty())
                addresses.AddRange(Address_TypeCode
                    .Split('|')
                    .Select((_, i) => new OrganizationAddress
                    {
                        AddressTypeId = addressTypes.First(a => a.Code == Address_TypeCode.Split('|')[i]).Id,
                        Address = new Address
                        {
                            City = Address_City.Split('|')[i].NullIfEmpty(),
                            CountryCode = Address_CountryCode.Split('|')[i].NullIfEmpty(),
                            HouseNo = Address_HouseNo.Split('|')[i].NullIfEmpty(),
                            HouseNoAddition = Address_HouseNoAddition.Split('|')[i].NullIfEmpty(),
                            PostalCode = Address_PostalCode.Split('|')[i].NullIfEmpty(),
                            Province = Address_Province.Split('|')[i].NullIfEmpty(),
                            StreetName = Address_StreetName.Split('|')[i].NullIfEmpty(),
                            FreeField1 = Address_FreeField1.Split('|')[i].NullIfEmpty(),
                            FreeField2 = Address_FreeField2.Split('|')[i].NullIfEmpty(),
                            FreeField3 = Address_FreeField3.Split('|')[i].NullIfEmpty(),
                            CreatedDate = DateTime.UtcNow,
                            LastModifiedDate = DateTime.UtcNow
                        }
                    }));
            if (!PostOfficeBox_BoxNo.IsNullOrEmpty())
                addresses.AddRange(PostOfficeBox_BoxNo
                    .Split('|')
                    .Select((_, i) => new OrganizationAddress
                    {
                        PostOfficeBox = new PostOfficeBox
                        {
                            City = PostOfficeBox_City.Split('|')[i].NullIfEmpty(),
                            CountryCode = PostOfficeBox_CountryCode.Split('|')[i].NullIfEmpty(),
                            PostalCode = PostOfficeBox_PostalCode.Split('|')[i].NullIfEmpty(),
                            Province = PostOfficeBox_Provice.Split('|')[i].NullIfEmpty(),
                            BoxNo = PostOfficeBox_BoxNo.Split('|')[i].NullIfEmpty(),
                            CreatedDate = DateTime.UtcNow,
                            LastModifiedDate = DateTime.UtcNow
                        }
                    }));

            var notes = !Notes.IsNullOrEmpty()
                ? Notes.Split('|').Select(n => new OrganizationNote { Notes = n }).ToList()
                : new List<OrganizationNote>();

            var organizationApplications = applications.Select(application => new OrganizationApplication
            {
                ApplicationId = application.Id,
                Application = new Application { Name = application.Name},
                OrganizationId = string.IsNullOrEmpty(Id) ? Guid.Empty : Guid.Parse(Id)
            }).ToList();

            Preferred_Language = Preferred_Language.NullIfEmpty();

            return new OrganizationUnit
            {
                Id = string.IsNullOrEmpty(Id) ? Guid.Empty : Guid.Parse(Id),
                ShortName = ShortName,
                LongName = LongName,
                OrganizationContacts = phones.Concat(mobiles).Concat(faxes).Concat(emails).ToList(),
                Website = Website.NullIfEmpty(),
                VatNumber = VatNumber.NullIfEmpty(),
                ChamberOfCommerceNumber = ChamberOfCommerceNumber.NullIfEmpty(),
                CreatedDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                OrganizationLabels = labels,
                OrganizationNotes = notes,
                OrganizationApplications = organizationApplications,
                OrganizationCodes = config.OrganizationCodeTypeId != Guid.Empty ? new List<OrganizationCode>()
                {
                    new OrganizationCode()
                    {
                        Code = RelationCode,
                        OrganizationCodeTypeId = config.OrganizationCodeTypeId,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow
                    }
                }: new List<OrganizationCode>(),
                OrganizationAddresses = addresses,
                Agent = config.IsAgent
                    ? new Agent()
                    {
                        ExternalCode = RelationCode,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow
                    }
                    : null,
                Client = config.IsClient
                    ? new Client()
                    {
                        Mandate = config.IsMandate
                            ? new Mandate()
                            {
                                CreatedDate = DateTime.UtcNow,
                                LastModifiedDate = DateTime.UtcNow,
                                CultureCodePreferredLanguage = Preferred_Language
                                
                            }
                            : null,
                        ExternalCode = RelationCode,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        CultureCodePreferredLanguage = Preferred_Language
                    }
                    : null,
                Repairer = config.IsRepairer
                    ? new Repairer()
                    {
                        ExternalCode = RelationCode,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        CultureCodePreferredLanguage = Preferred_Language
                    }
                    : null,
                Partner = config.IsPartner
                    ? new Partner()
                    {
                        ExternalCode = RelationCode,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        CultureCodePreferredLanguage = Preferred_Language
                    }
                    : null,
                AlarmCenter = config.IsAlarmCenter
                    ? new AlarmCenter()
                    {
                        ExternalCode = RelationCode,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        CultureCodePreferredLanguage = Preferred_Language
                    }
                    : null,
                InternationalAssistanceGroup = config.IsInternationalAssistanceGroup
                    ? new InternationalAssistanceGroup()
                    {
                        ExternalCode = RelationCode,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        InternationalAssistanceGroupTypeId = internationalAssistanceGroupTypes.First(x => x.Code == InternationalAssistanceGroup_TypeCode).Id,
                        CultureCodePreferredLanguage = Preferred_Language
                    }
                    : null,
                Insurer = config.IsInsurer
                    ? new Insurer()
                    {
                        ExternalCode = RelationCode,
                        CultureCodePreferredLanguage = Preferred_Language
                    }
                    : null,
                Supplier = config.IsSupplier
                    ? new Supplier()
                    {
                        SupplierServices = config.ServiceIds?
                            .Select(id => new SupplierService()
                            {
                                ServiceId = new Guid(id),
                                EffectiveDate = DateTime.UtcNow
                            })
                            .ToList(),
                        ExternalCode = RelationCode,
                        CreatedDate = DateTime.UtcNow,
                        LastModifiedDate = DateTime.UtcNow,
                        CultureCodePreferredLanguage = Preferred_Language
                    }
                    : null
            };
        }
    }
}