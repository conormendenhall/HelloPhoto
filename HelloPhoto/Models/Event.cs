using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HelloPhoto.Models
{
    public class Event
    {
        private byte[] _imageOverlay;
        private byte[] _landingOverlay;

        public string EventId { get; set; }
        public bool IsActive { get; set; }
        public string TwitterHashTag { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string ImageOverlay { get; set; }
        public string LandingPageOverlay { get; set; }
        public string OptionalEmailCC { get; set; }

        [JsonIgnore]
        [NotMapped]
        public byte[] ImageOverlayBytes
        {
            get
            {
                if (string.IsNullOrEmpty(ImageOverlay))
                {
                    return null;
                }

                if (_imageOverlay == null)
                {
                    _imageOverlay = new PhotoRepository().Get(ImageOverlay).Result;
                }

                return _imageOverlay;
            }
        }

        [JsonIgnore]
        [NotMapped]
        public byte[] LandingOverlayBytes
        {
            get
            {
                if (string.IsNullOrEmpty(LandingPageOverlay)){
                    return null;
                }

                if (_landingOverlay == null)
                {
                    _landingOverlay = new PhotoRepository().Get(LandingPageOverlay).Result;
                }

                return _landingOverlay;
            }
        }

    }
}
