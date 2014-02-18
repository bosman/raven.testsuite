using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Client.Wpf.Models;

namespace Raven.TestSuite.Client.Wpf.ViewModels
{
    public class TreeViewItemViewModel : INotifyPropertyChanged
    {
        readonly ObservableCollection<TreeViewItemViewModel> children;
        readonly TreeViewItemViewModel parent;

        public TreeViewItemViewModel(TreeViewItemViewModel parent)
        {
            this.parent = parent;
            this.children = new ObservableCollection<TreeViewItemViewModel>();
        }

        public ObservableCollection<TreeViewItemViewModel> Children
        {
            get { return children; }
        }

        public TreeViewItemViewModel Parent
        {
            get { return parent; }
        }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    foreach (var child in Children)
                    {
                        child.IsChecked = isChecked;
                    }
                    this.OnPropertyChanged("IsChecked");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
