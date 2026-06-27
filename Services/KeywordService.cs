using System;

namespace CybersecurityChatbotGUI.Services
{
    public static class KeywordService
    {
        public static string GetIntent(string input)
        {
            string lower = input.ToLower();

            // ==========================================
            // TASK INTENTS
            // ==========================================
            if (lower.Contains("add task") || lower.Contains("create task") ||
                lower.Contains("new task") || lower.Contains("add a task"))
                return "ADD_TASK";

            if (lower.Contains("remind me") || lower.Contains("set reminder") ||
                lower.Contains("add reminder") || lower.Contains("reminder"))
                return "SET_REMINDER";

            if (lower.Contains("show tasks") || lower.Contains("list tasks") ||
                lower.Contains("view tasks") || lower.Contains("my tasks"))
                return "SHOW_TASKS";

            if (lower.Contains("delete task") || lower.Contains("remove task") ||
                lower.Contains("delete this task"))
                return "DELETE_TASK";

            if (lower.Contains("complete task") || lower.Contains("mark done") ||
                lower.Contains("task done"))
                return "COMPLETE_TASK";
            if (lower.contains("todo")  || lower.contains("todo")) return "ADD_TASK";

            // ==========================================
            // QUIZ INTENTS
            // ==========================================
            if (lower.Contains("start quiz") || lower.Contains("play quiz") ||
                lower.Contains("take quiz") || lower.Contains("do quiz") ||
                lower.Contains("quiz me"))
                return "START_QUIZ";

            // ==========================================
            // LOG INTENTS
            // ==========================================
            if (lower.Contains("activity log") || lower.Contains("show log") ||
                lower.Contains("what have you done") || lower.Contains("recent actions") ||
                lower.Contains("show activity"))
                return "SHOW_LOG";

            // ==========================================
            // HELP INTENT
            // ==========================================
            if (lower.Contains("help") || lower.Contains("what can i ask") ||
                lower.Contains("what can i do"))
                return "HELP";

            // ==========================================
            // CYBERSECURITY TOPICS (fallback to original)
            // ==========================================
            return GetResponseKey(input);
        }

        // ==========================================
        // ORIGINAL KEYWORD RECOGNITION (from Part 2)
        // ==========================================
        public static string GetResponseKey(string input)
        {
            string lower = input.ToLower();

            if (lower.Contains("password")) return "password_tips";
            if (lower.Contains("scam")) return "scam_tips";
            if (lower.Contains("privacy")) return "privacy_tips";
            if (lower.Contains("phishing")) return "phishing_tips";
            if (lower.Contains("safe browsing") || lower.Contains("browsing")) return "browsing_tips";
            if (lower.Contains("another tip") || lower.Contains("tell me more")) return "more";

            return "default";
        }
    }
}
