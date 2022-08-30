namespace VatValidatorJob
{
    class Program
    {
        static void Main(string[] args)
        {
            new VatValidator(new WcfServiceCedVatNumberCheckClient()).CheckAllClientsVat();
        }
    }
}
