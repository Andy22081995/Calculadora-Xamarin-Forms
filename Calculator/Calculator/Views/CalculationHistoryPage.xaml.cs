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
        public ObservableCollection<History> Items = new ObservableCollection<History>();

        public CalculationHistoryPage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            var enumerator = App.DbController.GetDBItems();
            if (enumerator == null)
            {
                enumerator = new EmptyEnumerator<History>();
                EmptyState.IsVisible = true;
                ListViewItems.IsVisible = false;
            }
            else
            {
                while (enumerator.MoveNext())
                {
                    this.Items.Add(enumerator.Current);
                }
                EmptyState.IsVisible = false;
                ListViewItems.IsVisible = true;
                ListViewItems.ItemsSource = this.Items;
            }
        }

        private void MenuItem_Clicked(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            var model = (History)item.CommandParameter;
            this.Items.Remove(model);
            App.DbController.DeleteItem(model.Id);
            var enumerator = App.DbController.GetDBItems();
            if (enumerator == null)
            {
                enumerator = new EmptyEnumerator<History>();
                EmptyState.IsVisible = true;
                ListViewItems.IsVisible = false;
            }
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