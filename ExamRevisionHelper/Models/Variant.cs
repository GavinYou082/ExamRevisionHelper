﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRevisionHelper.Models
{
    public class Variant
    {
        public char VariantCode { get; set; }
        public Paper[] Papers { get; set; }
    }
}
