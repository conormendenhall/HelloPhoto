using HelloPhoto.Models;
using HelloPhoto.Repositories;
using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HelloPhoto
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Login : Page
	{
		public Login()
		{
			InitializeComponent();
		    submitButton.Visibility = Visibility.Visible;
		    if (AdminSettings.EnableFaceReg)
		    {
		        registerFace.Visibility = Visibility.Visible;
		    }
		    else
		    {
		        registerFace.Visibility = Visibility.Collapsed;
            }
        }

		#region Navigation

		private void HomeButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainPage));
		}

		#endregion Navigation

		private async void SubmitButton_Click(object sender, RoutedEventArgs e)
		{
		    submitButton.Visibility = Visibility.Collapsed;
			var contact = await SaveContactAsync();

			Frame.Navigate(typeof(PhotoBooth), contact);
		}

		private async Task<Contact> SaveContactAsync()
		{
			ContactRepository _repo = new ContactRepository();

			var contact = new Contact()
			{
				Id = Guid.NewGuid().ToString(),
				Email = string.IsNullOrWhiteSpace(EmailInput?.Text) ? "photobooth_image" : EmailInput.Text,
                EventId = AdminSettings.Event?.EventId ?? "NoEventIdSadFace"
			};

			return await _repo.Save(contact);
		}

        private void beethovensBtn_Click(object sender, RoutedEventArgs e)
        {
            //ask for password?
            pwGrid.Visibility = Visibility.Visible;
            pwText.Focus(FocusState.Keyboard);
        }

        private void pwButton_Click(object sender, RoutedEventArgs e)
        {
            if (pwText.Password.Equals("1234"))
            {
                Frame.Navigate(typeof(AdminPage));
            }
            else
            {
                pwGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void pwText_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                pwButton_Click(null, null);
            }
        }

        private void registerFace_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FaceOff));
        }
    }
}
