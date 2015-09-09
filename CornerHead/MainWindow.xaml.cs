using System.Windows.Threading;
using leMaik.McHeads;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CornerHead {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged {
        private String _playername;
        public String Playername {
            get { return _playername; }
            set {
                _playername = value;
                OnPropertyChanged("Playername");
                MinecraftSkin.LoadAsync(Playername).ContinueWith(t => {
                    var b = BitmapFromSource(t.Result.GetSegment(2, 2, 2, 2));
                    App.TrayIcon.Icon = System.Drawing.Icon.FromHandle(b.GetHicon());
                    b.Dispose();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private double _size;
        public double Size {
            get { return _size; }
            set {
                _size = value;
                OnPropertyChanged("Size");
            }
        }

        public MainWindow() {
            InitializeComponent();
            DataContext = this;

            var b = BitmapFromSource(MinecraftSkin.Steve.GetSegment(2, 2, 2, 2));
            App.TrayIcon.Icon = System.Drawing.Icon.FromHandle(b.GetHicon());
            b.Dispose();

            if (File.Exists("settings"))
                Settings.Load("settings", "1.0");
            ApplySettings();

            App.TrayIcon.MenuActivation = Hardcodet.Wpf.TaskbarNotification.PopupActivationMode.All;

            App.TrayIcon.ContextMenu = new ContextMenu();
            var config = new MenuItem() { Header = "Configuration" };
            config.Click += (s, e) => {
                var cwin = new ConfigWindow();
                cwin.PropertyChanged += (t, f) => ApplySettings();
                cwin.Show();
            };
            App.TrayIcon.ContextMenu.Items.Add(config);
            var exit = new MenuItem() { Header = "Close" };
            exit.Click += (s, e) => Application.Current.Shutdown();
            App.TrayIcon.ContextMenu.Items.Add(exit);

            Closed += (s, e) => {
                Settings.Save("settings", "1.0");
                App.TrayIcon.Dispose();
            };

            var dispatcherTimer = new DispatcherTimer(DispatcherPriority.ContextIdle);
            var animRunning = 0;
            dispatcherTimer.Tick += (t, f) => {
                if (!Settings.goaway)
                    return;
                if (IsPointOverForm(MouseWatchingHead.GetMousePosition())) {
                    if (Opacity > 0 && animRunning != 1) {
                        BeginAnimation(OpacityProperty,
                            new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(150)), FillBehavior.HoldEnd),
                            HandoffBehavior.SnapshotAndReplace);
                        animRunning = 1;
                    }
                }
                else {
                    if (Opacity < Settings.opacity && animRunning != 2) {
                        BeginAnimation(OpacityProperty,
                            new DoubleAnimation(Settings.opacity, new Duration(TimeSpan.FromMilliseconds(150)), FillBehavior.Stop),
                            HandoffBehavior.SnapshotAndReplace);
                        animRunning = 2;
                    }
                }

            };
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1 / 25.0);
            dispatcherTimer.Start();
        }

        private void ApplySettings() {
            Playername = Settings.playername;
            Size = Settings.size;

            Left = (Settings.position == 0 || Settings.position == 2) ? 0 : SystemParameters.VirtualScreenWidth - Size;
            Top = Settings.position <= 1 ? 0 : SystemParameters.VirtualScreenHeight - Size;
            Opacity = Settings.opacity;
        }

        private bool IsPointOverForm(Point p) {
            return p.X >= Left && p.X <= Left + ActualWidth && p.Y >= Top && p.Y <= Top + ActualHeight;
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e) {
            //if (Settings.goaway)
            //    BeginAnimation(LeftProperty, new DoubleAnimation(PositonX == 0 ? ActualWidth : 0, new Duration(TimeSpan.FromMilliseconds(250))));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private static System.Drawing.Bitmap BitmapFromSource(BitmapSource bitmapsource) {
            using (var outStream = new MemoryStream()) {
                var enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                return new System.Drawing.Bitmap(outStream);
            }
        }
    }
}
