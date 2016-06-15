using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VM.Net.VirtualMachine
{
    static class InterceptKeys
    {
        public delegate int KeyboardHookProc(int code, int wParam, ref KeyboardHookStruct lParam);

        public struct KeyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;
        
        private static IntPtr hhook = IntPtr.Zero;

        public static event EventHandler<int> OnKeyDown;
        public static event EventHandler<int> OnKeyUp;

        private static KeyboardHookProc callbackDelegate;
        
        public static void SetHook()
        {
            if (callbackDelegate != null) throw new InvalidOperationException("Can't hook more than once");
            IntPtr hInstance = LoadLibrary("User32");
            callbackDelegate = new KeyboardHookProc(HookProc);
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, callbackDelegate, hInstance, 0);
            if (hhook == IntPtr.Zero) throw new Win32Exception();
        }

        public static void UnHook()
        {
            if (callbackDelegate == null) return;
            bool ok = UnhookWindowsHookEx(hhook);
            if (!ok) throw new Win32Exception();
            callbackDelegate = null;
        }

        public static int HookProc(int code, int wParam, ref KeyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    OnKeyDown?.Invoke(null, lParam.vkCode);
                }
                else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    OnKeyUp?.Invoke(null, lParam.vkCode);
                }
            }
            return CallNextHookEx(hhook, code, wParam, ref lParam);
        }

        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookProc callback, IntPtr hInstance, uint threadId);
        
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref KeyboardHookStruct lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

    }

    public class VirtualKeyboard : IPeripheral, IDisposable
    {
        private Queue<uint> keyCodes;

        public VirtualKeyboard(Form parentForm)
        {
            keyCodes = new Queue<uint>();
            InterceptKeys.SetHook();
            InterceptKeys.OnKeyDown += KeyDown;
            InterceptKeys.OnKeyUp += KeyUp;
        }

        private void KeyDown(object sender, int e)
        {
            if (keyCodes.Count > 16)
                keyCodes.Dequeue();

            e |= 0x01000000;

            keyCodes.Enqueue((uint)e);
        }
        
        private void KeyUp(object sender, int e)
        {
            if (keyCodes.Count > 16)
                keyCodes.Dequeue();

            e |= 0x02000000;

            keyCodes.Enqueue((uint)e);
        }

        public void Pass(uint value)
        {
            if ((value & 0x00000001) != 0)
                keyCodes.Clear();
        }

        public uint Poll()
        {
            if (keyCodes.Count > 0)
                return keyCodes.Dequeue();
            else
                return 0;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                InterceptKeys.UnHook();
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~VirtualKeyboard() {
           // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
           Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
