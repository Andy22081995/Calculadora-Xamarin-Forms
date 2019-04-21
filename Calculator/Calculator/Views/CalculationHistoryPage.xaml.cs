﻿using Calculator.Models;
using Calculator.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var enumerator = App.DbController.GetDBItems();
            if (enumerator == null)
            {
                App.DbController.SaveOrUpdate(new History { Id = 0, Expression = "1 + 2 + 3 + 4", Result = "10" });
                enumerator = App.DbController.GetDBItems();
            }
            while (enumerator.MoveNext())
            {
                this.items.Add(enumerator.Current);
            }
            enumerator.Reset();
            ListViewItems.ItemsSource = this.items;
        }
    }
}