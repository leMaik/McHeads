using System;
using System.ComponentModel;

namespace CornerHead {
    /// <summary>
    /// Interaktionslogik für ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : INotifyPropertyChanged {
        public String Playername {
            get { return Settings.playername; }
            set {
                Settings.playername = value;
                OnPropertyChanged("Playername");
            }
        }

        public int Size {
            get { return Settings.size; }
            set {
                Settings.size = value;
                OnPropertyChanged("Size");
            }
        }

        public int Position {
            get { return Settings.position; }
            set {
                Settings.position = value;
                OnPropertyChanged("Position");
            }
        }

        public bool GoAway {
            get { return Settings.goaway; }
            set {
                Settings.goaway = value;
                OnPropertyChanged("GoAway");
            }
        }

        public double HeadOpacity {
            get { return Settings.opacity; }
            set {
                Settings.opacity = value;
                OnPropertyChanged("HeadOpacity");
            }
        }

        public ConfigWindow() {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String propertyName) {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
