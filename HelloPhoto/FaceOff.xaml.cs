using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HelloPhoto
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FaceOff : Page
    {
        public FaceOff()
        {
            this.InitializeComponent();
        }

        private void goBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateInput())
            {
                //go to face page
                Frame.Navigate(typeof(FaceRegistration), new Models.FaceOff()
                {
                    FirstName = FirstNameTxt.Text,
                    LastName = LasttNameTxt.Text,
                    Email = EmailTxt.Text
                });
            }
        }

        private bool ValidateInput()
        {
            var errors = "Please fill out missing fields: ";
            bool foundError = false;

            if (string.IsNullOrWhiteSpace(FirstNameTxt.Text))
            {
                errors += "First name, ";
                foundError = true;
            }

            if (string.IsNullOrWhiteSpace(LasttNameTxt.Text))
            {
                errors += "Last name, ";
                foundError = true;
            }

            if (string.IsNullOrWhiteSpace(EmailTxt.Text))
            {
                errors += "Email";
                foundError = true;
            }

            if (foundError)
            {
                errorTxt.Visibility = Visibility.Visible;
                errorTxt.Text = errors;
            }
            else
            {
                errorTxt.Visibility = Visibility.Collapsed;
            }

            return !foundError;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FirstNameTxt.Text = "";
            LasttNameTxt.Text = "";
            EmailTxt.Text = "";
            Frame.Navigate(typeof(MainPage));
        }
    }
}
