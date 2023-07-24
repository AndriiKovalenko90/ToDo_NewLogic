using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

using ToDo_NewLogic.Models;

namespace ToDo_NewLogic.Managers
{
    internal class FileManager
    {
        public BindingList<Task> LoadTasksFromFile(string filePath)
        {
            //Loadtasks from the specified file (filePath and return tham)
            string json = File.ReadAllText(filePath);
            BindingList<Task> tasks = JsonConvert.DeserializeObject<BindingList<Task>>(json);
            return tasks;
        }

        public void SaveTasksToFile(string filePath, BindingList<Task> tasks)
        {
            // Save the provided tasks to the specified file(filePath)
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
