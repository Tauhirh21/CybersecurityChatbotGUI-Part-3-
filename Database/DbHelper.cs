using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using CybersecurityChatbotGUI.Models;

namespace CybersecurityChatbotGUI.Database
{
    public static class DbHelper
    {
        // FIXED: (localdb) not (localhost)
        private static string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CybersecurityBot;Integrated Security=True";

        public static List<TaskModel> GetTasks()
        {
            List<TaskModel> tasks = new List<TaskModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM Tasks ORDER BY Id DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TaskModel task = new TaskModel();
                                task.Id = reader.GetInt32(0);
                                task.Title = reader.GetString(1);
                                task.Description = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                task.ReminderDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);
                                task.IsCompleted = reader.GetBoolean(4);
                                tasks.Add(task);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
            }

            return tasks;
        }

        public static void AddTask(TaskModel task)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Tasks (Title, Description, ReminderDate, IsCompleted) VALUES (@title, @desc, @reminder, @completed)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", task.Title);
                        cmd.Parameters.AddWithValue("@desc", task.Description ?? "");
                        cmd.Parameters.AddWithValue("@reminder", task.ReminderDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@completed", task.IsCompleted);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddTask error: {ex.Message}");
            }
        }

        public static void UpdateTask(TaskModel task)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE Tasks SET Title=@title, Description=@desc, ReminderDate=@reminder, IsCompleted=@completed WHERE Id=@id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", task.Id);
                        cmd.Parameters.AddWithValue("@title", task.Title);
                        cmd.Parameters.AddWithValue("@desc", task.Description ?? "");
                        cmd.Parameters.AddWithValue("@reminder", task.ReminderDate ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@completed", task.IsCompleted);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateTask error: {ex.Message}");
            }
        }

        public static void DeleteTask(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "DELETE FROM Tasks WHERE Id=@id";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteTask error: {ex.Message}");
            }
        }
    }
}