using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.TestSuite.Storage;

namespace Raven.TestSuite.Client.Wpf.ViewModels.TestsComparator
{
    public class RavenTestRunViewModel : INotifyPropertyChanged
    {
        public static RavenTestRunViewModel FromRavenTestRun(RavenTestRun ravenTestRun)
        {
            var result = new RavenTestRunViewModel
                {
                    Id = ravenTestRun.Id,
                    StartedAt = ravenTestRun.StartedAt,
                    StoppedAt = ravenTestRun.StoppedAt
                };
            return result;
        }

        private bool isChecked;

        public int Id { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime StoppedAt { get; set; }

        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    OnPropertyChanged("IsChecked");
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
