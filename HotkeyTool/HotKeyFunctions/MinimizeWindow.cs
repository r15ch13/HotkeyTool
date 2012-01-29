/*
 * Copyright 2012 Richard Kuhnt
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
    /// Minimizes the current foregroundwindow
    /// </summary>
    public class MinimizeWindow : IHotkeyFunction
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// Returns the handle of the current foregroundwindow
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        /// <summary>
        /// Minimizes the current foregroundwindow
        /// </summary>
        public void Execute()
        {
            IntPtr hWnd = GetForegroundWindow();
            if (!hWnd.Equals(IntPtr.Zero))
            {
                // SW_SHOWMAXIMIZED to maximize the window
                // SW_SHOWMINIMIZED to minimize the window
                // SW_SHOWNORMAL to make the window be normal size
                ShowWindowAsync(hWnd, SW_SHOWMINIMIZED);
            }
        }

        /// <summary>
        /// Hotkey Name
        /// </summary>
        public string Name
        {
            get
            {
                return "Minimize Window";
            }
        }
    }
}
