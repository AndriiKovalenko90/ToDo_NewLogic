using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ToDo_NewLogic.Views
{
    /// <summary>
    /// Interaction logic for CreateNewTodoList.xaml
    /// </summary>
    public partial class CreateNewTodoList : Window
    {
        public string TodoListTitle { get; private set; } = null;
        public CreateNewTodoList()
        {
            InitializeComponent();
        }

        private void BtnCreateTodoList_Click(object sender, RoutedEventArgs e)
        {
            TodoListTitle = TxtTodoListTitle.Text.Trim();
            if (string.IsNullOrWhiteSpace(TodoListTitle))
            {
                MessageBox.Show("Please enter a valid name for the TodoList.");
                return;
            }

            DialogResult = true;
        }
    }
}
