using Calculator.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Calculator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculationHistoryPage : ContentPage
    {
        public ObservableCollection<History> items = new ObservableCollection<History>();

        public CalculationHistoryPage()
        {
            InitializeComponent();
            Init();
        }

 

        private void Init()
        {
            try
            {
                var enumerator = App.DbController.GetDBItems();
                if (enumerator == null)
                {
                    enumerator = new EmptyEnumerator<History>();
                }
                else
                {
                    while (enumerator.MoveNext())
                    {
                        this.items.Add(enumerator.Current);
                    }

                    ListViewItems.ItemsSource = this.items;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            var model = (History)item.CommandParameter;
            this.items.Remove(model);
            App.DbController.DeleteItem(model.Id);
        }
    }
    public class EmptyEnumerator<T> : IEnumerator<T>
    {

        public T Current
        {
            get
            {
                return default(T);
            }
        }
        object System.Collections.IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }
        public void Dispose()
        {
        }
        public bool MoveNext()
        {
            return false;
        }
        public void Reset()
        {
        }
    }
}