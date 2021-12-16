using Gma.System.MouseKeyHook;
using GregsStack.InputSimulatorStandard;
using GregsStack.InputSimulatorStandard.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GregsStack.InputSimulatorStandard.Native.VirtualKeyCode;

namespace FfxivLogoutPreventer
{
    internal class WindowController
    {
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);


        public static async Task Start(object obj)
        {
            CancellationToken token = (CancellationToken)obj;
            var FfxivWindow = FindWindow(null, "FINAL FANTASY XIV");
            var sim = new InputSimulator();
            var possibleKeys = new[] { VK_W, VK_A, VK_S, VK_D, SPACE, VK_E, VK_Q };
            var r = new Random((int)DateTime.Now.Ticks); ;

            var timesRun = 0;
            var state = PRESS_STATES.NOT_PRESSED;
            var timesToRun = 0;
            VirtualKeyCode key = VK_W;

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    sim.Keyboard.KeyUp(key);
                    break;
                }


                if (timesRun >= timesToRun)
                {
                    if (SetForegroundWindow(FfxivWindow))
                    {
                        switch (state)
                        {
                            case PRESS_STATES.NOT_PRESSED:
                                key = possibleKeys[r.Next(0, 6)];
                                sim.Keyboard.KeyDown(key);
                                timesRun = 0;
                                timesToRun = r.Next(5, 30);
                                state = PRESS_STATES.PRESSED;
                                break;
                            case PRESS_STATES.PRESSED:
                                sim.Keyboard.KeyUp(key);
                                timesRun = 0;
                                timesToRun = r.Next(10, 50);
                                state = PRESS_STATES.WAIT;
                                break;
                            case PRESS_STATES.WAIT:
                                timesRun = 0;
                                timesToRun = 0;
                                state = PRESS_STATES.NOT_PRESSED;
                                break;
                        }
                    }
                }
                else
                {
                    timesRun++;
                    await Task.Delay(100);
                }
            }
        }

        private enum PRESS_STATES
        {
            NOT_PRESSED,
            PRESSED,
            WAIT
        }

    }
}
