using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ToDo_NewLogic.Models;

namespace ToDo_NewLogic.Managers
{
    public class TaskManager
    {
        public BindingList<Task> Tasks { get; set; }

        public TaskManager()
        {
            Tasks = new BindingList<Task>();
        }

        public void AddTask(Task task)
        {
            Tasks.Add(task);
        }

        public void RemoveTask(Task task)
        {
            Tasks.Remove(task);
        }

        public void EditTask(Task task, string newDescription)
        {
            throw new NotImplementedException();
        }
    }
}
