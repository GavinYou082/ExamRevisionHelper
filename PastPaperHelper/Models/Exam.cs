﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.Models
{
    public class Exam
    {
        public Subject Subject { get; set; }
        public string Year { get; set; }
        public ExamSeries ExamSeries { get; set; }
        //public PaperItem GradeThreshold { get; set; }
        public PaperItem[] Papers { get; set; }
    }

    public enum Curriculums { IGCSE, ALevel }
}