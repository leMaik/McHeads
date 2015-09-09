using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace leMaik.McHeads {
    /// <summary>
    /// Interaktionslogik für Head.xaml
    /// </summary>
    public partial class Head : INotifyPropertyChanged {
        public String Playername {
            get { return (String)GetValue(PlayernameProperty); }
            set { SetValue(PlayernameProperty, value); }
        }

        public static readonly DependencyProperty PlayernameProperty =
            DependencyProperty.Register("Playername", typeof(String), typeof(Head), new PropertyMetadata(String.Empty, OnPlayernamePropertyChanged));

        public String Uuid {
            get { return (String)GetValue(UuidProperty); }
            set { SetValue(UuidProperty, value); }
        }

        public static readonly DependencyProperty UuidProperty =
            DependencyProperty.Register("Uuid", typeof(String), typeof(Head), new PropertyMetadata(String.Empty, OnUuidPropertyChanged));

        private static async void OnPlayernamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (((Head)d).Uuid != null) {
                ((Head)d).Uuid = null;
            }
            await ((Head)d).Load();
        }

        private static async void OnUuidPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (String.IsNullOrWhiteSpace(((Head)d).Playername)) {
                await ((Head)d).Load();
            }
        }

        private HeadSkins _skin;
        public HeadSkins Skin {
            get { return _skin; }
            private set {
                _skin = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Skin"));
            }
        }

        public event EventHandler HeadLoaded;

        public double RotationX {
            get { return (double)GetValue(RotationXProperty); }
            set { SetValue(RotationXProperty, value); }
        }

        public static readonly DependencyProperty RotationXProperty =
            DependencyProperty.Register("RotationX", typeof(double), typeof(Head), new PropertyMetadata(0d));

        public double RotationY {
            get { return (double)GetValue(RotationYProperty); }
            set { SetValue(RotationYProperty, value); }
        }

        public static readonly DependencyProperty RotationYProperty =
            DependencyProperty.Register("RotationY", typeof(double), typeof(Head), new PropertyMetadata(0d));

        public double RotationZ {
            get { return (double)GetValue(RotationZProperty); }
            set { SetValue(RotationZProperty, value); }
        }

        public static readonly DependencyProperty RotationZProperty =
            DependencyProperty.Register("RotationZ", typeof(double), typeof(Head), new PropertyMetadata(0d));

        public double Scale {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register("Scale", typeof(double), typeof(Head), new PropertyMetadata(1d));

        public Head() {
            InitializeComponent();

            Loaded += (s, e) => {
                viewport.MouseUp += mainViewport_MouseUp;
            };
        }

        ModelVisual3D GetHitTestResult(Point location) {
            var result = VisualTreeHelper.HitTest(viewport, location);
            if (result == null || !(result.VisualHit is ModelVisual3D)) return null;
            return (ModelVisual3D)result.VisualHit;
        }

        void mainViewport_MouseUp(object sender, MouseButtonEventArgs e) {
            var location = e.GetPosition(viewport);
            var result = GetHitTestResult(location);
            if (result != null) {
                RaiseEvent(new RoutedEventArgs(HeadClickedEvent));
            }
        }

        public async Task Load() {
            if (!String.IsNullOrWhiteSpace(Uuid)) {
                if (Skin == null || Skin.Uuid != Uuid) {
                    var skin = await HeadSkins.LoadByUuidAsync(Uuid);
                    if (skin.Uuid == Uuid) {
                        Skin = skin;
                        if (HeadLoaded != null)
                            HeadLoaded(this, new EventArgs());
                    }
                }
            }
            else {
                var skin = await HeadSkins.LoadAsync(Playername);
                Uuid = skin.Uuid;
                if (skin.Playername == Playername) {
                    Skin = skin;
                    if (HeadLoaded != null)
                        HeadLoaded(this, new EventArgs());
                }
            }
        }

        public event RoutedEventHandler HeadClicked {
            add { AddHandler(HeadClickedEvent, value); }
            remove { RemoveHandler(HeadClickedEvent, value); }
        }
        public static readonly RoutedEvent HeadClickedEvent = EventManager.RegisterRoutedEvent("HeadClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Head));

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
