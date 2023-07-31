using System.Collections.Generic;
using ToDo_NewLogic.Models;

namespace ToDo_NewLogic.Managers
{
    public class TaskManager
    {
        public List<Task> Tasks { get; private set; }

        public TaskManager()
        {
        }

        public List<Task> GetTasks()
        {
            return Tasks;
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
