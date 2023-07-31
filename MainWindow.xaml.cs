using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

                // Create the TodoList file
                File.WriteAllText(newTodoList.FilePath, JsonConvert.SerializeObject(newTodoList));

                // Update the RecentLists ListBox
                RecentLists.Items.Add(newTodoList.Title);

                // Save the TodoLists to the "todolists.json" file
                _todoListManager.SaveTodoLists();
            }
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            //if (_hasUnsavedChanges)
            //{
            //    MessageBoxResult result = MessageBox.Show("Do you want to save changes?", "Save Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            //    switch (result)
            //    {
            //        case MessageBoxResult.Yes:
            //            SaveChanges();
            //            break;
            //        case MessageBoxResult.No:
            //            break;
            //        case MessageBoxResult.Cancel:
            //            return;
            //    }
            //}
        }

        private void BtnSaveAs_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
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

        }

        private void BtnActionRemoveTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Task taskItem)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to remove this item?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                }
            }
        }



        private void TasksDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            _hasUnsavedChanges = true;
        }
        private void TasksDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            _hasUnsavedChanges = true;
        }


    }
}
