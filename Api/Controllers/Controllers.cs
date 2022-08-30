using Api.Controllers.Abstract;
using DataAccess;

namespace Api.Controllers
{
    public class AddressController : BaseController<Address> { }
    public class AgentController : DeletableBaseController<Agent> { }
    public class ApplicationController : BaseController<Application> { }
    public class ClientController : DeletableBaseController<Client> { }
    public class SupplierServiceController : BaseController<SupplierService> { }
    public class SupplierController : DeletableBaseController<Supplier> { }
    public class RepairerController : DeletableBaseController<Repairer> { }
    public class RegionController : BaseController<Region> { }
    public class PostOfficeBoxController : BaseController<PostOfficeBox> { }
    public class PartnerController : DeletableBaseController<Partner> { }
    public class OrganizationCodeTypeController : BaseController<OrganizationCodeType> { }
    public class MandateController : DeletableBaseController<Mandate> { }
    public class LocalizedEntryController : DeletableBaseController<LocalizedEntry> { }
    public class LocalizableTypeController : BaseController<LocalizableType> { }
    public class LocalizableEntryController : BaseController<LocalizableEntry> { }
    public class LegalEntityController : BaseController<LegalEntity> { }
    public class JobController : BaseController<Job> { }
    public class HierarchyTypeController : BaseController<HierarchyType> { }
    public class AlarmCenterController : BaseController<AlarmCenter> { }
    public class PaymentMethodController: BaseController<PaymentMethod> { }
    public class InternationalAssistanceGroupTypeController : BaseController<InternationalAssistanceGroupType> { }
}