﻿namespace PastPaperHelper.Models
{
    public class PaperRepository
    {
        public Subject Subject { get; set; }
        public Syllabus[] Syllabus { get; set; }
        public Exam[] Exams { get; set; }

        public PaperRepository(Subject subject)
        {
            Subject = subject;
        }

    }
}
