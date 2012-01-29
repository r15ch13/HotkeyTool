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
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace HotkeyTool
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// List with Hotkeys
        /// </summary>
        private List<GlobalHotkey> hotkeys;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            hotkeys = new List<GlobalHotkey>();
            LoadHotkeys();
            UpdateHotkeyList();

            this.comboBoxHotkeyFunctions.DataSource = null;
            this.comboBoxHotkeyFunctions.DisplayMember = "Name";
            this.comboBoxHotkeyFunctions.DataSource = GlobalHotkey.GetHotkeyFunctions();
        }

        /// <summary>
        /// Updates the Hotkey Listview
        /// </summary>
        public void UpdateHotkeyList()
        {
            this.listBoxHotkeys.DataSource = null;
            this.listBoxHotkeys.DataSource = hotkeys;
        }

        /// <summary>
        /// Saves all Hotkeys
        /// </summary>
        public void SaveHotkeys()
        {
            Properties.Settings.Default.Hotkeys = "";
            foreach (var item in hotkeys)
            {
                if (item != null)
                {
                    Properties.Settings.Default.Hotkeys += item.Serialize() + "|";
                }
            }
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Loads all Hotkeys
        /// </summary>
        public void LoadHotkeys()
        {
            Properties.Settings.Default.Reload();
            string[] str = Properties.Settings.Default.Hotkeys.Split('|');

            foreach (var item in str)
            {
                GlobalHotkey ghk = GlobalHotkey.Deserialize(item, this.Handle);
                if (ghk != null)
                {
                    hotkeys.Add(ghk);
                }
            }
        }


        /// <summary>
        /// Handles all hotkeys and executes the Hotkeyfunctions
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            // only if hotkey is pressed
            if (m.Msg == GlobalHotkey.Constants.WM_HOTKEY_MSG_ID)
            {
                // get keyvalue
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);

                // get modifiers
                int modifier = (int)m.LParam & 0xFFFF;

                foreach (var item in hotkeys)
                {
                    if (item != null)
                    {
                        // only if modifier and key match
                        if (item.Modifier == modifier && item.Key == key)
                        {
                            if (item.HotkeyFunction != null)
                            {
                                // executes hotkeyfunction
                                item.HotkeyFunction.Execute();
                                Debug.WriteLine(String.Format("Executed Hotkey: {0}", item.Debug()));
                            }
                            else
                            {
                                Debug.WriteLine(String.Format("No HotkeyFunctions assigned to Hotkey: {0}", item.Debug()));
                            }
                        }
                    }
                }
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Handles Keyevents to determine the Hotkeytrigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxKey_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ShiftKey &&
                e.KeyCode != Keys.ControlKey &&
                e.KeyCode != Keys.Alt &&
                e.KeyCode != Keys.LWin &&
                e.KeyCode != Keys.RWin)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                textBoxKey.Text = e.KeyCode.ToString();
                textBoxKey.Tag = e.KeyCode;
            }
        }

        /// <summary>
        /// Adds Hotkey to Hotkeylist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxKey.Tag != null &&
                textBoxKey.Tag.GetType() == typeof(Keys) &&
                comboBoxHotkeyFunctions.SelectedItem != null)
            {
                if (typeof(IHotkeyFunction).IsAssignableFrom(comboBoxHotkeyFunctions.SelectedItem.GetType()))
                {
                    // create new Hotkey
                    GlobalHotkey ghk = new GlobalHotkey((Keys)textBoxKey.Tag, this.Handle,
                        GlobalHotkey.GetHotkeyFunctionInstanceByName(comboBoxHotkeyFunctions.SelectedItem.GetType().Name))
                    {
                        CTRL = checkBoxCtrl.Checked,
                        SHIFT = checkBoxShift.Checked,
                        ALT = checkBoxAlt.Checked,
                        WIN = checkBoxWin.Checked,
                    };

                    if (!CheckIfHotkeyExists(ghk))
                    {
                        // Add and register Hotkey
                        hotkeys.Add(ghk);
                        UpdateHotkeyList();
                        SaveHotkeys();
                        ghk.Register();
                    }
                }

                // Reset controls
                checkBoxCtrl.Checked = false;
                checkBoxShift.Checked = false;
                checkBoxAlt.Checked = false;
                checkBoxWin.Checked = false;
                textBoxKey.Clear();
                textBoxKey.Tag = null;
            }
        }

        /// <summary>
        /// Checks if the new Hotkey already exists
        /// </summary>
        /// <param name="ghk"></param>
        /// <returns></returns>
        private bool CheckIfHotkeyExists(GlobalHotkey ghk)
        {
            if (ghk != null)
            {
                if (hotkeys.Count > 0)
                {
                    // Check if hotkey exists
                    foreach (var item in hotkeys)
                    {
                        if (item != null)
                        {
                            if (item.Modifier == ghk.Modifier && item.Key == ghk.Key)
                            {
                                MessageBox.Show("Hotkey already exists!", "Adding Hotkey", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            return false;
        }


        /// <summary>
        /// Register all Hotkeys onload
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            foreach (var item in hotkeys)
            {
                if (item != null)
                {
                    item.Register();
                }
            }
        }

        /// <summary>
        /// Register all Hotkey onclose
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveHotkeys();
            foreach (var item in hotkeys)
            {
                if (item != null)
                {
                    item.Unregister();
                }
            }
        }

        /// <summary>
        /// Deletes a Hotkey
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxHotkeys.SelectedItem != null)
            {
                GlobalHotkey ghk = (GlobalHotkey)listBoxHotkeys.SelectedItem;
                ghk.Unregister();
                hotkeys.Remove(ghk);
            }
            UpdateHotkeyList();
        }

        /// <summary>
        /// Minimize window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// Restore window or show contextmenu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Show();
                this.BringToFront();
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.contextMenuStrip.Show();
            }
        }

        /// <summary>
        /// Restore window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemRestore_Click(object sender, EventArgs e)
        {
            this.Show();
            this.BringToFront();
        }

        /// <summary>
        /// Exit application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItemClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Shows program information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonInfo_Click(object sender, EventArgs e)
        {
            new Info().ShowDialog();
        }
    }

}
