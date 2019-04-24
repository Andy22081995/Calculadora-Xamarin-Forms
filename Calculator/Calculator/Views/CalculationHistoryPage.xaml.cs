using Calculator.Extensions;
using Calculator.Models;
using Calculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Calculator.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalculationHistoryPage : ContentPage
    {
        public ObservableCollection<History> Items = new ObservableCollection<History>();

        public new bool IsVisible { get; set; }

        public CalculationHistoryPage()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            var enumerator = App.DbController.GetDBItems();
            if (enumerator == null)
                IsEnumeratorEmpty(true, enumerator);
            else
            {
                while (enumerator.MoveNext())
                    this.Items.Add(enumerator.Current);

                IsEnumeratorEmpty(false);
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
                IsEnumeratorEmpty(true, enumerator);
        }

        private void IsEnumeratorEmpty(bool empty, [Optional] IEnumerator<History> enumerator)
        {
            if (empty)
            {
                enumerator = new EmptyEnumerator<History>();
                EmptyState.IsVisible = true;
                ListViewItems.IsVisible = false;
                IsVisible = false;
            }
            else
            {
                EmptyState.IsVisible = false;
                ListViewItems.IsVisible = true;
                IsVisible = true;
            }
        }

        private void HideableToolbarItem_Clicked(object sender, EventArgs e)
        {
            this.Items.Clear();
            App.DbController.DeleteAll();
            var enumerator = App.DbController.GetDBItems();
            if (enumerator == null)
                IsEnumeratorEmpty(true, enumerator);
        }
    }
}