

using PlatformSchool.Domain.Entities;
using PlatformSchool.Domain.Models;

namespace PlatformSchool.Domain.Repositories
{
    public interface ICourseRepository : IBaseRepository<Course,CourseGetModel>
    {
        public List<CourseGetModel> GetCoursesByDepartment(int departmentId);
    }
}
