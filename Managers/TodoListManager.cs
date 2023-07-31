using System.Collections.Generic;
using System.Linq;
using ToDo_NewLogic.Models;

namespace ToDo_NewLogic.Managers
{
    public class TodoListManager
    {
        private readonly FileManager _fileManager = new FileManager();
        private readonly TaskManager _taskManager = new TaskManager();

        private List<TodoList> _todoLists = new List<TodoList>();
        
        // private active string activeTodoListTitle

        public List<string> GetRecentTodoLists()
        {
            return _todoLists.Select(x => x.Title).ToList();
        }

        public List<TodoList> LoadToDoLists()
        {
            _todoLists = _fileManager.LoadTodoListsFromFile("todoLists.json");
            return _todoLists;
        }

        public void SaveTodoLists()
        {
            _fileManager.SaveTodoListsToFile("todoLists.json", _todoLists);
        }

        public void AddTodoList(TodoList t)
        {
            _todoLists.Add(t);
        }

        public void RemoveTodoList(TodoList todoList)
        {
            _todoLists.RemoveAll(t => t.Title == todoList.Title);
        }

        public TodoList GetTodoListByTitle(string todoListTitle)
        {
            return _todoLists.FirstOrDefault(t => t.Title == todoListTitle);
            // what to do in case not found
        }

        public void AddTask(Task task)
        {
            var activeTodoList = _todoLists.FirstOrDefault(t => t.Title == activeTodoListTitle);
            _taskManager.AddTask(activeTodoList, task);
        }
    }
}
