using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloPhoto.Models
{
    public class FaceOff
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CollectionName { get; set; } = "TSFaces";
        public string FilePath { get; set; }
    }
}
