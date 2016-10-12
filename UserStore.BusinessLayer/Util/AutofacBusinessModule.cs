using Autofac;
using UserStore.BusinessLayer.Interfaces;
using UserStore.BusinessLayer.Services;

namespace UserStore.BusinessLayer.Util
{
    public class AutofacBusinessModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserService>().As<IUserService>().InstancePerRequest();
            builder.RegisterType<DepartmentService>().As<IDepartmentService>().InstancePerRequest();
            builder.RegisterType<AuthService>().As<IAuthService>().InstancePerRequest();

            base.Load(builder);
        }
    }
}
