using Microsoft.VisualBasic;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using ToDo_NewLogic.Helpers;
using ToDo_NewLogic.Managers;
using ToDo_NewLogic.Utilities;
using ToDo_NewLogic.Models;
using ToDo_NewLogic.Views;
using static ToDo_NewLogic.Utilities.TodoListUtility;

namespace ToDo_NewLogic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool hasUnsavedChanges = false; //TO TRACK THE DATAGRID UPD
        private TodoListManager todoListManager = new TodoListManager();
        private TaskManager taskManager = new TaskManager();
        private TodoList activeTodoList = null;
        private List<TodoList> todoLists;
        private List<string> recentTodoLists = null;
        private TodoListUtility todoListUtility;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();
            UpdateRecentLists();
            LoadActiveTodoList();
            RefreshDataGrid();

            todoListUtility = new TodoListUtility(todoListManager, taskManager, recentTodoLists);
            todoListUtility.NewTodoListCreated += TodoListUtility_NewTodoListCreated;

        }

        private void InitializeData()
        {
            todoLists = todoListManager.LoadTodoLists();
            taskManager.Tasks = new BindingList<Task>();
            TasksDataGrid.ItemsSource = taskManager.Tasks;

            recentTodoLists = todoLists.Select(todoList => todoList.Title).ToList();
            RecentLists.Items.Clear();
            if (recentTodoLists.Any())
            {
                RecentLists.ItemsSource = recentTodoLists;
            }

            // Subscribe to DataGrid events
            TasksDataGrid.BeginningEdit += TasksDataGrid_BeginningEdit;
            TasksDataGrid.CellEditEnding += TasksDataGrid_CellEditEnding;

            BtnSave.IsEnabled = false;
        }

        private void UpdateRecentLists()
        {
            recentTodoLists = recentTodoLists
                .Where(title => todoLists.Any(todoList => todoList.Title == title && File.Exists(todoList.FilePath)))
                .ToList();

            RecentLists.ItemsSource = null;
            RecentLists.ItemsSource = recentTodoLists;
        }

        private void LoadActiveTodoList()
        {
            if (RecentLists.SelectedItem != null)
            {
                string selectedTodoListTitle = RecentLists.SelectedItem.ToString();
                activeTodoList = todoListManager.GetTodoListByTitle(selectedTodoListTitle);

                if (activeTodoList != null)
                {
                    taskManager.LoadTasks(activeTodoList.FilePath);
                    TasksDataGrid.ItemsSource = taskManager.Tasks;
                }
                else
                {
                    MessageBox.Show("Selected todo list not found.");
                }
            }
        }
        private void RefreshDataGrid()
        {
            TasksDataGrid.ItemsSource = GetActiveTodoListTasks();
        }


        //MAIN MENU BUTTONS
        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            todoListUtility.CreateNewTodoList();
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog= new OpenFileDialog();
            fileDialog.Filter = "JSON files (Tasks) | *.json";
            fileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            fileDialog.Title = "Please select a proper json file which consist of Tasks.";

            bool? success = fileDialog.ShowDialog();
            if (success == true)
            {
                try
                {
                    string jsonContent = File.ReadAllText(fileDialog.FileName);
                    BindingList<Task> loadedTasks = JsonConvert.DeserializeObject<BindingList<Task>>(jsonContent);

                    string openedTodoListTitle = Path.GetFileNameWithoutExtension(fileDialog.FileName);
                    TodoList existingTodoList = todoLists.FirstOrDefault(todoList => todoList.Title == openedTodoListTitle);

                    if (existingTodoList != null)
                    {
                        existingTodoList.FilePath = fileDialog.FileName;
                    }
                    else
                    {
                        string filePath = fileDialog.FileName;
                        TodoList newTodoList = new TodoList(openedTodoListTitle, filePath);
                        todoLists.Insert(0, newTodoList);
                    }

                    recentTodoLists.Insert(0, openedTodoListTitle);
                    RecentLists.ItemsSource = null;
                    RecentLists.ItemsSource = recentTodoLists;

                    ContentMain.Visibility = Visibility.Visible;
                    RecentLists.SelectedItem = openedTodoListTitle;

                    activeTodoList = existingTodoList ?? todoLists.FirstOrDefault(todoList => todoList.Title == openedTodoListTitle);

                    taskManager.Tasks = loadedTasks;
                    TasksDataGrid.ItemsSource = taskManager.Tasks; 
                    UpdateRecentLists();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading tasks: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("No file selected.");
            }


        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (hasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes?", "Save Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveChanges();
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }

        private void BtnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (activeTodoList != null)
            {
                var saveAsDialog = new SaveFileDialog();
                saveAsDialog.Filter = "JSON files (*.json)|*.json";
                saveAsDialog.DefaultExt = ".json";
                bool? result = saveAsDialog.ShowDialog();


                if (result == true)
                {
                    string newTodoListTitle = Path.GetFileNameWithoutExtension(saveAsDialog.FileName);
                    string newFilePath = $"{newTodoListTitle}.json";
                    TodoList newTodoList = new TodoList(newTodoListTitle, newFilePath);

                    foreach (Task task in activeTodoList.TaskManager.Tasks)
                    {
                        newTodoList.TaskManager.AddTask(new Task(task.Id, task.Title, task.Description, task.Priority, task.IsCompleted, task.DueDate));
                    }

                    todoLists.Insert(0, newTodoList);
                    recentTodoLists.Insert(0, newTodoListTitle);
                    RecentLists.ItemsSource = null;
                    RecentLists.ItemsSource = recentTodoLists;
                    activeTodoList = newTodoList;
                    SaveTasksForActiveTodoList();
                    SaveChanges();
                    RefreshDataGrid();
                    RecentLists.SelectedItem = newTodoListTitle;

                    
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseTodoList();
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        //Additional buttons
        private void AddTodoListItem_Click(object sender, RoutedEventArgs e)
        {
            AddItemModal addItemModal = new AddItemModal();
            bool? result = addItemModal.ShowDialog();
            
            if (result == true)
            {
                Task newItem = addItemModal.NewItem;
                taskManager.AddTask(newItem);
                RefreshDataGrid();
                hasUnsavedChanges = true; 
                BtnSave.IsEnabled = true;
            }
        }

        private void AddItemModal_Closed(object sender, EventArgs e)
        {

            AddItemModal addItemModal = (AddItemModal)sender;
            if (addItemModal.DialogResult == true) 
            {
                Task newItem = addItemModal.NewItem;
            }
            addItemModal.Closed -= AddItemModal_Closed;
        }

        private void BtnActionCompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton && toggleButton.DataContext is Task task)
            {
                task.IsCompleted = !task.IsCompleted;
                toggleButton.Content = task.IsCompleted ? "Uncomplete" : "Complete";

                RefreshDataGrid();
                hasUnsavedChanges = true; 
                BtnSave.IsEnabled = true;
            }
        }

        private void BtnActionRemoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Task taskItem)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to remove this item?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    GetActiveTodoListTasks().Remove(taskItem);
                    RefreshDataGrid();
                    hasUnsavedChanges = true;
                    BtnSave.IsEnabled = true;
                }
            }
        }



        private void TasksDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            hasUnsavedChanges = true;
        }
        private void TasksDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            hasUnsavedChanges = true;
        }

        private void RecentLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecentLists.SelectedItem != null)
            {

                if (hasUnsavedChanges)
                {
                    MessageBoxResult result = MessageBox.Show("Do you want to save changes?", "Save Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            SaveChanges(); 
                            break;
                        case MessageBoxResult.No:
                            hasUnsavedChanges = false;
                            BtnSave.IsEnabled = false;
                            break;
                    }
                }

                string selectedTodoListTitle = RecentLists.SelectedItem.ToString();
                activeTodoList = todoLists.FirstOrDefault(todoList => todoList.Title == selectedTodoListTitle);

                if (activeTodoList != null)
                {
                    LoadTasksForActiveTodoList();
                    ContentMain.Visibility = Visibility.Visible;
                    RecentLists.SelectedItem = activeTodoList.Title;
                }
                else
                {
                    MessageBox.Show("Selected todo list not found.");
                }
            }
        }




        private void SaveChanges()
        {
            if (activeTodoList != null)
            {
                taskManager.SaveTasks(activeTodoList.FilePath);
                todoListManager.SaveTodoLists(todoLists);
                hasUnsavedChanges = false;
                BtnSave.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Selected todo list not found.");
            }
        }

        private BindingList<Task> GetActiveTodoListTasks()
        {
            return taskManager.Tasks ?? new BindingList<Task>();
        }

        private void LoadTasksForActiveTodoList()
        {
            if (activeTodoList != null)
            {
                if (File.Exists(activeTodoList.FilePath))
                {
                    string jsonContent = File.ReadAllText(activeTodoList.FilePath);
                    BindingList<Task> loadedTasks = JsonConvert.DeserializeObject<BindingList<Task>>(jsonContent);
                    taskManager.Tasks = loadedTasks;
                    TasksDataGrid.ItemsSource = taskManager.Tasks;
                }

            }
        }

        private void SaveTasksForActiveTodoList()
        {
            if (activeTodoList != null)
            {
                string jsonContent = JsonConvert.SerializeObject(taskManager.Tasks);
                File.WriteAllText(activeTodoList.FilePath, jsonContent);
            }
        }



        private void CloseTodoList()
        {
            if (hasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes?", "Save Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveChanges();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }

            TasksDataGrid.ItemsSource = null;
            activeTodoList = null;
            ContentMain.Visibility = Visibility.Collapsed;
            hasUnsavedChanges = false;
            BtnSave.IsEnabled = false;

            RecentLists.SelectedItem = null;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            todoListUtility.NewTodoListCreated += TodoListUtility_NewTodoListCreated;
        }

        private void TodoListUtility_NewTodoListCreated(object sender, TodoList e)
        {
            TodoList newTodoList = e;
            recentTodoLists.Insert(0, newTodoList.Title);
            ContentMain.Visibility = Visibility.Visible;
            activeTodoList = newTodoList;
            todoLists.Insert(0, newTodoList);
            taskManager.Tasks = new BindingList<Task>();
            SaveTasksForActiveTodoList();
            RefreshDataGrid();
            UpdateRecentLists();
        }
    }
}
