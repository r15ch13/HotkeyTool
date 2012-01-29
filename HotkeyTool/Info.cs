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
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace HotkeyTool
{
    public partial class Info : Form
    {
        public Info()
        {
            InitializeComponent();
            label2.Text = String.Format("Developed by Richard 'r15ch13' Kuhnt\nVersion: {0}\nFileversion: {1}", Assembly.GetExecutingAssembly().GetAssemblyVersion(), Assembly.GetExecutingAssembly().GetFileVersion());
            label3.Text = "Iconsets used:\nfamfamfam.com Silk Iconset by Mark James\nHuman-O2 Iconset by Oliver Scholtz";
        }   
    }

    /// <summary>
    /// Assembly Extension Class
    /// </summary>
    public static class AssemblyExtension
    {
        /// <summary>
        /// Returns the Assembly Version
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Version GetAssemblyVersion(this Assembly assembly)
        {
            return assembly.GetName().Version;
        }

        /// <summary>
        /// Returns the Fileversion of the Assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Version GetFileVersion(this Assembly assembly)
        {
            return new Version(FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion);
        }
    }
}
