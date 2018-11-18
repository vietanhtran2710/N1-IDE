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

namespace N1_IDE
{
    public partial class Customize : Form
    {
        public Customize()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Customize_Load(object sender, EventArgs e)
        {
            textBox1.Text = Form1.Main.Directory;
            textBox2.Text = Form1.Main.CompilerDirectory;
            string data = File.ReadAllText(Form1.Main.EXEDirectory + "keyword.txt");
            textBox3.Text = data;
            comboBox1.Items.Add("Dark");
            comboBox1.Items.Add("Light");
            if (Form1.CustomizeData.Theme == "Dark")
                comboBox1.SelectedItem = comboBox1.Items[0];
            else comboBox1.SelectedItem = comboBox1.Items[1];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox2.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form1.Main.Directory = textBox1.Text;
            Form1.Main.CompilerDirectory = textBox2.Text;
            string[] data = textBox3.Lines;
            string[][] SplitKeyWord = new string[data.Length][];
            Form1.KeyWord.def = new string[data.Length];
            Form1.KeyWord.word = new string[data.Length];
            for (int i = 0; i <= SplitKeyWord.Length - 1; i++)
            {
                SplitKeyWord[i] = new string[2];
            }
            for (int i = 0; i <= data.Length - 1; i++)
                SplitKeyWord[i] = data[i].Split('-');
            for (int i = 0; i <= SplitKeyWord.Length - 1; i++)
            {
                Form1.KeyWord.def[i] = SplitKeyWord[i][0];
                Form1.KeyWord.word[i] = SplitKeyWord[i][1];
            }
            Form1.CustomizeData.Theme = comboBox1.SelectedItem.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form1.Main.Directory = textBox1.Text;
            Form1.Main.CompilerDirectory = textBox2.Text;
            string[] data = textBox3.Lines;
            string[][] SplitKeyWord = new string[data.Length][];
            Form1.KeyWord.def = new string[data.Length];
            Form1.KeyWord.word = new string[data.Length];
            for (int i = 0; i <= SplitKeyWord.Length - 1; i++)
            {
                SplitKeyWord[i] = new string[2];
            }
            for (int i = 0; i <= data.Length - 1; i++)
                SplitKeyWord[i] = data[i].Split('-');
            for (int i = 0; i <= SplitKeyWord.Length - 1; i++)
            {
                Form1.KeyWord.def[i] = SplitKeyWord[i][0];
                Form1.KeyWord.word[i] = SplitKeyWord[i][1];
            }
            Form1.CustomizeData.Theme = comboBox1.SelectedItem.ToString();
            Close();
        }
    }
}
