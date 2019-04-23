namespace Calculator.ViewModels
{
    using Calculator.Extensions;
    using Calculator.Models;
    using Calculator.Views;
    using GalaSoft.MvvmLight.Command;
    using System.Collections.ObjectModel;
    using System.Text.RegularExpressions;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class StartViewModel : BaseViewModel
    {
        #region Attributes
        private string _input;
        private string _output;
        #endregion

        #region Properties
        public string Input
        {
            get => _input;
            set
            {
                SetValue(ref _input, value);
                SetOutput(Regex.Replace(_input, " ", ""));
            }
        }
        public string Output
        {
            get => _output;
            set => SetValue(ref _output, value);
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        public StartViewModel()
        {
            this.Input = string.Empty;

            this.AddNumberCommand = new Command<string>(
               execute: (string param) =>
               {
                   this.Input += param;
                   RefreshCanExecutes();
               });

            this.AddOperatorCommand = new Command<string>(
                execute: (string param) =>
                {
                    this.Input += param;
                    RefreshCanExecutes();
                },
                canExecute: (string param) =>
                {
                    if (string.IsNullOrEmpty(this.Input))
                        return false;
                    if (char.IsDigit(this.Input[this.Input.Length - 1]))
                        return true;
                    else
                        return false;
                });

            this.ResolveCommand = new Command(
                execute: () =>
                {
                    DisplayResult();
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    if (string.IsNullOrEmpty(this.Input))
                        return false;
                    else
                        return true;
                });

            this.ClearCommand = new Command(
                execute: () =>
                {
                    this.Input = string.Empty;
                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    if (string.IsNullOrEmpty(this.Input))
                        return false;
                    else
                        return true;
                });

            this.DeleteCommand = new Command(
                execute: () =>
                {
                    if (!string.IsNullOrEmpty(this.Input) && this.Input[Input.Length - 1] == ' ')
                        this.Input = this.Input.Remove(this.Input.Length - 3, 3);
                    else if (!string.IsNullOrEmpty(Input))
                        this.Input = this.Input.Remove(this.Input.Length - 1, 1);

                    RefreshCanExecutes();
                },
                canExecute: () =>
                {
                    if (string.IsNullOrEmpty(this.Input))
                        return false;
                    else
                        return true;
                });
        }
        #endregion

        #region Commands
        public ICommand CalculationHistoryCommand => new RelayCommand(this.CalculationHistory);
        public ICommand AddNumberCommand { get; private set; }
        public ICommand AddOperatorCommand { get; private set; }
        public ICommand ResolveCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        #endregion

        #region Methods
        private void SetOutput(string entry)
        {
            MathExtension math = new MathExtension();
            if (!string.IsNullOrEmpty(entry) && char.IsDigit(entry[entry.Length - 1]))
            {
                Output = math.Parse(entry).ToString();
            }
            else
            {
                Output = "Error en la sintaxis";
            }
        }

        /// <summary>
        /// Inicia la navegación hacia el historial de cálculo
        /// </summary>
        private async void CalculationHistory()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new CalculationHistoryPage());
        }
        
        private void RefreshCanExecutes()
        {
            ((Command)this.AddNumberCommand).ChangeCanExecute();
            ((Command)this.AddOperatorCommand).ChangeCanExecute();
            ((Command)this.ResolveCommand).ChangeCanExecute();
            ((Command)this.ClearCommand).ChangeCanExecute();
            ((Command)this.DeleteCommand).ChangeCanExecute();
        }

        private async void DisplayResult()
        {
            App.DbController.SaveOrUpdate(new History { Expression = Input, Result = Output });

            await Application.Current.MainPage.DisplayAlert("Resultado", Output, "Aceptar");

            RefreshCanExecutes();
            return;
        }

        #endregion
    }
}