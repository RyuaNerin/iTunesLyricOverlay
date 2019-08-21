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
            this.Control_LocationChanged(null, null);

            MainModel.Instance.OnLyricsFocusChanged += this.MainModel_OnLyricsFocusChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.m_control.Owner = this;
            this.m_control.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void Control_LocationChanged(object sender, EventArgs e)
        {
            this.Left = this.m_control.Left + this.m_control.Width  - this.Width;
            this.Top  = this.m_control.Top;
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