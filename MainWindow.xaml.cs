using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        private bool _hasUnsavedChanges = false; //TO TRACK THE DATAGRID UPD

        private readonly TodoListManager _todoListManager = new TodoListManager();
        private readonly string _todoListsFilePath = "todolists.json";


        public MainWindow()
        {
            InitializeComponent();

            if (File.Exists(_todoListsFilePath))
            {
                // Load TodoLists from the file
                _todoListManager.LoadToDoLists();

                // Display the recent TodoLists in the left navigation panel
                List<TodoList> recentTodoLists = _todoListManager.GetRecentTodoLists(9); // Displaying 5 most recent TodoLists
                
                foreach (TodoList todoList in recentTodoLists)
                {
                    RecentLists.Items.Add(todoList.Title);
                }
                RecentListsLabel.Visibility = Visibility.Visible;
                RecentLists.Visibility = Visibility.Visible;
                // Hide the message
                NoTodoListsMessage.Visibility = Visibility.Collapsed;
            }
            else
            {
                // Hide the RecentLists ListBox
                RecentListsLabel.Visibility = Visibility.Collapsed;
                RecentLists.Visibility = Visibility.Collapsed;
                // Show the message
                NoTodoListsMessage.Visibility = Visibility.Visible;
            }

            ((INotifyCollectionChanged)RecentLists.Items).CollectionChanged += RecentLists_CollectionChanged;
            RecentLists.SelectionChanged += RecentLists_SelectionChanged;
            TasksDataGrid.BeginningEdit += TasksDataGrid_BeginningEdit;
            TasksDataGrid.CellEditEnding += TasksDataGrid_CellEditEnding;
        }

        private void RecentLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RecentLists.SelectedItem != null)
            {
                // Get the title of the selected TodoList
                string selectedTodoListTitle = RecentLists.SelectedItem.ToString();
                _hasUnsavedChanges = true;
                // Use the title to get the corresponding TodoList from the TodoListManager
                TodoList selectedTodoList = _todoListManager.GetTodoListByTitle(selectedTodoListTitle);

                ContentMain.Visibility = Visibility.Visible;
                BtnSaveAs.IsEnabled = true;
                TasksDataGrid.ItemsSource = null;
                TasksDataGrid.ItemsSource = selectedTodoList.Tasks;
            }
        }

        private void RecentLists_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RecentListsLabel.Visibility = Visibility.Visible;
            RecentLists.Visibility = Visibility.Visible;
            NoTodoListsMessage.Visibility = Visibility.Collapsed;

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // Get the newly added item
                TodoList newTodoList = e.NewItems[0] as TodoList;

                // Update the UI with the newly added TodoList
                // For example, you can add the TodoList to the ListBox or update any other UI element
                // For demonstration purposes, let's assume you have a ListBox named "RecentListBox" to display recent TodoLists
                // Make sure to bind the ListBox to RecentLists in XAML
                if (newTodoList != null)
                {
                    foreach (TodoList todoList in _todoListManager.GetRecentTodoLists(9))
                    {
                        RecentLists.Items.Add(todoList.Title);
                    }

                }
            }
        }




        //MAIN MENU BUTTONS
        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            var createTodoListDialog = new CreateNewTodoList();
            if (createTodoListDialog.ShowDialog() == true)
            {
                string todoListName = createTodoListDialog.TodoListTitle;

                // Create a new TodoList with the provided name
                TodoList newTodoList = new TodoList(todoListName, $"TodoLists/{todoListName}.json", new List<Task>());

                string directoryPath = Path.GetDirectoryName(newTodoList.FilePath);
                Directory.CreateDirectory(directoryPath);

                // Add the new TodoList to the TodoListManager
                _todoListManager.AddTodoList(newTodoList);
                _todoListManager.SaveTodoLists();

                // use File.WriteAllText to serialize only the new Todolist and save it to the file.
                // This step ensures that we are not overwriting the entire "todolists.json" file with just a single Todolist;
                // instead, we are adding the new Todolist to the existing collection of Todolists
                File.WriteAllText(newTodoList.FilePath, JsonConvert.SerializeObject(newTodoList));

                // Update the RecentLists ListBox
                //RecentLists.Items.Add(newTodoList.Title);
                _todoListManager.LoadToDoLists(); // Reload the TodoLists from file to get the updated list
                List<TodoList> recentTodoLists = _todoListManager.GetRecentTodoLists(9);
                RecentLists.Items.Clear();

                // Sort the TodoLists based on AddedTimeStamp (descending order) before adding them to the ListBox
                //recentTodoLists = recentTodoLists.OrderByDescending(todoList => todoList.AddedTimestamp).ToList();
                foreach (TodoList todoList in recentTodoLists)
                {
                    RecentLists.Items.Add(todoList.Title);
                }
                //RecentLists.UpdateLayout();

            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Files|*.json";
            openFileDialog.Title = "Open TodoList";

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Read the content of the selected JSON file
                    string jsonContent = File.ReadAllText(openFileDialog.FileName);
                    
                    // Deserialize the JSON content to a TodoList object
                    TodoList openedTodoList = JsonConvert.DeserializeObject<TodoList>(jsonContent);

                    if (RecentLists.Items.Contains(openedTodoList.Title))
                    {
                        // Remove it from its current position
                        RecentLists.Items.Remove(openedTodoList.Title);
                        _todoListManager.RemoveTodoList(openedTodoList);
                    }
                    
                    openedTodoList.AddedTimestamp = DateTime.UtcNow;
                    string directoryPath = Path.GetDirectoryName(openFileDialog.FileName);
                    string fileName = Path.GetFileName(openFileDialog.FileName);
                    openedTodoList.FilePath = Path.Combine(directoryPath, fileName);

                    // Add the opened TodoList to the TodoListManager
                    _todoListManager.AddTodoList(openedTodoList);
                    _todoListManager.SaveTodoLists();

                    // Update the RecentLists ListBox
                    _todoListManager.LoadToDoLists(); // Reload the TodoLists from file to get the updated list
                    List<TodoList> recentTodoLists = _todoListManager.GetRecentTodoLists(9);

                    
                    RecentLists.Items.Clear();
                    foreach (TodoList todoList in recentTodoLists)
                    {
                        RecentLists.Items.Add(todoList.Title);
                    }
                }
                catch (Exception ex)
                {
                    // Handle any errors that occur during file reading or deserialization
                    MessageBox.Show($"Error opening TodoList: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                SaveChanges();
                _hasUnsavedChanges = false;
            }
        }

        private void BtnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            string selectedTodoListTitle = RecentLists.SelectedItem.ToString();
            TodoList selectedTodoList = _todoListManager.GetTodoListByTitle(selectedTodoListTitle);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON Files|*.json";
            saveFileDialog.Title = "Save As";
            saveFileDialog.FileName = "Duplicate_" + selectedTodoList.Title;

            if (saveFileDialog.ShowDialog() == true)
            {
                // Create a new TodoList with the provided name
                string newTitle = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                TodoList newTodoList = new TodoList(newTitle, saveFileDialog.FileName, new List<Task>(selectedTodoList.Tasks));

                // Add the new TodoList to the TodoListManager
                _todoListManager.AddTodoList(newTodoList);
                _todoListManager.SaveTodoLists();

                // Serialize and save the new TodoList to its individual JSON file
                File.WriteAllText(newTodoList.FilePath, JsonConvert.SerializeObject(newTodoList));

                // Update the RecentLists ListBox
                _todoListManager.LoadToDoLists(); // Reload the TodoLists from file to get the updated list
                List<TodoList> recentTodoLists = _todoListManager.GetRecentTodoLists(9);

                RecentLists.Items.Clear();
                foreach (TodoList todoList in recentTodoLists)
                {
                    RecentLists.Items.Add(todoList.Title);
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            RecentLists.SelectedItem = null;
            ContentMain.Visibility = Visibility.Collapsed;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            if (_hasUnsavedChanges)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes before exiting?", "Save Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // Save changes and exit
                        SaveChanges();
                        break;
                    case MessageBoxResult.No:
                        // Exit without saving changes
                        break;
                    case MessageBoxResult.Cancel:
                        // Cancel the exit operation
                        return;
                }
            }
            Application.Current.Shutdown();
        }

        private void SaveChanges()
        {

            if (RecentLists.SelectedItem is string selectedTodoListTitle)
            {
                // Use the title to get the corresponding TodoList from the TodoListManager
                TodoList selectedTodoList = _todoListManager.GetTodoListByTitle(selectedTodoListTitle);

                if (selectedTodoList != null)
                {
                    try
                    {
                        // Serialize the updated TodoList object to JSON
                        string jsonContent = JsonConvert.SerializeObject(selectedTodoList, Formatting.Indented);

                        // Write the JSON content to the individual TodoList file
                        File.WriteAllText(selectedTodoList.FilePath, jsonContent);

                        // Update the TodoList in the TodoListManager
                        _todoListManager.SaveTodoLists();

                        // After successfully saving changes, set _hasUnsavedChanges to false
                        _hasUnsavedChanges = false;
                    }
                    catch (Exception ex)
                    {
                        // Handle any errors that occur during file writing
                        MessageBox.Show($"Error saving changes: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        //Additional buttons
        private void AddTodoListItem_Click(object sender, RoutedEventArgs e)
        {
            AddItemModal addItemModal = new AddItemModal(); 
            bool? result = addItemModal.ShowDialog();
            
            if (result == true)
            {
                Task newItem = addItemModal.NewItem;
                if (RecentLists.SelectedItem != null)
                {
                    string selectedTodoListTitle = RecentLists.SelectedItem.ToString();
                    TodoList selectedTodoList = _todoListManager.GetTodoListByTitle(selectedTodoListTitle);

                    // Add the new task to the selected TodoList
                    _todoListManager.AddTask(selectedTodoList, newItem);
                    _hasUnsavedChanges = true;
                    _todoListManager.SaveTodoLists();
                    TasksDataGrid.ItemsSource = null;
                    TasksDataGrid.ItemsSource = selectedTodoList.Tasks;
                }
            }
        }

        private void BtnActionCompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton toggleButton && toggleButton.DataContext is Task taskItem)
            {
                string selectedTodoListTitle = RecentLists.SelectedItem.ToString();
                TodoList selectedTodoList = _todoListManager.GetTodoListByTitle(selectedTodoListTitle);

                //_todoListManager.CompleteTask(selectedTodoList, taskItem);
                taskItem.IsCompleted = !taskItem.IsCompleted;
                toggleButton.Content = taskItem.IsCompleted ? "Uncomplete" : "Complete";
                _hasUnsavedChanges = true;
                _todoListManager.SaveTodoLists();
                TasksDataGrid.ItemsSource = null;
                TasksDataGrid.ItemsSource = selectedTodoList.Tasks;
            }
        }

        private void BtnActionRemoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Task taskItem)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to remove this item?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    string selectedTodoListTitle = RecentLists.SelectedItem.ToString();
                    TodoList selectedTodoList = _todoListManager.GetTodoListByTitle(selectedTodoListTitle);

                    // Remove the task from the selected TodoList
                    
                    _todoListManager.RemoveTask(selectedTodoList, taskItem);
                    _hasUnsavedChanges = true;
                    _todoListManager.SaveTodoLists();
                    TasksDataGrid.ItemsSource = null;
                    TasksDataGrid.ItemsSource = selectedTodoList.Tasks;
                }
            }
        }



        private void TasksDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            _hasUnsavedChanges = true;
            BtnSave.IsEnabled = true;
        }
        private void TasksDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _hasUnsavedChanges = true;
            BtnSave.IsEnabled = true;
        }


    }
}
