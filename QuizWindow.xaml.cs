using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CybersecurityChatbotGUI.Models;
using CybersecurityChatbotGUI.Services;

namespace CybersecurityChatbotGUI
{
    public partial class QuizWindow : Window
    {
        private void BtnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            var quiz = new QuizWindow();
            quiz.Owner = this;
            quiz.ShowDialog();
            ActivityLogger.Log("Quiz started by user.");
        }

        private List<QuizQuestion> _questions;
        private int _currentIndex = 0;
        private int _score = 0;
        private RadioButton _selectedRadio;

        public QuizWindow()
        {
            InitializeComponent();
            LoadQuestions();
            ShowQuestion();
        }

        private void LoadQuestions()
        {
            _questions = new List<QuizQuestion>
            {
                // 1
                new QuizQuestion
                {
                    Text = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report as phishing", "Ignore it" },
                    CorrectIndex = 2,
                    Explanation = "Legitimate companies never ask for passwords via email. Report phishing to help protect others."
                },
                // 2
                new QuizQuestion
                {
                    Text = "True or False: Using the same password for multiple accounts is safe.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Reusing passwords increases risk. Use a different password for every account."
                },
                // 3
                new QuizQuestion
                {
                    Text = "Which of these is an example of phishing?",
                    Options = new List<string> { "A friend's social media post", "An email from your bank asking to click a link", "A pop-up ad", "A text from your mom" },
                    CorrectIndex = 1,
                    Explanation = "Phishing often impersonates trusted organisations via email or text to steal your info."
                },
                // 4
                new QuizQuestion
                {
                    Text = "What does 2FA stand for?",
                    Options = new List<string> { "Two-Factor Authentication", "Two-Form Access", "Trusted Firewall Access", "Total File Access" },
                    CorrectIndex = 0,
                    Explanation = "2FA adds an extra layer of security by requiring a second verification step."
                },
                // 5
                new QuizQuestion
                {
                    Text = "True or False: Public Wi-Fi is safe for online banking.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 1,
                    Explanation = "Public Wi-Fi is not secure; avoid sensitive transactions. Use a VPN if you must."
                },
                // 6
                new QuizQuestion
                {
                    Text = "Which of these is a strong password?",
                    Options = new List<string> { "123456", "password", "S3cur3P@ssw0rd!", "qwerty" },
                    CorrectIndex = 2,
                    Explanation = "A strong password has uppercase, lowercase, numbers, and symbols."
                },
                // 7
                new QuizQuestion
                {
                    Text = "What is social engineering?",
                    Options = new List<string> { "A programming language", "Manipulating people to reveal confidential info", "A type of firewall", "A password manager" },
                    CorrectIndex = 1,
                    Explanation = "Social engineering tricks people into giving up sensitive data."
                },
                // 8
                new QuizQuestion
                {
                    Text = "True or False: You should enable automatic updates on your devices.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "Automatic updates ensure you get critical security patches quickly."
                },
                // 9
                new QuizQuestion
                {
                    Text = "What does HTTPS stand for?",
                    Options = new List<string> { "Hyper Text Transfer Protocol Secure", "High Traffic Transfer Protocol", "Hyper Transfer Text Protocol", "None of the above" },
                    CorrectIndex = 0,
                    Explanation = "HTTPS encrypts data between your browser and the website."
                },
                // 10
                new QuizQuestion
                {
                    Text = "You receive a suspicious link from an unknown sender. What should you do?",
                    Options = new List<string> { "Click it", "Forward it to friends", "Delete and report it", "Download the attachment" },
                    CorrectIndex = 2,
                    Explanation = "Never click unknown links. Delete and report to help stop scams."
                },
                // 11 (extra)
                new QuizQuestion
                {
                    Text = "Which of these is a common sign of a phishing email?",
                    Options = new List<string> { "Professional grammar", "Urgent language and threats", "Personal greeting", "No spelling mistakes" },
                    CorrectIndex = 1,
                    Explanation = "Scammers create urgency to pressure you into acting without thinking."
                },
                // 12 (extra)
                new QuizQuestion
                {
                    Text = "True or False: A VPN can protect your privacy online.",
                    Options = new List<string> { "True", "False" },
                    CorrectIndex = 0,
                    Explanation = "A VPN masks your IP and encrypts your traffic, enhancing privacy."
                }
            };

            // Shuffle questions for variety
            var rand = new Random();
            _questions = _questions.OrderBy(q => rand.Next()).ToList();
        }

