using Autofac;
using UserStore.DataLayer.EF;
using UserStore.DataLayer.Interfaces;
using UserStore.DataLayer.Repositories;

namespace UserStore.DataLayer.Util
{
    public class AutofacDataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<IdentityUnitOfWork>()
                .As<IUnitOfWork>()
                .WithParameter("connectionString", "DefaultConnection")
                .InstancePerRequest();

            base.Load(builder);
        }
    }
}
