namespace Api.Importing
{
    public abstract class OrganizationUnitImportExportBaseClass
    {
        public string Id { get; set; }
        public string RelationCode { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string Address_TypeCode { get; set; }
        public string Address_StreetName { get; set; }
        public string Address_HouseNo { get; set; }
        public string Address_HouseNoAddition { get; set; }
        public string Address_PostalCode { get; set; }
        public string Address_City { get; set; }
        public string Address_Province { get; set; }
        public string Address_CountryCode { get; set; }

        public string Address_FreeField1 { get; set; }
        public string Address_FreeField2 { get; set; }
        public string Address_FreeField3 { get; set; }

        public string Contact_landline_value { get; set; }
        public string Contact_landline_label { get; set; }

        public string Contact_mobile_value { get; set; }
        public string Contact_mobile_label { get; set; }

        public string Contact_fax_value { get; set; }
        public string Contact_fax_label { get; set; }

        public string Contact_email_value { get; set; }
        public string Contact_email_label { get; set; }

        public string Label { get; set; }
        public string LabelTypeCode { get; set; }

        public string Notes { get; set; }
        public string Preferred_Language { get; set; }

        public string Website { get; set; }
        public string PostOfficeBox_BoxNo { get; set; }
        public string PostOfficeBox_PostalCode { get; set; }
        public string PostOfficeBox_City { get; set; }
        public string PostOfficeBox_Provice { get; set; }
        public string PostOfficeBox_CountryCode { get; set; }
        public string VatNumber { get; set; }
        public string ChamberOfCommerceNumber { get; set; }
    }
}