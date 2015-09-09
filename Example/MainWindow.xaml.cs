using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace leMaik.McHeads {
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged {
        public String skin {
            get { return "saschb2b"; }
            set {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("skin"));
            }
        }

        public MainWindow() {
            InitializeComponent();
            DataContext = this;
            skin = "";
        }

        private void MouseWatchingHead_Clicked(object sender, EventArgs e) {
            ((MouseWatchingHead)sender).BeginAnimation(MouseWatchingHead.ScaleProperty, new DoubleAnimation(1.2, new Duration(TimeSpan.FromMilliseconds(400))) { AutoReverse = true, EasingFunction = new BackEase() });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
