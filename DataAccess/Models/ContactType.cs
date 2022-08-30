namespace DataAccess
{
    public partial class ContactType
    {
        public bool IsLandline()
        {
            return Code == "Landline";
        }

        public bool IsMobile()
        {
            return Code == "Mobile";
        }

        public bool IsFax()
        {
            return Code == "Fax";
        }

        public bool IsEmail()
        {
            return Code == "Email";
        }
    }
}