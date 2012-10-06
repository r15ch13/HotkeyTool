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
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace HotkeyTool
{
    /// <summary>
    /// The Hotkey
    /// </summary>
    [Serializable()]
    public class GlobalHotkey
    {

        #region properties
        /// <summary>
        /// CTRL modifier
        /// </summary>
        public bool Ctrl { get; set; }

        /// <summary>
        /// SHIFT modifier
        /// </summary>
        public bool Shift { get; set; }

        /// <summary>
        /// ALT modifier
        /// </summary>
        public bool Alt { get; set; }

        /// <summary>
        /// WIN
        /// </summary>
        public bool Win { get; set; }

        /// <summary>
        /// The key
        /// </summary>
        public Keys Key { get; set; }

        /// <summary>
        /// Form handle
        /// </summary>
        private IntPtr Handle { get; set; }

        /// <summary>
        /// Assigned function
        /// </summary>
        public IHotkeyFunction HotkeyFunction { get; set; }

        /// <summary>
        /// Hotkey function name
        /// </summary>
        private string hotkeyFunctionName;

        /// <summary>
        /// Hashcode
        /// </summary>
        private int Id
        {
            get
            {
                return Modifier ^ (int)Key ^ Handle.ToInt32();
            }
        }

        /// <summary>
        /// Combines all modifiers and returns the value
        /// </summary>
        public int Modifier
        {
            get
            {
                return (Ctrl ? Constants.Ctrl : 0x0000) + (Shift ? Constants.Shift : 0x0000) + (Alt ? Constants.Alt : 0x0000) + (Win ? Constants.Win : 0x0000);
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handle">Form handle</param>
        public GlobalHotkey(Keys key, IntPtr handle, IHotkeyFunction hotkeyfunction)
        {
            this.Key = key;
            this.Handle = handle;
            this.HotkeyFunction = hotkeyfunction;
        }

        #region DllImports

        internal static class NativeMethods
        {
            /// <summary>
            /// Registers the Hotkey
            /// </summary>
            /// <param name="hWnd"></param>
            /// <param name="id"></param>
            /// <param name="fsModifiers"></param>
            /// <param name="vk"></param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

            /// <summary>
            /// Unregisters the Hotkey
            /// </summary>
            /// <param name="hWnd"></param>
            /// <param name="id"></param>
            /// <returns></returns>
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        }
        #endregion

        #region methods

        /// <summary>
        /// Generates a hashcode, using the modifiers, key and the handle
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Modifier ^ (int)Key ^ Handle.ToInt32();
        }

        /// <summary>
        /// Registers the Hotkey
        /// </summary>
        /// <returns></returns>
        public bool Register()
        {
            this.Unregister();
            if (NativeMethods.RegisterHotKey(this.Handle, this.Id, this.Modifier, (int)Key))
            {
                System.Diagnostics.Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "Hotkey registered: {0}", Debug()));
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "Hotkey failed to register: {0}", Debug()));
                return false;
            }
        }

        /// <summary>
        /// Unregisters the Hotkey
        /// </summary>
        /// <returns></returns>
        public bool Unregister()
        {
            if (NativeMethods.UnregisterHotKey(Handle, Id))
            {
                System.Diagnostics.Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "Hotkey unregisted: {0}", Debug()));
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "Hotkey failed to unregister: {0}", Debug()));
                return false;
            }
        }

        /// <summary>
        /// Returns the serialized Hotkey
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}{4} => {5}", (Ctrl ? "CTRL + " : ""), (Shift ? "SHIFT + " : ""), (Alt ? "ALT + " : ""), (Win ? "WIN + " : ""), Key, HotkeyFunctionName);
        }

        /// <summary>
        /// Returns all Hotkey values
        /// </summary>
        /// <returns></returns>
        public string Debug()
        {
            return String.Format(CultureInfo.CurrentCulture, "CTRL:{0} | SHIFT:{1} | ALT:{2} | WIN:{3} | Key:{4} | Handle:{5}", Ctrl, Shift, Alt, Win, Key, Handle);
        }

        /// <summary>
        /// Returns the HotkeyFunctionType
        /// </summary>
        public string HotkeyFunctionType
        {
            get
            {
                return (HotkeyFunction != null ? HotkeyFunction.GetType().Name : "");
            }
        }

        
        /// <summary>
        /// Returns the HotkeyFunction Name
        /// </summary>
        public string HotkeyFunctionName
        {
            get
            {
                return (HotkeyFunction != null ? HotkeyFunction.Name : this.hotkeyFunctionName);
            }
            set 
            {
                this.hotkeyFunctionName = value;
            }
        }
        #endregion

        /// <summary>
        /// Serialized String
        /// </summary>
        /// <returns>e.g. "False:True:True:False:A:MinimizeWindow"</returns>
        public string Serialize()
        {
            return String.Format(CultureInfo.CurrentCulture, "{0}:{1}:{2}:{3}:{4}:{5}:{6}", Ctrl, Shift, Alt, Win, Key, HotkeyFunctionType, HotkeyFunctionName);
        }

        /// <summary>
        /// Deserialize a String to a GlobalHotkey
        /// </summary>
        /// <param name="serialized"></param>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static GlobalHotkey Deserialize(string serialized, IntPtr handle)
        {
            string[] str = serialized.Split(':');
            if (str.Length == 7)
            {
                Keys key = (Keys)Enum.Parse(typeof(Keys), str[4]);

                GlobalHotkey hk = new GlobalHotkey(key, handle, GetHotkeyFunctionInstanceByName(str[5]));
                hk.Ctrl = str[0] == "True" ? true : false;
                hk.Shift = str[1] == "True" ? true : false;
                hk.Alt = str[2] == "True" ? true : false;
                hk.Win = str[3] == "True" ? true : false;
                hk.HotkeyFunctionName = str[6];
                return hk;
            }
            return null;
        }


        /// <summary>
        /// Gets all types of a namespace
        /// </summary>
        /// <param name="assembly">current assembly</param>
        /// <param name="nameSpace">the namespace you want to get all types from</param>
        /// <returns></returns>
        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes().Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal)).ToArray();
        }

        /// <summary>
        /// Returns all HotkeyFunctions
        /// </summary>
        /// <returns></returns>
        public static ReadOnlyCollection<IHotkeyFunction> HotkeyFunctions
        {
            get
            {
                ReadOnlyCollection<IHotkeyFunction> hkf = null;

                List<IHotkeyFunction> tmp = new List<IHotkeyFunction>();
                IHotkeyFunction hk = null;
                foreach (var item in GlobalHotkey.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "HotkeyTool.HotKeyFunctions"))
                {
                    hk = GetHotkeyFunctionInstanceByName(item.Name);
                    if (hk != null)
                    {
                        tmp.Add(hk);
                    }
                }
                hkf = new ReadOnlyCollection<IHotkeyFunction>(tmp);
                return hkf;
            }
        }

        /// <summary>
        /// Returns an instance of the given HotkeyFunction
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IHotkeyFunction GetHotkeyFunctionInstanceByName(string name)
        {
            Type[] types = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "HotkeyTool.HotKeyFunctions");
            foreach (var item in types)
            {
                // Check if its an IHotkeyFunction
                if (item.Name == name && typeof(IHotkeyFunction).IsAssignableFrom(item))
                {
                    // return new instance of Type
                    return (IHotkeyFunction)Activator.CreateInstance(item);
                }
            }
            return null;
        }

    }
}
