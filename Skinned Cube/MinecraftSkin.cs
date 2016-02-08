using Codeplex.Data;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace leMaik.McHeads {
    public class MinecraftSkin {
        private const String SKIN_URL_NICKNAME = "http://skins.minecraft.net/MinecraftSkins/{0}.png";
        private const String SKIN_URL_UUID = "http://textures.minecraft.net/texture/{0}";
        public static readonly MinecraftSkin Steve;
        private readonly BitmapImage _skin;

        private MinecraftSkin(BitmapImage skin) {
            _skin = skin;
        }

        static MinecraftSkin() {
            var skin = new BitmapImage();
            skin.BeginInit();
            skin.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("leMaik.McHeads.steve.png");
            skin.EndInit();
            skin.Freeze();
            Steve = new MinecraftSkin(skin);
        }

        public static async Task<MinecraftSkin> LoadByNicknameAsync(String playername) {
            var mcSkin = await Task.Run(() => {
                var request = WebRequest.Create(String.Format(SKIN_URL_NICKNAME, playername));
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
                    catch {
                        //web exception, or maybe decoding the image failed
                        return Steve;
                    }
                }

                skin.Freeze();
                return new MinecraftSkin(skin);
            });
            return mcSkin;
        }

        private static async Task<String> GetSkinUrl(String uuid) {
            using (var client = new WebClient()) {
                var rawJson = await client.DownloadStringTaskAsync(new Uri("https://sessionserver.mojang.com/session/minecraft/profile/" + uuid.Replace("-", String.Empty)));
                dynamic json = DynamicJson.Parse(rawJson);
                var properties = ((dynamic[])json.properties).First(p => p.name == "textures").value;

                var textureData = DynamicJson.Parse(System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(properties)));
                var textureUrl = textureData.textures.SKIN.url;
                return textureUrl;
            }
        }

        public static async Task<MinecraftSkin> LoadByUuidAsync(String uuid) {
            string url;
            try {
                url = await GetSkinUrl(uuid);
            }
            catch {
                return Steve;
            }
            var mcSkin = await Task.Run(() => {
                var request = WebRequest.Create(url);
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
                    catch {
                        //web exception, or maybe decoding the image failed
                        return Steve;
                    }
                }

                skin.Freeze();
                return new MinecraftSkin(skin);
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
