

namespace PlatformSchool.Domain.Models
{
    public record CourseGetModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        public int DepartmentId { get; set; }
        public int DepartmentName { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
