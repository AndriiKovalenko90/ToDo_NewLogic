using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using ToDo_NewLogic.Models;


namespace ToDo_NewLogic.Helpers
{
    public class IOHelper
    {
        private string filePath;

        public IOHelper(string filePath)
        {
            this.filePath = filePath;
        }

        public void SaveTodoLists(List<TodoList> todoLists)
        {
            string jsonContent = JsonConvert.SerializeObject(todoLists, Formatting.Indented);
            File.WriteAllText(filePath, jsonContent);
        }

        public List<TodoList> LoadTodoLists()
        {
            if (File.Exists(filePath))
            {
                string jsonContent = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<TodoList>>(jsonContent);
            }

            return new List<TodoList>();
        }
    }
}
