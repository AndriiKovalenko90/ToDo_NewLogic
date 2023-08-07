using System;
using System.Collections.Generic;

namespace ToDo_NewLogic.Models
{
    public class TodoList
    {
        public string Title { get; set; }
        public DateTime AddedTimestamp { get; set; } = DateTime.UtcNow;
        public string FilePath { get; set; }
        public List<Task> Tasks { get; set; }

        public TodoList(string title, string filePath, List<Task> tasks)
        {
            Title = title;
            FilePath = filePath;
            Tasks = tasks;
        }
    }
}
