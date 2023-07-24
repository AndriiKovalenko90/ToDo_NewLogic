using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_NewLogic.Managers;

namespace ToDo_NewLogic.Models
{
    public class TodoList
    {
        public string Title { get; set; }
        public string FilePath { get; set; }
        public TaskManager TaskManager { get; }

        public TodoList(string title, string filePath)
        {
            Title = title;
            FilePath = filePath;
            TaskManager = new TaskManager();
        }
    }
}
