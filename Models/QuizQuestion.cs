using System.Collections.Generic;

namespace CybersecurityChatbotGUI.Models
{
    public class QuizQuestion
    {
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int CorrectIndex { get; set; }   // 0‑based index
        public string Explanation { get; set; }
    }
}
