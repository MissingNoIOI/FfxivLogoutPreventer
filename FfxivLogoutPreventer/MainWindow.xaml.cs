using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FfxivLogoutPreventer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cts;
        private bool isRunning = false;

        private IKeyboardMouseEvents globalHook;
        private Task controllerTask;

        public MainWindow()
        {
            InitializeComponent();
            globalHook = Hook.GlobalEvents();
            globalHook.KeyPress += GlobalHookKeyPress;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(cts == null)
            {
                cts = new CancellationTokenSource();
            }

            if (isRunning)
            {
                cts.Cancel();
            }
            else
            {
                controllerTask = Task.Run(() => WindowController.Start(cts.Token));
                controllerTask.GetAwaiter().OnCompleted(() =>
                {
                    isRunning = false;
                    startButton.Content = "Start";
                    startButton.Glyph = WPFUI.Common.Icon.Play12;
                    cts?.Dispose();
                    cts = new CancellationTokenSource();
                });
                
                isRunning = true;
                startButton.Content = "Stop";
                startButton.Glyph = WPFUI.Common.Icon.Pause12;
            }
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 'c')
            {
                if (cts != null)
                {
                    cts.Cancel();
                }
            }
        }
    }
}
