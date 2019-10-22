using Codeplex.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace leMaik.McHeads
{
    public class MinecraftSkin
    {
        //private const String SKIN_URL_NICKNAME = "http://skins.minecraft.net/MinecraftSkins/{0}.png";
        private const          String        SKIN_URL_UUID = "http://textures.minecraft.net/texture/{0}";
        public static readonly MinecraftSkin Steve;
        private readonly       BitmapImage   _skin;

        private MinecraftSkin(BitmapImage skin) { _skin = skin; }

        static MinecraftSkin() {
            var skin = new BitmapImage();
            skin.BeginInit();
            skin.StreamSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("leMaik.McHeads.steve.png");
            skin.EndInit();
            skin.Freeze();
            Steve = new MinecraftSkin(skin);
        }

        public static async Task<MinecraftSkin> LoadByNicknameAsync(String playername) {
            return await Task.Run(async () => {
                                      try {
                                          using (var client = new WebClient()) {
                                              var rawJson = client.DownloadString(new Uri("https://api.mojang.com/users/profiles/minecraft/" + playername));
                                              dynamic json = DynamicJson.Parse(rawJson);
                                              return await LoadByUuidAsync(json["id"]);
                                          }
                                      }
                                      catch (Exception) {
                                          return Steve;
                                      }
                                  });
        }

        private static Dictionary<string, MinecraftSkinUrlQuery> CachedResponces = new Dictionary<string, MinecraftSkinUrlQuery>();

        private static async Task<String> GetSkinUrl(String uuid) {
            using (WebClient wc = new WebClient()) {
                string responce = null;
                if (CachedResponces.TryGetValue(uuid.Replace("-", String.Empty), out var cached) && cached.IsCooldownPassed()) {
                    responce = cached.Responce;
                }
                else {
                    if (CachedResponces.ContainsKey(uuid.Replace("-", String.Empty)))
                        CachedResponces.Remove(uuid.Replace("-", String.Empty));
                    responce = await wc.DownloadStringTaskAsync(@"https://sessionserver.mojang.com/session/minecraft/profile/" + uuid.Replace("-", String.Empty));
                    CachedResponces.Add(uuid.Replace("-", String.Empty), new MinecraftSkinUrlQuery() {QueryTime = DateTime.Now, Responce = responce});
                }

                dynamic json = DynamicJson.Parse(responce);
                byte[] encodedBytes = Convert.FromBase64String(json["properties"][0]["value"]);
                string decodedString = Encoding.UTF8.GetString(encodedBytes);
                dynamic data = DynamicJson.Parse(decodedString);
                var toreturn = data["textures"]["SKIN"]["url"];
                return data["textures"]["SKIN"]["url"];
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
                                                    using (var response = (HttpWebResponse) request.GetResponse()) {
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

    public class MinecraftProfileManifest
    {
        public string id   { get; set; }
        public string name { get; set; }
    }

    public class MinecraftSkinUrlQuery
    {
        public DateTime QueryTime { get; set; }
        public string   Responce  { get; set; }

        public bool IsCooldownPassed() {
            TimeSpan pauseMin = TimeSpan.FromMinutes(1.5);
            DateTime compare = QueryTime.Add(pauseMin);
            return DateTime.Now >= compare;
        }
    }
}
