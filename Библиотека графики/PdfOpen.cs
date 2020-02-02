using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Библиотека_графики
{
    public partial class PdfOpen : Form
    {
        public PdfOpen(string caption,string filename)
        {
            InitializeComponent();
            this.Text = caption;
            webBrowser1.Navigate(Path.Combine( Environment.CurrentDirectory,filename));
        }
    }
}
