
using PlatformSchool.Domain.Base;
using PlatformSchool.Domain.Models;
using PlatformSchool.Domain.Models.Department;

namespace PlatformSchool.Domain.Repositories
{
    public interface IDepartmentRepository
    {
        Task<OperationResult<List<DepartmentGetModel>>> GetAllDepartmentsAsync();
        Task<OperationResult<DepartmentGetModel>> GetDepartmentByIdAsync(int id);
        Task<OperationResult<DepartmentGetModel>> CreateDepartmentAsync(DepartmentGetModel model);  
    }
}
