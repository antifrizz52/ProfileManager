using System.Collections.Generic;
using System.Threading.Tasks;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;

namespace UserStore.BusinessLayer.Interfaces
{
    public interface IUserService
    {
        // todo : add CRUD??

        Task<OperationDetails> CreateProfile(UserDTO userDto);
        UserDTO GetUser(int? id);
        IEnumerable<UserDTO> GetUsers();

        void Dispose();
    }
}
