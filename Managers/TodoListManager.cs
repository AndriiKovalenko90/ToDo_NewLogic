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
        

        public List<TodoList> GetTodoLists()
        {
            return _todoLists.ToList();
        }

        public List<TodoList> GetRecentTodoLists(int count)
        {
            return _todoLists.OrderByDescending(todoList => todoList.AddedTimestamp).Take(count).ToList();
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

        public void AddTodoList(TodoList todoList)
        {
            _todoLists.Add(todoList);
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
            _taskManager.AddTask(task);
        }

        public void RemoveTask(Task task)
        {
            _taskManager.RemoveTask(task);
        }
    }
}
