using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
/*2: add usings*/
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace TransparencyWindowWpfApp_1
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    ///
    public partial class Window1 : Window
    {
        /*3: Import some WinAPI functions*/
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr window, int index, int value);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr window, int index);
        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(int hWnd, int hWndInsertAfter,
            int X, int Y, int cx, int cy, uint uFlags);
        /*4 define constants*/
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int HWND_BOTTOM = 0x1;
        public const uint SWP_NOSIZE = 0x1;
        public const uint SWP_NOMOVE = 0x2;
        public const uint SWP_SHOWWINDOW = 0x40;

        private Boolean mAlreadyFaded;

        public Window1()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Application app = Application.Current;
            //app.Shutdown();
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!mAlreadyFaded)
            {
                Closing -= Window_Closing_1;
                e.Cancel = true;
                //DoubleAnimation doubleAnimation = new DoubleAnimation();
                //doubleAnimation.From = 1;
                //doubleAnimation.To = 0;
                //doubleAnimation.Duration = TimeSpan.FromSeconds(10);
                var doubleAnimation = new DoubleAnimation(0, (Duration)TimeSpan.FromSeconds(1));
                doubleAnimation.Completed += new EventHandler(doubleAnimation_Completed);
                this.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
                mAlreadyFaded = true;
            }

        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            mAlreadyFaded = false;
            /*7: start HideFromAltTab*/
            HideFromAltTab(Handle);
        }

        void doubleAnimation_Completed(object sender, EventArgs e)
        {
            Close();
        }

        /*5: create functions*/
        public static void HideFromAltTab(IntPtr Handle)
        {
            SetWindowLong(Handle, GWL_EXSTYLE,
                GetWindowLong(Handle, GWL_EXSTYLE) | WS_EX_TOOLWINDOW);
        }
        private void ShoveToBackground()
        {
            SetWindowPos((int)this.Handle, HWND_BOTTOM,
                0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        /*6: create handle of the window*/
        private IntPtr Handle
        {
            get
            {
                return new WindowInteropHelper(this).Handle;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            /*8: start ShoveToBackground*/
            ShoveToBackground();
        }
    }
}
