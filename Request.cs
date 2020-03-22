using System;
using System.Collections.Generic;
using System.Text;

namespace Me.Core
{
    public class Request
    {
        /// <summary>
        /// Is this request represent a conversation
        /// </summary>
        public bool IsConversation { get; set; }
        /// <summary>
        /// Tuple<S.No, question, answer, isAnswerIsQuestion>
        /// </summary>
        public List<Tuple<int, string, string, bool>> Conversations { get; set; } = new List<Tuple<int, string, string, bool>>();
        /// <summary>
        /// Question
        /// </summary>
        public string Question { get; set; }
        /// <summary>
        /// Answer
        /// </summary>
        public string Answer { get; set; }
        /// <summary>
        /// Display Value Or a Multiline answer
        /// </summary>
        public string DisplayValue { get; set; }

    }
}
