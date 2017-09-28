using HelloPhoto.Models;
using HelloPhoto.Repositories;
using System;
using System.Threading.Tasks;
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

		private async void SubmitButton_Click(object sender, RoutedEventArgs e)
		{
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
                EventId = MainPage.EventData?.EventId ?? "NoEventIdSadFace"
			};

			return await _repo.Save(contact);
		}
	}
}
