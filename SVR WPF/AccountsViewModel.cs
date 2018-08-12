using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SVR_WPF
{
    class AccountsViewModel : INotifyPropertyChanged
    {
        private string _FirstName;
        private string _LastName;
        private string _MiddleName;
        private string _username;

        public string FirstName
        {
            get { return _FirstName; }
            set
            {
                this.MutateVerbose(ref _FirstName, value, RaisePropertyChanged());
            }
        }

        public string LastName
        {
            get { return _LastName; }
            set
            {
                this.MutateVerbose(ref _LastName, value, RaisePropertyChanged());
            }
        }

        public string MiddleName
        {
            get { return _MiddleName; }
            set
            {
                this.MutateVerbose(ref _MiddleName, value, RaisePropertyChanged());
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                this.MutateVerbose(ref _username, value, RaisePropertyChanged());
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
