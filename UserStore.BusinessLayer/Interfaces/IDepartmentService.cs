using System.Collections.Generic;
using System.Threading.Tasks;
using UserStore.BusinessLayer.DTO;
using UserStore.BusinessLayer.Infrastructure;

namespace UserStore.BusinessLayer.Interfaces
{
    public interface IDepartmentService
    {
        // todo : add CRUD??

        Task<OperationDetails> Create(DepartmentDTO departmentDto);
        DepartmentDTO GetDepartment(int? id);
        IEnumerable<DepartmentDTO> GetDepartments();
        Task<OperationDetails> Update(DepartmentDTO departmentDto);
        IEnumerable<UserDTO> GetAssociatedUsers(int? id);

        void Dispose();
    }
}
