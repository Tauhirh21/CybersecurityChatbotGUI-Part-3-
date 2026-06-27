using CybersecurityChatbotGUI.Models;

namespace CybersecurityChatbotGUI.Services
{
    public static class SentimentService
    {
        public static void DetectAndUpdate(string input, UserMemory memory)
        {
            string lower = input.ToLower();
            if (lower.Contains("worried") || lower.Contains("scared") || lower.Contains("afraid"))
            {
                memory.IsWorried = true;
            }
            else
            {
                memory.IsWorried = false;
            }
        }

        public static string GetEmpathyResponse()
        {
            return "It is understandable to feel that way. Let me help you with a tip.";
        }
    }
}