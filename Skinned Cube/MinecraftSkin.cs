using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace leMaik.McHeads {
    public class MinecraftSkin {
        private const String SKIN_URL = "https://skins.minecraft.net/MinecraftSkins/{0}.png";
        public static readonly MinecraftSkin Steve;
        private BitmapImage _skin;

        private MinecraftSkin() { }

        static MinecraftSkin() {
            Steve = new MinecraftSkin();
            var skin = new BitmapImage();
            skin.BeginInit();
            skin.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("leMaik.McHeads.steve.png");
            skin.EndInit();
            skin.Freeze();
            Steve._skin = skin;
        }

        public static async Task<MinecraftSkin> LoadAsync(String playername) {
            var mcSkin = await Task.Run(() => {
                var request = WebRequest.Create(String.Format(SKIN_URL, playername));
                var buffer = new byte[4096];
                var skin = new BitmapImage();
                using (var target = new MemoryStream()) {
                    try {
                        using (var response = (HttpWebResponse)request.GetResponse()) {
                            using (var stream = response.GetResponseStream()) {
                                int read;

                                while (stream != null && (read = stream.Read(buffer, 0, buffer.Length)) > 0) {
                                    target.Write(buffer, 0, read);
                                }
                            }

                            skin.BeginInit();
                            skin.CacheOption = BitmapCacheOption.OnLoad;
                            skin.StreamSource = target;
                            skin.EndInit();
                        }
                    }
                    catch (WebException) {
                        //For some dumb reason, this is .NET framwork's way to say "hey, error 404 or so"
                        return Steve;
                    }
                }

                skin.Freeze();
                return new MinecraftSkin { _skin = skin };
            });
            return mcSkin;
        }

        public BitmapSource GetSegment(int x, int y, int width = 1, int height = 1) {
            var source = new CroppedBitmap(_skin, new Int32Rect(4 * x, 4 * y, width * 4, height * 4));
            return CreateResizedImage(source, 64, 64, 0);
        }

        private static BitmapFrame CreateResizedImage(ImageSource source, int width, int height, int margin) {
            var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.NearestNeighbor);
            group.Children.Add(new ImageDrawing(source, rect));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(group);

            var resizedImage = new RenderTargetBitmap(
                width, height,         // Resized dimensions
                96, 96,                // Default DPI values
                PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            return BitmapFrame.Create(resizedImage);
        }
    }
}
