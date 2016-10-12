using System.Security.Claims;
using System.Threading.Tasks;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;

namespace UserStore.BusinessLayer.Interfaces
{
    public interface IAuthService
    {
        Task<OperationDetails> Create(AppUserDTO appUser);
        Task<ClaimsIdentity> Authenticate(AppUserDTO appUser);

        void Dispose();
    }
}