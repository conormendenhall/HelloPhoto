using System;

namespace HelloPhoto.Models
{
	class Photo
	{
		public string PhotoId { get; set; }
		public Contact PhotoContact { get; set; }
		public string FilePath { get; set; }
	}
}
