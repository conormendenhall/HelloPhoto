namespace HelloPhoto.Models
{
    public static class AdminSettings
    {
        public static double LEDBrightness { get; set; } = 100;

        public static Event Event { get; set; }

        public static bool UseSplash { get; set; } = true;

        public static bool UseOverlay { get; set; } = true;

        public static bool EnableFaceReg { get; set; } = true;

        public static int ComPort { get; set; } = 3;

        public static bool ProdEnabled { get; set; } = false;

    }
}
