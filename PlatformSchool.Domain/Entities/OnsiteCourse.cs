﻿

namespace PlatformSchool.Domain.Entities 
{
    public sealed class OnsiteCourse
    {
        public int CourseId { get; set; }

        public string Location { get; set; }

        public string Days { get; set; }

        public DateTime Time { get; set; }
    }
}

