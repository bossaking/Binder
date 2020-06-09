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


namespace Binder
{
    public partial class Form1 : Form
    {

        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("User32.dll")]
        static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        private IntPtr hWnd;

        public Form1()
        {
            InitializeComponent();

            hWnd = FindProcess();

            RegisterKeys();

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

        private void RegisterKeys()
        {
            for (int i = 0; i < 3; i++)
            {
                RegisterHotKey(Handle, i, 0x0000, (int)Keys.NumPad1 + i);
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
                        SendKeys.SendWait(textBox1.Text);
                        break;
                    case 1:
                        SendKeys.SendWait(textBox2.Text);
                        break;
                    case 2:
                        SendKeys.SendWait(textBox3.Text);
                        break;
                }
            }

            base.WndProc(ref m);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
