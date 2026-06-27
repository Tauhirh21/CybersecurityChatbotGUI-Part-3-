// Part 3 - Task Assistant with Database 
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CybersecurityChatbotGUI.Database;
using CybersecurityChatbotGUI.Models;
using CybersecurityChatbotGUI.Services;
using CybersecurityChatbotGUI.Utils;

namespace CybersecurityChatbotGUI
{
    public partial class MainWindow : Window
    {
        // ==========================================
        // FIELDS
        // ==========================================
        private UserMemory _memory = new UserMemory();
        private string _lastTopicKey = null;
        private ObservableCollection<TaskModel> _tasks = new ObservableCollection<TaskModel>();
        private bool _isAwaitingTaskDetails = false;
        private bool _isAwaitingReminderDetails = false;

        // ==========================================
        // CONSTRUCTOR
        // ==========================================
        public MainWindow()
        {
            InitializeComponent();
            AsciiArtBlock.Text = AsciiArtHelper.GetLogo();
            AudioService.PlayGreeting();

            LoadTasks();

            try
            {
                var tasks = DbHelper.GetTasks();
                System.Diagnostics.Debug.WriteLine($"✅ Database connected! Found {tasks.Count} tasks.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Database error: {ex.Message}");
                AppendMessage("System", "⚠️ Database not available. Task feature will not work.");
            }

            AppendMessage("Bot", "Hello! What's your name?");
        }

