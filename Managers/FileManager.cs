using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

using ToDo_NewLogic.Models;

namespace ToDo_NewLogic.Managers
{
    internal class FileManager
    {
        public List<Task> LoadTasksFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var tasks = JsonConvert.DeserializeObject<List<Task>>(json);
            return tasks;
        }

        public void SaveTasksToFile(string filePath, List<Task> tasks)
        {
            string json = JsonConvert.SerializeObject(tasks);
            File.WriteAllText(filePath, json);
        }

        public List<TodoList> LoadTodoListsFromFile(string filePath)
        {
            List<TodoList> todoLists= new List<TodoList>();
            if (File.Exists(filePath))
            {
                string jsonTodoLists = File.ReadAllText(filePath);
                todoLists = JsonConvert.DeserializeObject<List<TodoList>>(jsonTodoLists);
            }
            return todoLists;
        }

        public void SaveTodoListsToFile(string filePath, List<TodoList> todoLists)
        {
            string jsonTodoLists = JsonConvert.SerializeObject(todoLists);
            File.WriteAllText(filePath, jsonTodoLists);
        }
    }
}
