using Example.Domain.Readmodel;
using Ninject;
using Scritchy.Infrastructure.Configuration;
using SignalR.Infrastructure;
using SignalR.Ninject;
using Scritchy.Infrastructure;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Example.SignalR.App_Start.NinjectSignalR), "Start")]

namespace Example.SignalR.App_Start {
    public static class NinjectSignalR {
        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() {
            IKernel kernel = CreateKernel();
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel() {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel) {
            kernel.Bind<ScritchyBus>().ToMethod(k=>new ScritchyBus( t => k.Kernel.Get(t))).InSingletonScope();
            kernel.Bind<StockDictionary>().ToSelf().InSingletonScope();
            kernel.Bind<StockDictionaryHandler>().ToSelf().InSingletonScope();
        }
    }
}