        // ==========================================
        // TASK METHODS
        // ==========================================
        private void LoadTasks()
        {
            _tasks.Clear();
            var list = DbHelper.GetTasks();
            foreach (var task in list)
                _tasks.Add(task);
            LvTasks.ItemsSource = _tasks;
        }

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtTaskTitle.Text))
            {
                AppendMessage("System", "⚠️ Task title cannot be empty.");
                return;
            }

            var task = new TaskModel
            {
                Title = TxtTaskTitle.Text.Trim(),
                Description = TxtTaskDesc.Text?.Trim() ?? "",
                ReminderDate = DateReminder.SelectedDate,
                IsCompleted = false
            };

            DbHelper.AddTask(task);
            LoadTasks();
            ActivityLogger.Log($"Task added: '{task.Title}'");
            AppendMessage("Bot", $"✅ Task '{task.Title}' added.");

            TxtTaskTitle.Clear();
            TxtTaskDesc.Clear();
            DateReminder.SelectedDate = null;
        }

        private void BtnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (LvTasks.SelectedItem is TaskModel selected)
            {
                DbHelper.DeleteTask(selected.Id);
                LoadTasks();
                ActivityLogger.Log($"Task deleted: '{selected.Title}'");
                AppendMessage("Bot", $"🗑️ Task '{selected.Title}' deleted.");
            }
            else
            {
                AppendMessage("System", "⚠️ Please select a task to delete.");
            }
        }

        private void TaskCompleted_Changed(object sender, RoutedEventArgs e)
        {
            var cb = sender as CheckBox;
            if (cb?.Tag is int id)
            {
                var task = _tasks.FirstOrDefault(t => t.Id == id);
                if (task != null)
                {
                    task.IsCompleted = cb.IsChecked == true;
                    DbHelper.UpdateTask(task);
                    ActivityLogger.Log($"Task '{task.Title}' marked as {(task.IsCompleted ? "completed" : "incomplete")}");
                }
            }
        }

        // ==========================================
        // QUIZ METHODS
        // ==========================================
        private void BtnStartQuiz_Click(object sender, RoutedEventArgs e) => StartQuiz();

        private void StartQuiz()
        {
            var quiz = new QuizWindow();
            quiz.Owner = this;
            quiz.ShowDialog();
            ActivityLogger.Log("Quiz started by user.");
        }

        // ==========================================
        // CHAT / NLP METHODS
        // ==========================================
        private void SendButton_Click(object sender, RoutedEventArgs e) => ProcessInput();

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ProcessInput();
        }

        private void ProcessInput()
        {
            string input = UserInput.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                AppendMessage("System", "Please type something.");
                UserInput.Clear();
                return;
            }

            AppendMessage("You", input);
            UserInput.Clear();

            // Capture name first
            if (string.IsNullOrEmpty(_memory.Name))
            {
                _memory.Name = input;
                AppendMessage("Bot", $"Nice to meet you, {_memory.Name}! Ask me about passwords, scams, privacy, phishing, or safe browsing. Type 'help' for options.");
                return;
            }

            // Sentiment detection
            SentimentService.DetectAndUpdate(input, _memory);
            if (_memory.IsWorried)
            {
                AppendMessage("Bot", SentimentService.GetEmpathyResponse());
                string autoTip = ResponseService.GetRandomResponse("phishing_tips");
                AppendMessage("Bot", autoTip);
                return;
            }

            // Get intent from NLP
            string intent = KeywordService.GetIntent(input);

            switch (intent)
            {
                // TASK INTENTS
                case "ADD_TASK":
                    AppendMessage("Bot", "📝 Please provide the task title and description (e.g., 'Enable 2FA - Set up two-factor authentication').");
                    _isAwaitingTaskDetails = true;
                    break;

                case "SET_REMINDER":
                    AppendMessage("Bot", "⏰ Please tell me what you want to be reminded about and when (e.g., 'Remind me to update password in 3 days').");
                    _isAwaitingReminderDetails = true;
                    break;

                case "SHOW_TASKS":
                    LoadTasks();
                    AppendMessage("Bot", $"📋 You have {_tasks.Count} tasks. Check the Task Assistant tab for details.");
                    ActivityLogger.Log("User viewed tasks.");
                    break;

                case "DELETE_TASK":
                    AppendMessage("Bot", "🗑️ Please select a task from the Task Assistant tab and click Delete.");
                    break;

                case "COMPLETE_TASK":
                    AppendMessage("Bot", "✅ Please go to the Task Assistant tab and check the 'Done' box for the task you've completed.");
                    break;

                // QUIZ INTENT
                case "START_QUIZ":
                    StartQuiz();
                    break;

                // LOG INTENT
                case "SHOW_LOG":
                    ShowActivityLog();
                    break;

                // HELP INTENT
                case "HELP":
                    ShowHelpMenu();
                    break;

                // CYBERSECURITY TOPICS (fallback)
                default:
                    string key = KeywordService.GetResponseKey(input);
                    if (key != "default" && key != "more")
                    {
                        _lastTopicKey = key;
                        _memory.FollowUpCount = 0;
                        if (key.EndsWith("_tips") && string.IsNullOrEmpty(_memory.FavouriteTopic))
                            _memory.FavouriteTopic = key.Replace("_tips", "");

                        string response = ResponseService.GetRandomResponse(key);
                        AppendMessage("Bot", response);
                    }
                    else if (key == "more")
                    {
                        // Handle "another tip" follow-up
                        if (!string.IsNullOrEmpty(_lastTopicKey))
                        {
                            _memory.FollowUpCount++;
                            string next = ResponseService.GetNextTipForTopic(_lastTopicKey, _memory.FollowUpCount);
                            AppendMessage("Bot", next);
                        }
                        else
                        {
                            AppendMessage("Bot", "Please ask about a specific topic first (e.g., 'password').");
                        }
                    }
                    else
                    {
                        AppendMessage("Bot", ResponseService.GetRandomResponse("default"));
                    }
                    break;
            }

            // Memory recall
            if (!string.IsNullOrEmpty(_memory.FavouriteTopic) && new Random().Next(6) == 0)
            {
                string recall = ResponseService.GetRandomResponse($"{_memory.FavouriteTopic}_tips");
                AppendMessage("Bot", $"💡 Since you're interested in {_memory.FavouriteTopic}, here's a quick tip: {recall}");
            }
        }

        // ==========================================
        // LOG & HELP
        // ==========================================
        private void ShowActivityLog()
        {
            var log = ActivityLogger.GetLog();
            if (log.Count == 0)
                AppendMessage("Bot", "No activities recorded yet.");
            else
            {
                AppendMessage("Bot", "📋 Here's a summary of recent actions:");
                foreach (var entry in log)
                    AppendMessage("Bot", entry);
            }
        }

        private void ShowHelpMenu()
        {
            string help = @"
📚 HELP MENU 📚

• 'password' - Password safety tips
• 'phishing' - Phishing awareness
• 'privacy' - Privacy tips
• 'scam' - Scam awareness
• 'safe browsing' - Safe browsing tips
• 'add task' - Add a cybersecurity task
• 'remind me' - Set a reminder
• 'show tasks' - View your tasks
• 'start quiz' - Take the cybersecurity quiz
• 'activity log' - View recent actions
• 'another tip' - Get another tip on same topic"; 

            AppendMessage("Bot", help);
        }

        private void AppendMessage(string sender, string msg)
        {
            ChatHistory.Items.Add($"[{DateTime.Now:HH:mm:ss}] {sender}: {msg}");
            ChatHistory.ScrollIntoView(ChatHistory.Items[ChatHistory.Items.Count - 1]);
        }
    }
}
