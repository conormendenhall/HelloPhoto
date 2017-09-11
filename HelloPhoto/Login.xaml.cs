using HelloPhoto.Models;
using HelloPhoto.Repositories;
using System;
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
		}

		#region Navigation

		private void HomeButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainPage));
		}

		#endregion Navigation

		private void SubmitButton_Click(object sender, RoutedEventArgs e)
		{
			ContactRepository _repo = new ContactRepository();

			var contact = new Contact()
			{
				Id = Guid.NewGuid().ToString(),
				Email = EmailInput.Text
			};

			_repo.Save(contact);

			Frame.Navigate(typeof(PhotoBooth), contact);
		}
	}
}
