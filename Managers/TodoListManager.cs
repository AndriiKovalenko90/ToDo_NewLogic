using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDo_NewLogic.Models;

namespace ToDo_NewLogic.Managers
{
    public class TodoListManager
    {
        private List<TodoList> todoLists = new List<TodoList>();
        private FileManager fileManager = new FileManager();

        public List<string> GetRecentTodoLists()
        {
            return todoLists.Select(x => x.Title).ToList();
        }

        public List<TodoList> GetTodoLists()
        {
            return fileManager.LoadTodoListsFromFile("todoLists.json");
        }

        public void SetTodoLists(List<TodoList> todoLists)
        {
            fileManager.SaveTodoListsToFile("todoLists.json", todoLists);
        }

        public void AddTodoList(TodoList t)
        {
            todoLists.Insert(0, t);
        }

        public void RemoveTodoList(TodoList t)
        {
            todoLists.Remove(t);
        }

        public TodoList GetTodoListByTitle(string todoListTitle)
        {
            return todoLists.Find(x => x.Title== todoListTitle);
        }


    }
}
