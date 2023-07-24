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
using ToDo_NewLogic.Models;
using ToDo_NewLogic.Views;

namespace ToDo_NewLogic
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool hasUnsavedChanges = false; //TO TRACK THE DATAGRID UPD
        //private List<TodoList> todoLists = new List<TodoList>(); // Declare the todoLists variable at the class level

        //private List<string> recentTodoLists = new List<string>();
        //private TodoList activeTodoList = null;
        ////private BindingList<Task> Tasks = new BindingList<Task>();
        //private TaskManager taskManager;

        private TodoListManager todoListManager;
        private TaskManager taskManager;
        private FileManager fileManager = new FileManager();
        private TodoList activeTodoList = null;
        private List<TodoList> todoLists;
        private List<string> recentTodoLists;

        public MainWindow()
        {
            InitializeComponent();

            todoListManager = new TodoListManager();
            taskManager = new TaskManager();

            //todoLists = todoListManager.LoadTodoLists();

            //if (RecentLists.SelectedItem != null)
            //{
            //    string selectedTodoListTitle = RecentLists.SelectedItem.ToString();
            //    TodoList selectedTodoList = todoListManager.GetTodoListByTitle(selectedTodoListTitle);

            //    if (selectedTodoList != null)
            //    {
            //        taskManager.LoadTasks(selectedTodoList.FilePath);
            //        TasksDataGrid.ItemsSource = taskManager.Tasks;
            //    }
            //}

            //// Populate the RecentLists ListBox with the titles of the loaded todo lists
            //todoLists = 
            //recentTodoLists = todoLists.Select(todoList => todoList.Title).ToList();
            //RecentLists.Items.Clear();

            //if (recentTodoLists.Any())
            //{
            //    RecentLists.ItemsSource = recentTodoLists;
            //}

            //taskManager = new TaskManager();
            


            //// Subscribe to DataGrid events here
            //TasksDataGrid.BeginningEdit += TasksDataGrid_BeginningEdit;
            //TasksDataGrid.CellEditEnding += TasksDataGrid_CellEditEnding;

            //BtnSave.IsEnabled = false;
        }

        


        //MAIN MENU BUTTONS
        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            var createNewTodoListWindow = new CreateNewTodoList();
            createNewTodoListWindow.ShowDialog();

            if (createNewTodoListWindow.DialogResult == true)
            {
                string todoListTitle = createNewTodoListWindow.TodoListTitle;
                string filePath = $"{todoListTitle}.json";


                TodoList newTodoList = new TodoList(todoListTitle, filePath);
                todoListManager.AddTodoList(newTodoList);

                //IOHelper ioHelper = new IOHelper("todoLists.json");
                //ioHelper.SaveTodoLists(todoLists);

                recentTodoLists.Insert(0, todoListTitle);
                //RecentLists.ItemsSource = null;
                //RecentLists.ItemsSource = recentTodoLists;


                ContentMain.Visibility = Visibility.Visible;
                

                activeTodoList = newTodoList;
                taskManager.Tasks = new BindingList<Task>();
                SaveTasksForActiveTodoList();
                RefreshDataGrid();
                UpdateRecentLists();

                RecentLists.SelectedItem = todoListTitle;
            }
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
                    //TasksDataGrid.ItemsSource = loadedTasks;

                    string openedTodoListTitle = Path.GetFileNameWithoutExtension(fileDialog.FileName);
                    TodoList existingTodoList = todoLists.FirstOrDefault(todoList => todoList.Title == openedTodoListTitle);

                    if (existingTodoList != null)
                    {
                        // Todo list with the same title already exists, update its file path
                        existingTodoList.FilePath = fileDialog.FileName;
                    }
                    else
                    {
                        // Todo list does not exist, create a new entry in the todoLists
                        string filePath = fileDialog.FileName;
                        TodoList newTodoList = new TodoList(openedTodoListTitle, filePath);
                        todoLists.Insert(0, newTodoList);
                    }

                    IOHelper ioHelper = new IOHelper("todoLists.json");
                    ioHelper.SaveTodoLists(todoLists);

                    recentTodoLists.Insert(0, openedTodoListTitle);
                    RecentLists.ItemsSource = null;
                    RecentLists.ItemsSource = recentTodoLists;

                    ContentMain.Visibility = Visibility.Visible;
                    RecentLists.SelectedItem = openedTodoListTitle;

                    activeTodoList = existingTodoList ?? todoLists.FirstOrDefault(todoList => todoList.Title == openedTodoListTitle);

                    
                    taskManager.Tasks = loadedTasks; // Update the TaskManager tasks with the loaded tasks
                    TasksDataGrid.ItemsSource = taskManager.Tasks; // Update the DataGrid to show the tasks of the opened todo list
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
                // Ask the user if they want to save changes before switching to another list
                MessageBoxResult result = MessageBox.Show("Do you want to save changes?", "Save Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // Implement the method to save changes made in the DataGrid
                        SaveChanges();
                        break;
                    case MessageBoxResult.No:

                        break;
                    case MessageBoxResult.Cancel:
                        // Cancel the selection change and stay on the current list
                        
                        return;
                }
            }
        }

        private void BtnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (activeTodoList != null)
            {
                // Show a dialog for the user to enter a new title for the TodoList
                //var saveAsDialog = new SaveAsDialog();
                var saveAsDialog = new SaveFileDialog();
                saveAsDialog.Filter = "JSON files (*.json)|*.json";
                saveAsDialog.DefaultExt = ".json";
                bool? result = saveAsDialog.ShowDialog();


                if (result == true)
                {
                    // Get the new title entered by the user
                    string newTodoListTitle = Path.GetFileNameWithoutExtension(saveAsDialog.FileName);

                    // Create a new file path for the TodoList using the new title
                    string newFilePath = $"{newTodoListTitle}.json";

                    // Create a new TodoList instance with the new title and file path
                    TodoList newTodoList = new TodoList(newTodoListTitle, newFilePath);

                    // Copy the tasks from the current active TodoList to the new TodoList

                    foreach (Task task in activeTodoList.TaskManager.Tasks)
                    {
                        newTodoList.TaskManager.AddTask(new Task(task.Id, task.Title, task.Description, task.Priority, task.IsCompleted, task.DueDate));
                    }

                    // Add the new TodoList to the todoLists list
                    todoLists.Insert(0, newTodoList);

                    // Save the updated todoLists list
                    IOHelper ioHelper = new IOHelper("todoLists.json");
                    ioHelper.SaveTodoLists(todoLists);

                    // Update the recentTodoLists collection with the new title
                    recentTodoLists.Insert(0, newTodoListTitle);
                    RecentLists.ItemsSource = null;
                    RecentLists.ItemsSource = recentTodoLists;

                    // Update the activeTodoList to the new TodoList
                    activeTodoList = newTodoList;

                    // Save the tasks of the new TodoList to the new file
                    SaveTasksForActiveTodoList();

                    SaveChanges();

                    // Refresh the DataGrid to show the tasks of the new TodoList
                    RefreshDataGrid();

                    // Update the RecentLists selection to the new TodoList
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
                hasUnsavedChanges = true; // Indicate unsaved changes after adding a new task
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
                // Toggle the IsCompleted property of the task
                task.IsCompleted = !task.IsCompleted;

                // Update the content of the ToggleButton based on the new IsCompleted value
                toggleButton.Content = task.IsCompleted ? "Uncomplete" : "Complete";

                // If you want to update the background color of the row when clicking the button:
                RefreshDataGrid();
                hasUnsavedChanges = true; 
                BtnSave.IsEnabled = true;
            }
        }

        private void BtnActionRemoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Task taskItem)
            {
                // Show a confirmation dialog box
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




        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    _ioHelper = new IOHelper(PATH);

        //    try
        //    {
        //        _todoDataList = _ioHelper.LoadData();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        Close();
        //    }

        //    TasksDataGrid.ItemsSource = _todoDataList;
        //    _todoDataList.ListChanged += _todoDataList_ListChanged;

        //}

        //private void _todoDataList_ListChanged(object sender, ListChangedEventArgs e)
        //{
        //    if (e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted || e.ListChangedType == ListChangedType.ItemChanged)
        //    {
        //        try
        //        {
        //            _ioHelper.SaveTasksToFile(sender);
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show(ex.Message);
        //            Close();
        //        }
        //    }
        //}
        private void TasksDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            hasUnsavedChanges = true;
        }

        // Event handler for the DataGrid when the user finishes editing a cell
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
                    // Ask the user if they want to save changes before switching to another list
                    MessageBoxResult result = MessageBox.Show("Do you want to save changes?", "Save Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            SaveChanges(); // Implement the method to save changes made in the DataGrid
                            break;
                        case MessageBoxResult.No:
                            // Continue without saving changes
                            hasUnsavedChanges = false; // Set the flag to false only after actually saving changes
                            BtnSave.IsEnabled = false;
                            break;
                    }
                }

                string selectedTodoListTitle = RecentLists.SelectedItem.ToString();
                activeTodoList = todoLists.FirstOrDefault(todoList => todoList.Title == selectedTodoListTitle);

                if (activeTodoList != null)
                {
                    LoadTasksForActiveTodoList();
                    //TasksDataGrid.ItemsSource = taskManager.Tasks;
                    ContentMain.Visibility = Visibility.Visible;
                    RecentLists.SelectedItem = activeTodoList.Title;
                }
                else
                {
                    //TasksDataGrid.ItemsSource = null;
                    //ContentMain.Visibility = Visibility.Collapsed;
                    MessageBox.Show("Selected todo list not found.");
                }
            }
        }


        private void RefreshDataGrid()
        {
            TasksDataGrid.ItemsSource = GetActiveTodoListTasks();

        }

        private void SaveChanges()
        {
            if (activeTodoList != null)
            {
                // Save the tasks to the separate JSON file for the active todo list
                SaveTasksForActiveTodoList();

                // Save the todoLists list
                IOHelper ioHelper = new IOHelper("todoLists.json");
                ioHelper.SaveTodoLists(todoLists);

                // After saving, set hasUnsavedChanges to false and disable the BtnSave button.
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

        private void UpdateRecentLists()
        {
            // Filter the recentTodoLists to include only those that have corresponding files
            recentTodoLists = recentTodoLists
                .Where(title => todoLists.Any(todoList => todoList.Title == title && File.Exists(todoList.FilePath)))
                .ToList();

            // Update the ListBox with the filtered recentTodoLists
            RecentLists.ItemsSource = null;
            RecentLists.ItemsSource = recentTodoLists;
        }

        private void CloseTodoList()
        {
            if (hasUnsavedChanges)
            {
                // Ask the user if they want to save changes before closing the list
                MessageBoxResult result = MessageBox.Show("Do you want to save changes?", "Save Changes", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // Save the changes and close the list
                        SaveChanges();
                        break;
                    case MessageBoxResult.No:
                        // Close the list without saving, do not reset the flag
                        break;
                    //case MessageBoxResult.Cancel:
                    //    Cancel the Close operation and stay on the current list
                    //    return;
                }
            }

            TasksDataGrid.ItemsSource = null;
            activeTodoList = null;
            ContentMain.Visibility = Visibility.Collapsed;
            hasUnsavedChanges = false;
            BtnSave.IsEnabled = false;

            RecentLists.SelectedItem = null;
        }
    }
}
