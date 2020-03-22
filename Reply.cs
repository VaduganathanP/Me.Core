using System;
using System.Collections.Generic;
using System.Text;

namespace Me.Core
{
    public class Reply
    {
        public bool IsAnswer { get; set; }
        public string DisplayValue { get; set; }
        public string AnswerId { get; set; }
        public string Question { get; set; }
        public List<string> Answers { get; set; } = new List<string>();
        public int CurrentAnswerIndex { get; set; }
    }
}
