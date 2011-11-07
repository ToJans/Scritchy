using System.Web.Mvc;
using System.Web.Routing;
using Example.Domain.Readmodel;
using Example.Web.Controllers;
using Ninject;
using Ninject.Web.Mvc;
using Scritchy.Infrastructure;
using Scritchy.Infrastructure.Configuration;
using Scritchy.Infrastructure.Implementations;

namespace Example.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : NinjectHttpApplication
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Bus", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected override void OnApplicationStarted()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }

        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<ICommandBus>().ToConstant(new ScritchyBus(x => kernel.Get(x))).InSingletonScope();
            kernel.Bind<BusController.CommandHistory>().ToSelf().InSingletonScope();
            kernel.Bind<StockDictionaryHandler>().ToSelf().InSingletonScope();
            kernel.Bind<StockDictionary>().ToConstant(new StockDictionary());
            return kernel;
        }

    }
}