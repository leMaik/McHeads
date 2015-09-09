using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace leMaik.McHeads {
    /// <summary>
    /// Interaktionslogik für MouseWatchingHead.xaml
    /// </summary>
    public class MouseWatchingHead : Head {
        #region Ugly code
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition() {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }
        #endregion

        public double ScreenDistance {
            get { return (double)GetValue(ScreenDistanceProperty); }
            set { SetValue(ScreenDistanceProperty, value); }
        }

        public static readonly DependencyProperty ScreenDistanceProperty =
            DependencyProperty.Register("ScreenDistance", typeof(double), typeof(MouseWatchingHead), new PropertyMetadata(350d));

        public bool LookAtCursor {
            get { return (bool)GetValue(LookAtCursorProperty); }
            set { SetValue(LookAtCursorProperty, value); }
        }

        public static readonly DependencyProperty LookAtCursorProperty =
            DependencyProperty.Register("LookAtCursor", typeof(bool), typeof(MouseWatchingHead), new PropertyMetadata(true));

        private void RotateHead() {
            if (!IsVisible) return;
            if (!LookAtCursor) return;
            var mouse = PointFromScreen(GetMousePosition());

            mouse.Offset(-ActualWidth / 2, -ActualHeight / 2);
            RotationY = Math.Acos(mouse.Y / (Math.Sqrt(ScreenDistance * ScreenDistance + mouse.X * mouse.X + mouse.Y * mouse.Y))) * 180 / Math.PI - 90;
            RotationZ = Math.Sign(mouse.X) * Math.Acos(ScreenDistance / (Math.Sqrt(ScreenDistance * ScreenDistance + mouse.X * mouse.X))) * 180 / Math.PI;
        }

        private DispatcherTimer _dispatcherTimer;

        public MouseWatchingHead() {
            Loaded += (s, e) => {
                _dispatcherTimer = new DispatcherTimer(DispatcherPriority.Render);
                _dispatcherTimer.Tick += (t, f) => RotateHead();
                _dispatcherTimer.Interval = TimeSpan.FromSeconds(1 / 25f);
                _dispatcherTimer.Start();

            };
            HeadLoaded += (s, e) => {
                var oldScale = Scale;
                BeginAnimation(ScaleProperty,
                    new DoubleAnimation(0d, oldScale, new Duration(TimeSpan.FromMilliseconds(500))) {
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                        FillBehavior = FillBehavior.Stop
                    });
            };
        }

        ~MouseWatchingHead() {
            if (_dispatcherTimer != null)
                _dispatcherTimer.Stop();
        }
    }
}
