using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI.Services
{
    public static class ActivityLogger
    {
        private static List<string> _log = new List<string>();
        private const int MaxEntries = 10;

        public static void Log(string action)
        {
            string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {action}";
            _log.Insert(0, entry);
            if (_log.Count > MaxEntries)
                _log.RemoveAt(_log.Count - 1);
        }

        public static List<string> GetLog() => new List<string>(_log);
    }
}