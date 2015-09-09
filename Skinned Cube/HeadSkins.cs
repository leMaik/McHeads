using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace leMaik.McHeads {
    public class HeadSkins : INotifyPropertyChanged {
        public string Uuid { get; private set; }

        public string Playername { get; private set; }

        private CubeSkin _head;
        public CubeSkin Head {
            get { return _head; }
            set {
                _head = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Head"));
            }
        }

        private CubeSkin _helmet;
        public CubeSkin Helmet {
            get { return _helmet; }
            set {
                _helmet = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Helmet"));
            }
        }

        private HeadSkins() { }

        public static async Task<HeadSkins> LoadAsync(String player) {
            return assembleHead(await MinecraftSkin.LoadByNicknameAsync(player), player, null);
        }

        public static async Task<HeadSkins> LoadByUuidAsync(String uuid) {
            return assembleHead(await MinecraftSkin.LoadByUuidAsync(uuid), null, uuid);
        }

        private static HeadSkins assembleHead(MinecraftSkin skin, String player, String uuid) {
            return new HeadSkins {
                Playername = player,
                Uuid = uuid,
                Head = new CubeSkin {
                    Top = skin.GetSegment(2, 0, 2, 2),
                    Bottom = skin.GetSegment(4, 0, 2, 2),
                    Left = skin.GetSegment(4, 2, 2, 2),
                    Right = skin.GetSegment(0, 2, 2, 2),
                    Front = skin.GetSegment(2, 2, 2, 2),
                    Back = skin.GetSegment(6, 2, 2, 2)
                },
                Helmet = new CubeSkin {
                    Top = skin.GetSegment(10, 0, 2, 2),
                    Bottom = skin.GetSegment(12, 0, 2, 2),
                    Left = skin.GetSegment(12, 2, 2, 2),
                    Right = skin.GetSegment(8, 2, 2, 2),
                    Front = skin.GetSegment(10, 2, 2, 2),
                    Back = skin.GetSegment(14, 2, 2, 2)
                }
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class CubeSkin {
        public BitmapSource Top { get; set; }
        public BitmapSource Bottom { get; set; }
        public BitmapSource Left { get; set; }
        public BitmapSource Right { get; set; }
        public BitmapSource Front { get; set; }
        public BitmapSource Back { get; set; }
    }
}
