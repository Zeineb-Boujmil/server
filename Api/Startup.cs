using Api.Attributes;
using Api.ViewModels;
using DataAccess;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security.Jwt;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using CoverageProduct = DataAccess.CoverageProduct;

[assembly: OwinStartup(typeof(Api.Startup))]
namespace Api
{
    public class Startup
    {
        private static readonly string SharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
        private static readonly string AudienceUri = ConfigurationManager.AppSettings["AudienceUri"];
        private static readonly string TokenIssuerId = ConfigurationManager.AppSettings["TokenIssuerId"];


        public void Configuration(IAppBuilder app)
        {
            app.UseExceptionHandler();
            app.UseWebApi(SetupOdata(app));
        }

        private static void ConfigureAuthentication(IAppBuilder appBuilder)
        {
            var symmetricKey = TextEncodings.Base64Url.Decode(SharedSecret);

            appBuilder.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { AudienceUri },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(TokenIssuerId, symmetricKey)
                    }
                });
        }

        private static EnableCorsAttribute ConfigureAndGetCorsAttribute()
        {
            var corsAttribute = new EnableCorsAttribute("*", "*", "*");
            corsAttribute.ExposedHeaders.Add("Content-Disposition");
            corsAttribute.ExposedHeaders.Add("x-filename");
            return corsAttribute;
        }

        private static HttpConfiguration SetupOdata(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            config.Services.Replace(typeof(IExceptionLogger), new GlobalExceptionLogger());
            config.EnableCors(ConfigureAndGetCorsAttribute());
            config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);

            ConfigureAuthentication(app);

            config.Filters.Add(new GlobalExceptionHandlerFilterAttribute());

            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            config.SetTimeZoneInfo(TimeZoneInfo.Utc);

            var builder = new ODataConventionModelBuilder()
                .EnableLowerCamelCase();

            builder.EntitySet<Tenant>();
            builder.EntitySet<OrganizationType>();
            builder.EntitySet<CollectionCode>();
            builder.EntitySet<OrganizationUnit>();
            builder.EntitySet<OrganizationGroup>();
            builder.EntitySet<HierarchyType>();
            builder.EntitySet<Address>();
            builder.EntitySet<PostOfficeBox>();
            builder.EntitySet<LocalizableEntry>();
            builder.EntitySet<LocalizableType>();
            builder.EntitySet<LocalizedEntry>();
            builder.EntitySet<Client>();
            builder.EntitySet<Mandate>();
            builder.EntitySet<Agent>();
            builder.EntitySet<Partner>();
            builder.EntitySet<Repairer>();
            builder.EntitySet<Supplier>();
            builder.EntitySet<OrganizationHierarchy>();
            builder.EntitySet<TaxCode>();
            builder.EntitySet<TaxType>();
            builder.EntitySet<Product>();
            builder.EntitySet<ProductGroup>();
            builder.EntitySet<DamageReason>();
            builder.EntitySet<BankAccount>();
            builder.EntitySet<OrganizationAccount>("OrganizationAccountOData");
            builder.EntitySet<HandlingType>();
            builder.EntitySet<MainContract>("Contract");
            builder.EntitySet<OrganizationCodeType>();
            builder.EntitySet<ContactPersonType>();
            builder.EntitySet<Gender>();
            builder.EntitySet<Skill>();
            builder.EntitySet<ServiceGroup>();
            builder.EntitySet<Service>();
            builder.EntitySet<WorkActivity>();
            builder.EntitySet<ActivityOption>();
            builder.EntitySet<InsuranceCoverage>();
            builder.EntitySet<InsuranceObject>();
            builder.EntitySet<ActivityExecution>();
            builder.EntitySet<Application>();
            builder.EntitySet<OrganizationUnitWithCurrentStatus>();
            builder.EntitySet<DamageLocation>();
            builder.EntitySet<Brand>();
            builder.EntitySet<Specification>();
            builder.EntitySet<OrganizationHierarchyTreeNode>("OrganizationHierarchyTree");
            builder.EntitySet<PaintType>();
            builder.EntitySet<ContactType>();
            builder.EntitySet<OrganizationLabelType>();
            builder.EntitySet<Status>();
            builder.EntitySet<Fuel>();
            builder.EntitySet<Transmission>();
            builder.EntitySet<MeterReadingDetermined>();
            builder.EntitySet<MeterReadingQuantity>();
            builder.EntitySet<InputMethod>();
            builder.EntitySet<Color>();
            builder.EntitySet<Chain>();
            builder.EntitySet<RoleCode>();
            builder.EntitySet<CorrespondenceMethod>();
            builder.EntitySet<TimeWindowList>();
            builder.EntitySet<Currency>();
            builder.EntitySet<Country>();
            builder.EntitySet<SepaCountry>();
            builder.EntitySet<Employee>();
            builder.EntitySet<Job>();
            builder.EntitySet<Region>();
            builder.EntitySet<CostAgreementType>();
            builder.EntitySet<CostSettlement>();
            builder.EntitySet<LegalEntity>();
            builder.EntitySet<BusinessUnit>();
            builder.EntitySet<SupplierService>();
            builder.EntitySet<CostAgreement>();
            builder.EntitySet<AddressType>();
            builder.EntitySet<InternationalAssistanceGroupType>();
            builder.EntitySet<DocumentType>();
            builder.EntitySet<ClientInsuranceProduct>();
            builder.EntitySet<OrganizationUnit>("organizationUnitWithInvalidVat");
            builder.EntitySet<Creditor>();
            builder.EntitySet<AlarmCenter>();
            builder.EntitySet<ClientInvoiceRecipient>();
            builder.EntitySet<ClientMandate>();
            builder.EntitySet<Insurer>();
            builder.EntitySet<TimeSlot>();
            builder.EntitySet<TimeTable>();
            builder.EntitySet<TimeTableSlot>();
            builder.EntitySet<AppointmentTimeSlot>();
            builder.EntitySet<PaymentMethod>();
            builder.EntitySet<PaymentCondition>();
            builder.EntitySet<OrganizationPaymentMethod>();
            builder.EntitySet<OrganizationPaymentCondition>();
            builder.EntitySet<CreditorAccount>();
            builder.EntitySet<CreditorOrganizationRelation>();
            builder.EntitySet<Log>();
            builder.EntitySet<Debtor>();
            builder.EntitySet<DebtorAccount>();
            builder.EntitySet<DebtorOrganizationRelation>();
            builder.EntitySet<ContractParty>();
            builder.EntitySet<ConvenantParty>();
            builder.EntitySet<CoverageProduct>();
            builder.EntitySet<OrganizationCoverageProduct>();
            builder.EntitySet<OrganizationCoverageProductsExtension>();
            builder.EntitySet<InvoiceDeliveryMethod>();
            builder.EntitySet<RoadAuthority>();
            builder.EntitySet<RoadAuthorityType>();
            builder.EntitySet<LeasingCompany>();
            builder.EntitySet<SalesOrderApprovalSetting>();
            builder.EntitySet<ProductClassification>();
            builder.EntitySet<SalesOrderApprovalSettingsView>("salesorderapprovalsettings");
            builder.EntitySet<DebtorOrganizationRelationsView>("debtororganizationrelations");
            builder.EntitySet<Template>();

            // Functions
            builder.Namespace = "Services";
            var fillDropDown = builder.EntityType<OrganizationHierarchy>().Collection.Function("FillDropDown");
            fillDropDown.Parameter<Guid?>("organizationId");
            fillDropDown.Parameter<string>("filter");
            fillDropDown.Parameter<Guid?>("hierarchyTypeId");
            fillDropDown.ReturnsCollection<DropDownViewModel>();

            var fillDepartmentDropDown = builder.EntityType<OrganizationUnit>().Collection.Function("FillDepartmentDropDown");
            fillDepartmentDropDown.Parameter<Guid?>("legalEntityId");
            fillDepartmentDropDown.Parameter<string>("filter");
            fillDepartmentDropDown.ReturnsCollection<DropDownViewModel>();

            var fillInsuranceObjectHierarchyDropDown = builder.EntityType<InsuranceObject>().Collection.Function("FillHierarchyDropdown");
            fillInsuranceObjectHierarchyDropDown.Parameter<Guid?>("objectId");
            fillInsuranceObjectHierarchyDropDown.Parameter<string>("filter");
            fillInsuranceObjectHierarchyDropDown.ReturnsCollection<DropDownViewModel>();

            var fillInsuranceCoverageHierarchyDropDown = builder.EntityType<InsuranceCoverage>().Collection.Function("FillHierarchyDropdown");
            fillInsuranceCoverageHierarchyDropDown.Parameter<Guid?>("coverageId");
            fillInsuranceCoverageHierarchyDropDown.Parameter<string>("filter");
            fillInsuranceCoverageHierarchyDropDown.ReturnsCollection<DropDownViewModel>();

            var fillProductClassificationDropDown = builder.EntityType<ProductClassification>().Collection.Function("FillProductClassificationDropdown");
            fillProductClassificationDropDown.Parameter<Guid?>("productClassificationId");
            fillProductClassificationDropDown.Parameter<string>("filter");
            fillProductClassificationDropDown.ReturnsCollection<DropDownViewModel>();

            var organizationList = builder.EntityType<OrganizationUnit>().Collection.Action("SaveAll");
            organizationList.CollectionEntityParameter<OrganizationUnit>("organizationUnits");

            var fillAvailableCreditorSupplierDropDown = builder.EntityType<Creditor>().Collection
                .Function("FillCreditorAvailableSuppliersDropDown");
            fillAvailableCreditorSupplierDropDown.Parameter<Guid?>("legalEntityId");
            fillAvailableCreditorSupplierDropDown.Parameter<string>("vatNumber");
            fillAvailableCreditorSupplierDropDown.Parameter<string>("filter");
            fillAvailableCreditorSupplierDropDown.ReturnsCollection<DropDownViewModel>();

            var fillAvailableDebtorClientDropDown = builder.EntityType<Debtor>().Collection
                .Function("FillDebtorAvailableClientsDropDown");
            fillAvailableDebtorClientDropDown.Parameter<Guid?>("legalEntityId");
            fillAvailableDebtorClientDropDown.Parameter<string>("vatNumber");
            fillAvailableDebtorClientDropDown.Parameter<string>("filter");
            fillAvailableDebtorClientDropDown.ReturnsCollection<DropDownViewModel>();

            config.MapODataServiceRoute("ODataRoute", "odata", builder.GetEdmModel());

            return config;
        }
    }
}