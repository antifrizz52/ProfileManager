using System.Reflection;
using Autofac;
using Autofac.Integration.Mvc;


namespace UserStore.WebLayer.Util
{
    public class AutofacWebModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            base.Load(builder);
        }
    }
}