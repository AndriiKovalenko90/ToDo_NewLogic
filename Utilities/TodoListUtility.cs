using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using ToDo_NewLogic.Managers;
using ToDo_NewLogic.Models;
using ToDo_NewLogic.Views;

namespace ToDo_NewLogic.Utilities
{
    public class TodoListUtility
    {
        private readonly TodoListManager todoListManager;
        private readonly TaskManager taskManager;
        private List<string> recentTodoLists;
        public List<string> RecentTodoLists => recentTodoLists;

        public event EventHandler<TodoList> NewTodoListCreated;
        public TodoListUtility(TodoListManager todoListManager, TaskManager taskManager, List<string> recentTodoLists)
        {
            this.todoListManager = todoListManager;
            this.taskManager = taskManager;
            this.recentTodoLists = recentTodoLists;
        }

        public void CreateNewTodoList()
        {
            var createNewTodoListWindow = new CreateNewTodoList();
            createNewTodoListWindow.ShowDialog();

            if (createNewTodoListWindow.DialogResult == true)
            {
                string todoListTitle = createNewTodoListWindow.TodoListTitle;
                string filePath = $"{todoListTitle}.json";

                TodoList newTodoList = new TodoList(todoListTitle, filePath);
                todoListManager.AddTodoList(newTodoList);
                OnNewTodoListCreated(newTodoList);
            }
        }

        protected void OnNewTodoListCreated(TodoList todoList)
        {
            NewTodoListCreated?.Invoke(this, todoList);
        }

        public List<string> GetRecentTodoListTitles()
        {
            return recentTodoLists;
        }
    }
}
