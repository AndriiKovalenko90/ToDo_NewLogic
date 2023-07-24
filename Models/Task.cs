using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ToDo_NewLogic.Models
{
    public class Task : INotifyPropertyChanged
    {
        private string _title;
        private string? _description;
        private int? _priority;
        private bool _isCompleted;
        private DateTime? _dueDate;     

        public Guid Id { get; set; }

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title == value)
                    return;
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string? Description
        {
            get { return _description; }
            set
            {
                if (_description == value)
                    return;
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public int? Priority
        {
            get { return _priority; }
            set 
            {
                if (_priority == value)
                    return;
                _priority = value;
                OnPropertyChanged("Priority");
            }
        }

        public bool IsCompleted
        {
            get { return _isCompleted; }
            set 
            {
                if (_isCompleted == value)
                    return;
                _isCompleted = value;
                OnPropertyChanged("IsCompleted");
            }
        }

        public DateTime CreatedAt { get; set; }

        public DateTime? DueDate
        {
            get { return _dueDate; }
            set
            {
                if (_dueDate == value)
                    return;
                _dueDate = value;
                OnPropertyChanged("DueDate");
            }
        }

        public Task(Guid id, string title, string? description, int? priority = null, bool isCompleted = false, DateTime? dueDate = null)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Priority = priority;
            IsCompleted = isCompleted;
            CreatedAt = DateTime.UtcNow;
            DueDate = dueDate;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}