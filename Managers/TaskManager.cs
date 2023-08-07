using System.Collections.Generic;
using System.Linq;
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

        public void AddTask(TodoList activeTodoList, Task task)
        {
            activeTodoList.Tasks.Add(task);
        }

        public void RemoveTask(TodoList activeTodoList, Task task)
        {
            activeTodoList.Tasks.Remove(task);
        }

        public void MarkAsCompleted(TodoList activeTodoList, Task task)
        {
            activeTodoList.Tasks.First(t => t.Id == task.Id).IsCompleted = true;
        }
    }
}
