﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SearchableNavbar
{
    public class SelectableData : INotifyPropertyChanged
    {
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool isSelected = false;
    }
}
