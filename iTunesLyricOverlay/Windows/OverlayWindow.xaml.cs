using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using iTunesLyricOverlay.Models;

namespace iTunesLyricOverlay.Windows
{
    public partial class OverlayWindow : Window
    {
        private readonly OverlayControlWindow m_control;

        public OverlayWindow()
        {
            this.InitializeComponent();

            this.m_control = new OverlayControlWindow();
            this.m_control.LocationChanged += this.Control_LocationChanged;
            this.m_control.SizeChanged += this.Control_SizeChanged;

            MainModel.Instance.OnLyricsFocusChanged += this.MainModel_OnLyricsFocusChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.m_control.Owner = this;

            this.m_control.Left = this.Left + this.Width - this.m_control.Width;
            this.m_control.Top  = this.Top - this.m_control.Height;

            this.m_control.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public new void Show()
        {
            base.Show();
            this.m_control?.Show();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.m_control.Left = this.Left + this.Width - this.m_control.Width;
            this.m_control.Top  = this.Top - this.m_control.Height;
        }

        private volatile bool m_controlSizeChanged = true;
        private void Control_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.m_controlSizeChanged = true;
            this.m_control.Left = this.Left + this.Width - e.NewSize.Width;
            this.m_control.Top  = this.Top - e.NewSize.Height;
            this.m_controlSizeChanged = false;
        }
        private void Control_LocationChanged(object sender, EventArgs e)
        {
            if (this.m_controlSizeChanged)
                return;
            
            this.Left = this.m_control.Left + this.m_control.Width  - this.Width;
            this.Top  = this.m_control.Top + this.m_control.Height;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var helper = new WindowInteropHelper(this);
            helper.EnsureHandle();

            var exStyle = NativeMethods.GetWindowLong(helper.Handle, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(helper.Handle, NativeMethods.GWL_EXSTYLE, exStyle | NativeMethods.WS_EX_NOACTIVATE | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TRANSPARENT);
        }

        private void MainModel_OnLyricsFocusChanged(LyricLineGroupModel item)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.ctlLyric.ItemsSource = item.Where(e => !string.IsNullOrWhiteSpace(e.Line.Text)).ToArray();
            }));
        }
    }
}
