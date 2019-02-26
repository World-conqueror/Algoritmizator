using System;
using System.Windows.Forms;
using Microsoft.Office.Interop.VisOcx;
namespace Algoritmizator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Bloxator blox = new Bloxator();
            blox.TextCode = textBox1.Text;
            blox.CreateShape(textBox1.Text);
            BlockMethod blokMethod = blox.GetComplietShape();
            Designer.GenerateDiagram(blokMethod);
        }
    }
}