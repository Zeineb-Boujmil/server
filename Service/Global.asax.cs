using System.Configuration;
using System.Web.Http;
using AutoMapper;
using CED.Framework.Interfaces;
using CED.Framework.Logging;
using DataAccess;
using DataAccess.Repositories;
using MasterDataService.DTO;
using MasterDataService.DTO.Entities;
using Ninject;
using Ninject.Extensions.Wcf;
using Ninject.Web.Common;

namespace MasterDataService
{
	public class Global : NinjectHttpApplication
	{
	    private static readonly string LogRequests = ConfigurationManager.AppSettings["LogRequests"];
	    private static readonly string LoggerName = "Ced.MasterDataService";

	    protected override void OnApplicationStarted()
	    {
	        GlobalConfiguration.Configure(RegisterWebApi);
	        base.OnApplicationStarted();
	    }

		protected override IKernel CreateKernel()
		{
			var kernel = new StandardKernel();
			BindServices(kernel);
			NinjectServiceHostFactory.SetKernel(kernel);
			return kernel;
		}

		private static void BindServices(IKernel kernel)
		{
			kernel.Bind<ILogger>().ToMethod(ctx => Logger.GetLogger(LoggerName));
		    kernel.Bind<IPingRepository>().To<PingRepository>();
			kernel.Bind<IMapper>().ToMethod(ctx =>
			{
				var config = new MapperConfiguration(cfg =>
				{
					cfg.CreateMap<AddressDto, Address>().ReverseMap();
					cfg.CreateMap<PostOfficeBoxDto, PostOfficeBox>().ReverseMap();
					cfg.CreateMap<OrganizationUnitDto, OrganizationUnit>()
						.AfterMap((src, dest) =>
						{
							dest.AddressId = dest.Address?.Id;
							dest.PostOfficeBoxId = dest.PostOfficeBox?.Id;

							if (dest.Supplier != null)
								dest.Supplier.Id = dest.Id;
						})
						.ReverseMap();
					cfg.CreateMap<SupplierDto, Supplier>().ReverseMap();

					cfg.CreateMap<BankAccountDto, BankAccount>();
				    cfg.CreateMap<DocumentDto, Document>();
					cfg.CreateMap<OrganizationAccountDto, OrganizationAccount>();
				});

				return config.CreateMapper();
			});
		}

	    private static void RegisterWebApi(HttpConfiguration httpConfiguration)
	    {
	        httpConfiguration.MapHttpAttributeRoutes();
	        RegisterWebApiRequestLogger(httpConfiguration);
	    }

	    private static void RegisterWebApiRequestLogger(HttpConfiguration httpConfiguration)
	    {
	        if (CanLogWebApiRequests())
	        {
	            var logger = Logger.GetLogger(LoggerName);
	            httpConfiguration.MessageHandlers.Add(new RequestLogger(logger));
	        }
	    }

	    private static bool CanLogWebApiRequests()
	    {
	        bool logRequests;
	        return bool.TryParse(LogRequests, out logRequests) && logRequests;
	    }
	}
}