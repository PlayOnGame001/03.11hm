using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO;


namespace _03._11hm
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }


    public class MainViewModel : INotifyPropertyChanged
    {
        private string txtName;
        private string txtAddress;
        private string txtPhone;
        private Model selectedContact;
        private string block;

        public ObservableCollection<Model> Contacts { get; set; }

        public string Name
        {
            get => txtName;
            set
            {
                txtName = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get => txtAddress;
            set
            {
                txtAddress = value;
                OnPropertyChanged();
            }
        }

        public string Phone
        {
            get => txtPhone;
            set
            {
                txtPhone = value;
                OnPropertyChanged();
            }
        }

        public Model SelectedContact
        {
            get => selectedContact;
            set
            {
                selectedContact = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand ChangeCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public MainViewModel()
        {
            Contacts = new ObservableCollection<Model>();

            AddCommand = new RelayCommand(AddContact);
            ChangeCommand = new RelayCommand(ChangeContact);
            DeleteCommand = new RelayCommand(DeleteContact);
            SaveCommand = new RelayCommand(SaveContact);
        }

        private void AddContact(object parameter)
        {
            Contacts.Add(new Model { Name = Name, Address = Address, Phone = Phone });
            block += $"Name: {Name}\tAddress: {Address}\tPhone: {Phone}\n";

            Name = string.Empty;
            Address = string.Empty;
            Phone = string.Empty;
        }

        private void ChangeContact(object parameter)
        {
            if (SelectedContact != null)
            {
                Name = SelectedContact.Name;
                Address = SelectedContact.Address;
                Phone = SelectedContact.Phone;
            }
            DeleteContact(parameter);
        }

        private void DeleteContact(object parameter)
        {
            if (SelectedContact != null)
            {
                Contacts.Remove(SelectedContact);
                SelectedContact = null;
            }
        }

        private void SaveContact(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, block);
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
