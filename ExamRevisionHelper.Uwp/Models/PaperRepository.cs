﻿using System.Collections.Generic;

namespace ExamRevisionHelper.Models
{
    public class PaperRepository : List<ExamYear>
    {
        public Subject Subject { get; set; }

        public PaperRepository(Subject subject)
        {
            Subject = subject;
        }

        public ExamYear this[string year] { get => GetExamYear(year); }
        public ExamYear GetExamYear(string Year)
        {
            foreach (ExamYear item in this)
            {
                if (item.Year == Year) return item;
            }
            return null;
        }
    }
}
