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

        private int allowedBinds = 10;

        private Configs configs;

        #region Initialization
        public Form1()
        {
            InitializeComponent();

            profiles = new List<Profile>();
            hWnd = FindProcess();
            
            RegisterKeys();

            LoadProfilesList();
            LoadConfiguration();

            

            

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

        private void LoadConfiguration()
        {
            if (File.Exists("Configs/Conf.txt"))
            {
                using (StreamReader sr = new StreamReader("Configs/Conf.txt"))
                {
                    configs = JsonConvert.DeserializeObject<Configs>(sr.ReadToEnd());
                }

                profilesComboBox.SelectedItem = configs.lastSelectedProfile;
            }
            else
            {
                configs = new Configs();
                for(int i = 0; i < allowedBinds; i++)
                {
                    Controls.Find("textBox" + i, true).First().Enabled = false;
                }
                saveButton.Enabled = false;
            }
             
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

        protected override void WndProc(ref Message m) //handle keys and send messages
        {
            if(m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();

                SetForegroundWindow(hWnd);
                Clipboard.Clear();

                string bindText = (Controls.Find($"textBox{id}", true).First() as TextBox).Text;

                if (!bindText.Equals(string.Empty))
                {

                    Clipboard.SetText(bindText);
                    SendKeys.Flush();
                    SendKeys.SendWait("^(v)");
                }
                //SendKeys.SendWait((Controls.Find($"textBox{id}", true).First() as TextBox).Text);

            }

            base.WndProc(ref m);
        }

        #endregion

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveProfileBinds();
        }

        private void SaveProfileBinds()
        {
            Profile profile = profiles.Find(p => p.title.Equals(profilesComboBox.SelectedItem.ToString()));

            profile.binds = new List<string>(allowedBinds);

            for(int i = 0; i < allowedBinds; i++)
            {
                profile.binds.Add((Controls.Find("textBox" + i, true).FirstOrDefault() as TextBox).Text);
            }

            using(StreamWriter sw = new StreamWriter($"Binds/{profile.title}.txt"))
            {
                sw.Write(JsonConvert.SerializeObject(profile.binds));
            }
        }

        private void LoadProfilesList()
        {
            string line = string.Empty;
            

            if (File.Exists("Profiles/ProfilesList.txt"))
            {

                using (StreamReader sr = new StreamReader($"Profiles/ProfilesList.txt"))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        Profile profile = JsonConvert.DeserializeObject<Profile>(line);
                        profile.binds = LoadProfileBinds(profile.title);
                        profiles.Add(profile);
                        profilesComboBox.Items.Add(profile.title);

                        

                    }
                }

            }
            

        }

        private List<string> LoadProfileBinds(string title)
        {
            List<string> binds = new List<string>();

            for (int i = 0; i < allowedBinds; i++)
                binds.Add(string.Empty);

            if (File.Exists($"Binds/{title}.txt"))
            {
                using(StreamReader sr = new StreamReader($"Binds/{title}.txt"))
                {
                    return JsonConvert.DeserializeObject<List<string>>(sr.ReadToEnd());
                }
            }

            return binds;
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
                    Profile profile = new Profile(addNew.title, allowedBinds);

                    profilesComboBox.Items.Add(profile.title);
                    profiles.Add(profile);

                    using(StreamWriter sw = new StreamWriter("Profiles/ProfilesList.txt", true))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(profile));
                    }

                    profilesComboBox.SelectedItem = profile.title;

                    profilesComboBox.Update();

                }
            }
        }

        private void ProfilesComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ShowProfileBinds((sender as ComboBox).SelectedItem.ToString());

            if(!textBox0.Enabled)
            {
                for (int i = 0; i < allowedBinds; i++)
                {
                    Controls.Find("textBox" + i, true).First().Enabled = true;
                }

                saveButton.Enabled = true;
            }
        }

        private void ShowProfileBinds(string title)
        {
            Profile profile = profiles.Find(p => p.title.Equals(title));

            for(int i = 0; i < profile.binds.Count; i++)
            {
                (Controls.Find($"textBox{i}", true).First() as TextBox).Text = profile.binds[i];
            }

            configs.lastSelectedProfile = profile.title;
            SaveConfiguration();
        }

        private void SaveConfiguration()
        {
            using(StreamWriter sw = new StreamWriter("Configs/Conf.txt"))
            {
                sw.WriteLine(JsonConvert.SerializeObject(configs));
            }
        }
    }
}
