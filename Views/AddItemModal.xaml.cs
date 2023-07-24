using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToDo_NewLogic.Models;
using ToDo_NewLogic.Managers;

namespace ToDo_NewLogic.Views
{
    /// <summary>
    /// Interaction logic for AddItemModal.xaml
    /// </summary>
    public partial class AddItemModal : Window
    {
        public Task NewItem { get; private set; }
        public AddItemModal()
        {
            InitializeComponent();
        }

        private void BtnAddTodoItem_Click(object sender, RoutedEventArgs e)
        {
            Task newItem = new Task(
                Guid.NewGuid(),
                TxtTitle.Text,
                TxtDescription.Text,
                int.Parse(((ComboBoxItem)CmbPriority.SelectedItem).Content.ToString()),
                false,
                DpDueDate.SelectedDate
            );

            NewItem = newItem;

            // Set the DialogResult to true to indicate that the user clicked the "Add item" button
            DialogResult = true;

            // Close the AddItemModal window
            Close();
        }

        private void TxtTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Enable the "Add item" button if the Title is not empty, otherwise disable it.
            BtnAddTodoItem.IsEnabled = !string.IsNullOrWhiteSpace(TxtTitle.Text);
        }


    }
}
