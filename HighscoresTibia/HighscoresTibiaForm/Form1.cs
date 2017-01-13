using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HighscoresTibiaForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void SendDataToolStrip_Click(object sender, EventArgs e)
        {
            //consoleView.Items.Add("Processing the selected data AAAAAAAAAAAAAAAAAAAAAAAAAAAA");

  

            string[] h = { "Processing the selected data", "AAAAAAAAAAAAAAAAAAAAAAAAAAAA" };
            int count = 0;
            foreach (string s in h) //this in case when you have more that two strings in you array
            {
                ListViewItem lvi = consoleView.Items[count++];
                lvi.SubItems.Add(s);
                consoleView.Items.Add(lvi);
            }

        }
        

    }
}
