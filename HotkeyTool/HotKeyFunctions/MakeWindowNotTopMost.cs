/*
 * Copyright 2012 Richard 'r15ch13' Kuhnt
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Runtime.InteropServices;

namespace HotkeyTool.HotKeyFunctions
{
    /// <summary>
    /// Makes the current window allways on top
    /// 
    /// http://stackoverflow.com/questions/2292778/make-window-always-stay-on-top-of-another-window-that-already-stays-on-top
    /// </summary>
    class MakeWindowNotTopMost : IHotkeyFunction
    {
        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern int SetWindowLong(HandleRef hWnd, int nIndex, HandleRef dwNewLong);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        }

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private static readonly IntPtr HWND_TOP = new IntPtr(0);
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        public void Execute()
        {
            IntPtr hWnd = MinimizeWindow.NativeMethods.GetForegroundWindow();
            if (!hWnd.Equals(IntPtr.Zero))
            {
                NativeMethods.SetWindowPos(hWnd, HWND_NOTOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }
        }

        /// <summary>
        /// Hotkey Name
        /// </summary>
        public string Name
        {
            get { return "Make window not top most"; }
        }
    }
}
