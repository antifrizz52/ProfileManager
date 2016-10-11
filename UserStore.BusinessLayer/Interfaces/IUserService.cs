using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;

namespace UserStore.BusinessLayer.Interfaces
{
    public interface IUserService
    {
        // todo : add CRUD??

        Task<OperationDetails> Create(UserDTO userDto);
        UserDTO GetUser(int? id);
        IEnumerable<UserDTO> GetUsers();
        Task<ClaimsIdentity> Authenticate(UserDTO userDto);
        Task SetInitialData(UserDTO adminDto, List<string> roles);

        void Dispose();
    }
}
