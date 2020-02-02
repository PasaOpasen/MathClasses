using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Библиотека_графики
{
    public partial class TextBoxForm : Form
    {
        public TextBoxForm(string caption,TextBox textBox)
        {
            InitializeComponent();
            this.Text = caption;
            this.textBox1.WordWrap = textBox.WordWrap;
            this.textBox1.Text = textBox.Text;

            this.Size = new Size(textBox.Size.Width+16,textBox.Size.Height+38);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
