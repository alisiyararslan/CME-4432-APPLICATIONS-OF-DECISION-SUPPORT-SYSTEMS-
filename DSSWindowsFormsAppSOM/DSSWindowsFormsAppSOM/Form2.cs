using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSSWindowsFormsAppSOM
{
    public partial class Form2 : Form
    {
        public List<string> resultList;
        public Form2(List<string> list, string title)
        {
            resultList = list;
            InitializeComponent();

            foreach (string item in resultList)
            {
                resultLabel.Text += item + "\n";
            }

            this.Text = title;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.Location = new Point(500, 300);
        }
    }
}
