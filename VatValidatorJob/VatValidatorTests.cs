using NUnit.Framework;

namespace VatValidatorJob
{
    public class VatValidatorTests
    {
        [Test]
        public void VatValidatorServiceIntegrationTest()
        {
            new VatValidator(new WcfServiceCedVatNumberCheckClientMock()).CheckAllClientsVat();
        }
    }
}