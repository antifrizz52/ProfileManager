using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using UserStore.BusinessLayer.Util;
using UserStore.DataLayer.Util;

namespace UserStore.WebLayer.Util
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new AutofacWebTypesModule());

            builder.RegisterModule(new AutofacDataModule());
            builder.RegisterModule(new AutofacBusinessModule());
            builder.RegisterModule(new AutofacWebModule());

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}