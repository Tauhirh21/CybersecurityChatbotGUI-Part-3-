using System;
using System.Collections.Generic;

namespace CybersecurityChatbotGUI.Services
{
    public static class ResponseService
    {
        private static Dictionary<string, List<string>> _responses = new();

        static ResponseService()
        {
            _responses["password_tips"] = new List<string>
            {
                "🔐 Use 12+ characters with uppercase, lowercase, numbers, and symbols.",
                "🔐 Never reuse passwords – use a password manager.",
                "🔐 Enable Two‑Factor Authentication (2FA) like FaceID or fingerprint passwords.",
                 "🔐 NEW TIP: Avoid using dictionary words in your passwords."
            };
            _responses["scam_tips"] = new List<string>
            {
                "⚠️ If it sounds too good to be true, it is most definetly a scam.",
                "⚠️ Never share OTPs or bank details over phones, messages, or email.",
                "⚠️ Analyze and check the sender's email address carefully."
            };
            _responses["privacy_tips"] = new List<string>
            {
                "🕵️ Review social media privacy settings every 3 months.",
                "🕵️ Use a VPN on public Wi‑Fi.",
                "🕵️ Limit oversharing online."
            };
            _responses["phishing_tips"] = new List<string>
            {
                "🎣 Hover over links before clicking – see the real URL.",
                "🎣 Real companies never ask for passwords via email.",
                "🎣 Look for spelling mistakes."
            };
            _responses["browsing_tips"] = new List<string>
            {
                "🌐 Look for 🔒 and 'https://' before entering info.",
                "🌐 Clear cache/cookies regularly.",
                "🌐 Always keep your browser updated."
            };
            _responses["more"] = new List<string>
            {
                "Here's another tip: ",
                "Sure! Let me add: "
            };
            _responses["default"] = new List<string>
            {
                "I don't understand. Try 'password', 'scam', 'privacy', 'phishing', or 'safe browsing'."
            };
        }

        public static string GetRandomResponse(string key)
        {
            if (_responses.ContainsKey(key) && _responses[key].Count > 0)
            {
                var list = _responses[key];
                Random r = new Random();
                return list[r.Next(list.Count)];
            }
            return _responses["default"][0];
        }

        public static string GetNextTipForTopic(string topicKey, int index)
        {
            if (_responses.ContainsKey(topicKey) && _responses[topicKey].Count > 0)
            {
                var list = _responses[topicKey];
                return list[index % list.Count];
            }
            return GetRandomResponse("default");
        }
    }
}