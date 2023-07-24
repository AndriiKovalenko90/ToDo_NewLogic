using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_NewLogic.Models;

namespace ToDo_NewLogic.Managers
{
    internal class TodoListManager
    {
        private List<TodoList> todoLists = new List<TodoList>();

        public List<string> GetRecentTodoLists()
        {
            return todoLists.Select(x => x.Title).ToList();
        }

        public void LoadTodoLists()
        {
            FileManager fileManager = new FileManager();
            todoLists = fileManager.LoadTodoListsFromFile("todoLists.json");
            
        }

        public void SaveTodoLists()
        {
            FileManager filemanager = new FileManager();
            filemanager.SaveTodoListsToFile("todoLists.json", todoLists);
        }
    }
}
