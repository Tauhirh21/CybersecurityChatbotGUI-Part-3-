using System.IO;
using System.Windows.Media;

namespace CybersecurityChatbotGUI.Services
{
    public static class AudioService
    {
        private static MediaPlayer _player = new MediaPlayer();

        public static void PlayGreeting()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Audio", "greeting.wav");
            if (File.Exists(path))
            {
                _player.Open(new Uri(path, UriKind.Absolute));
                _player.Play();
            }
        }
    }
}