using System;

namespace Me.Core
{
    public static class Question
    {
        public static Reply Process(Request request)
        {
            string retVal = string.Empty;
            if (string.IsNullOrEmpty(request.Question) || string.IsNullOrWhiteSpace(request.Question))
                return null;
            else
                request.Question = request.Question.Trim();
            if (string.IsNullOrEmpty(request.Answer) || string.IsNullOrWhiteSpace(request.Answer))
            {
                if (string.IsNullOrEmpty(request.DisplayValue) || string.IsNullOrWhiteSpace(request.DisplayValue))
                    return Answer.GetAnswer(request);
                else
                    request.DisplayValue = request.DisplayValue.Trim();
            }
            else
                request.Answer = request.Answer.Trim();


            if (!string.IsNullOrEmpty(request.Answer) || !string.IsNullOrEmpty(request.DisplayValue))
            {
                Answer.SaveAnswer(request);
                return null;
            }

            return Answer.GetAnswer(request);
        }

        
    }
}
