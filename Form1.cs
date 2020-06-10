using Binder.DataLayer;
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

        #region DLL_IMPORTS

        [DllImport("User32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        #endregion

        #region FLAGS

        public const int VK_RETURN = 0x0D; //V key code
        public const int VK_LCONTROL = 0xA2; //Left Control key code
        public const int V = 0x56; //V key code

        #endregion

        #region Variables

        private List<Profile> profiles;

        private int allowedBinds = 10;

        private Configs configs;

        #endregion

        #region Initialization
        public Form1()
        {
            InitializeComponent();

            profiles = new List<Profile>();
            
            RegisterKeys();

            LoadProfilesList();
            LoadConfiguration();

            
        }

        #endregion

        #region Configuration

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
                for (int i = 0; i < allowedBinds; i++)
                {
                    Controls.Find("textBox" + i, true).First().Enabled = false;
                }
                saveButton.Enabled = false;
            }

        }


        private void SaveConfiguration()
        {
            using (StreamWriter sw = new StreamWriter("Configs/Conf.txt"))
            {
                sw.WriteLine(JsonConvert.SerializeObject(configs));
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

        #endregion

        #region Click Handler

        protected override void WndProc(ref Message m) //handle keys and send messages
        {
            if(m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();


                Clipboard.Clear();

                string bindText = (Controls.Find($"textBox{id}", true).First() as TextBox).Text;
                bool imSend = (Controls.Find($"checkBox{id}", true).First() as CheckBox).Checked;

                if (!bindText.Equals(string.Empty))
                {

                    Clipboard.SetText(bindText);

                    Paste();


                    if (imSend)
                    {
                        Send();
                    }

                }

            }

            base.WndProc(ref m);
        }

        private void Paste()
        {
            keybd_event(VK_LCONTROL, 0, 0, 0);
            keybd_event(V, 0, 0, 0);
            keybd_event(V, 0, 2, 0);
            keybd_event(VK_LCONTROL, 0, 2, 0);
        }

        private void Send()
        {
            keybd_event(VK_RETURN, 0, 0, 0);
            keybd_event(VK_RETURN, 0, 2, 0);
        }

        #endregion

        

        #region Profiles

        private void AddNewProfile()
        {
            using (AddNewProfileForm addNew = new AddNewProfileForm())
            {
                if (addNew.ShowDialog() == DialogResult.OK)
                {
                    Profile profile = new Profile(addNew.title, allowedBinds);

                    profilesComboBox.Items.Add(profile.title);
                    profiles.Add(profile);

                    using (StreamWriter sw = new StreamWriter("Profiles/ProfilesList.txt", true))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(profile));
                    }

                    profilesComboBox.SelectedItem = profile.title;

                    profilesComboBox.Update();

                }
            }
        }

        private void LoadProfilesList()
        {
            if (File.Exists("Profiles/ProfilesList.txt"))
            {

                using (StreamReader sr = new StreamReader($"Profiles/ProfilesList.txt"))
                {
                    string line;
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


        #endregion

        #region Binds

        private List<Bind> LoadProfileBinds(string title)
        {
            List<Bind> binds = new List<Bind>();

            if (File.Exists($"Binds/{title}.txt"))
            {
                using (StreamReader sr = new StreamReader($"Binds/{title}.txt"))
                {
                    return JsonConvert.DeserializeObject<List<Bind>>(sr.ReadToEnd());
                }
            }

            for (int i = 0; i < allowedBinds; i++)
                binds.Add(new Bind(string.Empty));

            return binds;
        }

        private void ShowProfileBinds(string title)
        {
            Profile profile = profiles.Find(p => p.title.Equals(title));

            for (int i = 0; i < profile.binds.Count; i++)
            {
                (Controls.Find($"textBox{i}", true).First() as TextBox).Text = profile.binds[i].text;
                (Controls.Find($"checkBox{i}", true).First() as CheckBox).Checked = profile.binds[i].imSend;
            }

            configs.lastSelectedProfile = profile.title;
            SaveConfiguration();
        }

        private void SaveProfileBinds()
        {
            Profile profile = profiles.Find(p => p.title.Equals(profilesComboBox.SelectedItem.ToString()));

            profile.binds = new List<Bind>(allowedBinds);

            for(int i = 0; i < allowedBinds; i++)
            {
                profile.binds.Add(new Bind((Controls.Find("textBox" + i, true).FirstOrDefault() as TextBox).Text)
                { 
                    imSend = (Controls.Find("checkBox" + i, true).FirstOrDefault() as CheckBox).Checked

                });
            }

            using(StreamWriter sw = new StreamWriter($"Binds/{profile.title}.txt"))
            {
                sw.Write(JsonConvert.SerializeObject(profile.binds));
            }
        }

        #endregion

        #region Buttons Clicks

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveProfileBinds();
        }

        private void AddProfileButton_Click(object sender, EventArgs e)
        {
            AddNewProfile();
        }

        #endregion

        #region Boxes Events

        private void ProfilesComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            ShowProfileBinds((sender as ComboBox).SelectedItem.ToString());

            if (!textBox0.Enabled)
            {
                for (int i = 0; i < allowedBinds; i++)
                {
                    Controls.Find("textBox" + i, true).First().Enabled = true;
                }

                saveButton.Enabled = true;
            }
        }


        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            Profile profile = profiles.Find(p => p.title.Equals(profilesComboBox.SelectedItem));

            profile.binds[Convert.ToInt32(checkBox.Tag)].imSend = !profile.binds[Convert.ToInt32(checkBox.Tag)].imSend;

        }


        #endregion






    }
}
