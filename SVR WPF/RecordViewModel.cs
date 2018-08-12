using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SVR_WPF
{
    class RecordViewModel : INotifyPropertyChanged
    {
        private string _StudNo;
        private string _LastName;
        private string _FirstName;
        private string _ResidenceStatus;

        public RecordViewModel()
        {
            List = new List<String>();
            List.Add("Computer Science");
            List.Add("Transferee");
            List.Add("Shifter");
            ResidenceStatus = null;
        }
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

        public string StudNo
        {
            get { return _StudNo; }
            set
            {
                this.MutateVerbose(ref _StudNo, value, RaisePropertyChanged());
            }
        }

        public string ResidenceStatus
        {
            get { return _ResidenceStatus; }
            set
            {
                this.MutateVerbose(ref _ResidenceStatus, value, RaisePropertyChanged());
            }
        }
        public IList<String> List { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }
    }
}
