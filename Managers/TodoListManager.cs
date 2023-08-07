using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
            var todolist = _todoLists.FirstOrDefault(t => t.Title == todoListTitle);
            if (todolist != null)
            {
                return todolist;
            }

            MessageBox.Show("TodoList not found");
            return null;
            // what to do in case not found
        }

        public void AddTask(TodoList selectedList, Task task)
        {
            _taskManager.AddTask(selectedList, task);
        }

        public void RemoveTask(TodoList selectedList, Task task)
        {
            _taskManager.RemoveTask(selectedList, task);
        }

        public void CompleteTask(TodoList selectedList, Task task)
        {
            _taskManager.MarkAsCompleted(selectedList, task);
        }
    }
}
