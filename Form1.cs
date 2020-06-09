﻿using Binder.DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

namespace Binder
{
    public partial class Form1 : Form
    {

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("User32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        private readonly IntPtr hWnd;

        private List<Profile> profiles;

        private int allowedBinds = 3;

        #region Initialization
        public Form1()
        {
            InitializeComponent();

            hWnd = FindProcess();
            
            RegisterKeys();

            LoadProfilesList();

        }

        private IntPtr FindProcess()
        {

            foreach (Process pList in Process.GetProcessesByName("GTAV"))
            {
                if (pList.MainWindowTitle.Contains("RАGЕ Multiрlayer   "))
                {
                    return pList.MainWindowHandle;
                }
            }

            return IntPtr.Zero;
        }
        #endregion


        #region Global Keys

        private void RegisterKeys()
        {
            for (int i = 0; i < allowedBinds; i++)
            {
                RegisterHotKey(Handle, i, 0x0000, (int)Keys.NumPad0 + i);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if(m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();

                SetForegroundWindow(hWnd);

                switch (id)
                {
                    case 0:
                        SendKeys.SendWait(textBox0.Text);
                        break;
                    case 1:
                        SendKeys.SendWait(textBox1.Text);
                        break;
                    case 2:
                        SendKeys.SendWait(textBox2.Text);
                        break;
                }
            }

            base.WndProc(ref m);
        }

        #endregion

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveProfileBinders();
        }

        private void SaveProfileBinders()
        {
            Profile profile = new Profile(profilesComboBox.SelectedItem.ToString());

            for(int i = 0; i < allowedBinds; i++)
            {
                profile.binds.Add((Controls.Find("textBox" + i, true).FirstOrDefault() as TextBox).Text);
            }

            using(StreamWriter sw = new StreamWriter($"Binders/{profile.title}.txt"))
            {
                sw.Write(JsonConvert.SerializeObject(profile.binds));
            }
        }

        private void LoadProfilesList()
        {
            string line = string.Empty;
            //Profile profile = new Profile();

            if (File.Exists("Profiles/ProfilesList.txt"))
            {

                using (StreamReader sr = new StreamReader($"Profiles/ProfilesList.txt"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        Profile profile = JsonConvert.DeserializeObject<Profile>(line);
                        profiles.Add(profile);
                        profilesComboBox.Items.Add(profile.title);
                    }
                }

            }
            

        }

        private void AddProfileButton_Click(object sender, EventArgs e)
        {
            AddNewProfile();
        }

        private void AddNewProfile()
        {
            using (AddNewProfileForm addNew = new AddNewProfileForm())
            {
                if (addNew.ShowDialog() == DialogResult.OK)
                {
                    Profile profile = new Profile(addNew.title);

                    profilesComboBox.Items.Add(profile.title);
                    profilesComboBox.Update();

                }
            }
        }

        private void ProfilesComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            LoadProfile((sender as ComboBox).Items[0].ToString());
        }

        private void LoadProfile(string title)
        {

        }
    }
}
