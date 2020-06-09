using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Binder
{
    public partial class AddNewProfileForm : Form
    {

        public string title { get; set; }

        public AddNewProfileForm()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void AddProfileButton_Click(object sender, EventArgs e)
        {
            title = TitleTextBox.Text;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {

        }
    }
}
