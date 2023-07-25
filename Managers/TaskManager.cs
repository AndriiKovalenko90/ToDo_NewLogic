using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ToDo_NewLogic.Models;

namespace ToDo_NewLogic.Managers
{
    public class TaskManager
    {
        public BindingList<Task> Tasks { get; set; }

        public TaskManager()
        {
            Tasks = new BindingList<Task>();
        }

        public void LoadTasks(string filePath)
        {
            FileManager fileManager = new FileManager();
            Tasks = fileManager.LoadTasksFromFile(filePath);

        }

        public void SaveTasks(string filePath)
        {
            FileManager fileManager = new FileManager();
            fileManager.SaveTasksToFile(filePath, Tasks);
        }

        public void AddTask(Task task)
        {
            Tasks.Add(task);
        }

        public void RemoveTask(Task task)
        {
            Tasks.Remove(task);
        }
    }
}
