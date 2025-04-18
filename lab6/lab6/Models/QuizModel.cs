using System.Collections.Generic;

namespace lab6.Models
{
    public class QuizModel
    {
        public string Text { get; set; }
        public string TranslatedText { get; set; }
        public List<QuizQuestion> Questions { get; set; } = new();

    }
}
