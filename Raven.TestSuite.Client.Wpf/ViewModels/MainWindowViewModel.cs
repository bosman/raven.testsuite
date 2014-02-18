using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using Raven.TestSuite.Client.Wpf.Helpers;
using Raven.TestSuite.Client.Wpf.Models;
using Raven.TestSuite.TestRunner;
using System.Linq;

namespace Raven.TestSuite.Client.Wpf.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private static RavenTestRunner runner;

        #region Properties

        #region MyDateTime

        private DateTime _myDateTime;
        public DateTime MyDateTime
        {
            get { return _myDateTime; }
            set
            {
                if (_myDateTime != value)
                {
                    _myDateTime = value;
                    RaisePropertyChanged(() => MyDateTime);
                }
            }
        }

        #endregion

        #region PersonsCollection

        private ObservableCollection<Person> _personsCollection;
        public ObservableCollection<Person> PersonsCollection
        {
            get { return _personsCollection; }
            set
            {
                if (_personsCollection != value)
                {
                    _personsCollection = value;
                    RaisePropertyChanged(() => PersonsCollection);
                }
            }
        }

        #endregion

        #endregion

        #region Commands

        public ICommand RefreshDateCommand { get { return new DelegateCommand(OnRefreshDate); } }
        public ICommand RefreshPersonsCommand { get { return new DelegateCommand(OnRefreshPersons); } }
        public ICommand DoNothingCommand { get { return new DelegateCommand(OnDoNothing, CanExecuteDoNothing); } }

        #endregion


        private ObservableCollection<TestModel> testsCollection;
        public ObservableCollection<TestModel> TestsCollection
        {
            get { return testsCollection; }
            set
            {
                if (testsCollection != value)
                {
                    testsCollection = value;
                    RaisePropertyChanged(() => TestsCollection);
                }
            }
        }



        #region Ctor
        public MainWindowViewModel()
        {
            



            TestsCollection = new ObservableCollection<TestModel>();
            var testMethods =
                runner.GetAllRavenDotNetApiTests()
                      .Select(
                          x =>
                          new TestGroupModel
                              {
                                  Name = x.GroupType.Name,
                                  Tests = x.Tests.Select(y => new TestModel {Name = y.Name}).ToList()
                              })
                      .ToList();
            foreach (var testMethod in testMethods)
            {
                TestsCollection.Add(new TestModel { Name = testMethod.Name });
            }

            RandomizeData();
        }
        #endregion

        #region Command Handlers

        private void OnRefreshDate()
        {
            MyDateTime = DateTime.Now;
        }

        private void OnRefreshPersons()
        {
            RandomizeData();
        }

        private void OnDoNothing()
        {
        }

        private bool CanExecuteDoNothing()
        {
            return false;
        }

        #endregion

        private void RandomizeData()
        {
            PersonsCollection = new ObservableCollection<Person>();

            for (var i = 0; i < 10; i++)
            {
                PersonsCollection.Add(new Person(
                    RandomHelper.RandomString(10, true),
                    RandomHelper.RandomInt(1, 43),
                    RandomHelper.RandomBool(),
                    RandomHelper.RandomNumber(50, 180, 1),
                    RandomHelper.RandomDate(new DateTime(1980, 1, 1), DateTime.Now),
                    RandomHelper.RandomColor()
                    ));
            }
        }
    }
}