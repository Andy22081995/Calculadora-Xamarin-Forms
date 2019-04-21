﻿using Android.App;
using Android.Widget;
using Calculator.Droid.Alerts;
using Calculator.Helpers;

[assembly: Xamarin.Forms.Dependency(typeof(MessageAndroid))]
namespace Calculator.Droid.Alerts
{
    public class MessageAndroid : IMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}