        private void ShowQuestion()
        {
            if (_currentIndex >= _questions.Count)
            {
                EndQuiz();
                return;
            }

            var q = _questions[_currentIndex];
            TblQuestion.Text = q.Text;
            PanelAnswers.Children.Clear();
            _selectedRadio = null;

            for (int i = 0; i < q.Options.Count; i++)
            {
                var rb = new RadioButton
                {
                    Content = q.Options[i],
                    Tag = i,
                    Margin = new Thickness(5),
                    Foreground = System.Windows.Media.Brushes.White,
                    GroupName = "answers"
                };
                rb.Checked += (s, e) => { _selectedRadio = rb; BtnNext.IsEnabled = true; };
                PanelAnswers.Children.Add(rb);
            }

            BtnNext.IsEnabled = false;
            TblQuestion.Foreground = System.Windows.Media.Brushes.White;
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRadio == null)
            {
                MessageBox.Show("Please select an answer.");
                return;
            }

            int selected = (int)_selectedRadio.Tag;
            var q = _questions[_currentIndex];
            bool correct = selected == q.CorrectIndex;

            if (correct) _score++;

            // Show feedback with colour
            string feedback = correct ? "✅ Correct!" : "❌ Wrong.";
            feedback += $" Correct answer: {q.Options[q.CorrectIndex]}. {q.Explanation}";
            TblQuestion.Foreground = correct ? System.Windows.Media.Brushes.LightGreen : System.Windows.Media.Brushes.OrangeRed;
            TblQuestion.Text = feedback;

            // Disable all radio buttons
            foreach (var child in PanelAnswers.Children)
                (child as RadioButton).IsEnabled = false;

            BtnNext.IsEnabled = false;

            _currentIndex++;

            if (_currentIndex < _questions.Count)
            {
                // Show "Next" button again, but now we need to change its click to show next question
                // We'll replace the handler or just use a flag.
                BtnNext.Content = "Next Question";
                BtnNext.IsEnabled = true;
                // We'll handle the next click separately by using a flag. Easiest: use a separate method.
                // Actually, we'll just re-enable the button and handle it in the click event.
                BtnNext.Click -= BtnNext_Click; // avoid multiple subscriptions
                BtnNext.Click += BtnNext_NextQuestion;
            }
            else
            {
                BtnNext.Content = "Finish";
                BtnNext.Click -= BtnNext_Click;
                BtnNext.Click += BtnNext_Finish;
                BtnNext.IsEnabled = true;
            }
        }

        private void BtnNext_NextQuestion(object sender, RoutedEventArgs e)
        {
            // Reset UI for next question
            BtnNext.Click -= BtnNext_NextQuestion;
            BtnNext.Click += BtnNext_Click;
            BtnNext.Content = "Next";
            ShowQuestion();
        }

        private void BtnNext_Finish(object sender, RoutedEventArgs e)
        {
            EndQuiz();
        }

        private void EndQuiz()
        {
            string result = $"Quiz finished! Your score: {_score}/{_questions.Count}";
            TblQuestion.Text = result;
            PanelAnswers.Children.Clear();
            BtnNext.Visibility = Visibility.Collapsed;

            string feedback = _score >= _questions.Count * 0.7 ? "🎉 Great job! You're a cybersecurity pro!" : "📚 Keep learning to stay safe online!";
            TblQuestion.Text += $"\n\n{feedback}";

            // Log quiz completion
            ActivityLogger.Log($"Quiz completed. Score: {_score}/{_questions.Count}");
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
