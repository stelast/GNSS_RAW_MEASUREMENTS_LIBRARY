using GnssMeasurementSample.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace GnssMeasurementSample.Model
{
    public class MainModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand OnInitTest { private set; get; }

        public MainModel()
        {
            OnInitTest = new Command(initTest);
            DependencyService.Get<IGnssCompare>().checkPermissions();
        }

        private async void initTest()
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Se inicia con el testing", "OK");

            DependencyService.Get<IGnssCompare>().init();

        }
    }
}
