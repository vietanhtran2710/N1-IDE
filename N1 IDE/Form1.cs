using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace N1_IDE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static class CustomizeData
        {
            public static string Theme = "Dark";
        }

        public static class KeyWord
        {
            public static string[] word, def;
        }

        public static class SyntaxColor
        {
            public static Color Key = Color.FromArgb(86, 156, 214);
            public static Color Hashtag = Color.FromArgb(155, 155, 155);
            public static Color String = Color.FromArgb(214, 137, 133);
            public static Color Comment = Color.FromArgb(87, 166, 74);
        }

        public static class Object
        {
            public static bool String, Comment, Hashtag;
        }

        public static class Main
        {
            public static int PreviousLength, CommentType;
            public static string Directory, FileName, CompilerDirectory, EXEDirectory;
            public static bool Saved, No;
            public static char[] Char;
            public static bool[] Processed;
        }

        private void GetDirectory()
        {
            MessageBox.Show("Please choose a directory to store files", "Initialize Directory", MessageBoxButtons.OK, MessageBoxIcon.Information);
            folderBrowserDialog1.ShowDialog();
            Main.Directory = folderBrowserDialog1.SelectedPath;
            MessageBox.Show("Please select the compiler dircetory", "Initialize Directory", MessageBoxButtons.OK, MessageBoxIcon.Information);
            folderBrowserDialog1.ShowDialog();
            Main.CompilerDirectory = folderBrowserDialog1.SelectedPath;
        }

        private void InitializeData()
        {
            string[] data = File.ReadAllLines(Main.EXEDirectory + "setting.ini");
            if (data[0]=="1")
            {
                GetDirectory();
                data[0] = "0"; data[1] = Main.Directory; data[2] = Main.CompilerDirectory;
            }
            Main.Directory = data[1];
            Main.CompilerDirectory = data[2];
            int Index = 0;
            Main.Char = new char[data.Length - 3];
            for (int i=4;i<=data.Length-1;i++)
            {
                Main.Char[Index] = Convert.ToChar(data[i]);
                Index++;
            }
            File.WriteAllLines(Main.EXEDirectory + "setting.ini", data, Encoding.Unicode);
            data = File.ReadAllLines(Main.EXEDirectory + "keyword.txt");
            string[][] SplitKeyWord = new string[data.Length][];
            KeyWord.def = new string[data.Length];
            KeyWord.word = new string[data.Length];
            for (int i = 0;i <= SplitKeyWord.Length - 1;i++)
            {
                SplitKeyWord[i] = new string[2];
            }
            for (int i = 0; i <= data.Length - 1; i++)
                SplitKeyWord[i] = data[i].Split('-');
            for (int i = 0; i <= SplitKeyWord.Length -1 ; i++)
            {
                KeyWord.def[i] = SplitKeyWord[i][0];
                KeyWord.word[i] = SplitKeyWord[i][1];
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Main.EXEDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\";
            BackColor = Color.FromArgb(45, 45, 48);
            richTextBox1.BackColor = Color.FromArgb(30, 30, 30);
            listBox1.BackColor = Color.FromArgb(37, 37, 38);
            WindowState = FormWindowState.Maximized;
            richTextBox1.Width = Size.Width - 34;
            listBox1.Dock = DockStyle.Bottom;
            richTextBox1.Height = Size.Height - listBox1.Height - menuStrip1.Height - 50; 
            Object.String = Object.Comment = Object.Hashtag = false;
            Main.No = false;
            InitializeData();
        }

        private bool IsAlphabetic(char c)
        {
            for (int i = 0; i <= Main.Char.Length - 1; i++)
                if (c == Main.Char[i])
                    return true;
            return false;
        }

        private bool IsKeyWord(string s)
        {
            for (int i=0;i<=KeyWord.word.Length-1;i++)
                if (s == KeyWord.word[i]) return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="State"></param>
        private void ProcessSyntax(int i, string State)
        {
            Main.Processed[i] = true;
            richTextBox1.SelectionLength = 0; int o = richTextBox1.Lines.Length;
            richTextBox1.SelectionColor=Color.White;
            string Line = richTextBox1.Lines[i];
            int Position = richTextBox1.SelectionStart, P;
            P = 0;
            for (int k = 0; k < i; k++)
                P += richTextBox1.Lines[k].Length;
            int j = 0, Last = j;
            if (State=="Comment")
            {
                int p = j + 1;
                while (p < Line.Length - 1)
                    if ((Line[p] == '/') && (Line[p - 1] == '*'))
                        break;
                    else p++;
                if (p >= Line.Length - 1)
                {
                    richTextBox1.Select(P + i, Line.Length);
                    richTextBox1.SelectionColor = SyntaxColor.Comment;
                    Last = p;
                    if (i < richTextBox1.Lines.Length - 1)
                        ProcessSyntax(i + 1, "Comment");
                }
                else
                {
                    richTextBox1.Select(P + i, p - j + 1);
                    richTextBox1.SelectionColor = SyntaxColor.Comment;
                    Last = p;
                }
            }
            if (State=="String")
            {
                int p = j + 1;
                while (p <= Line.Length - 1)
                    if (Line[p] == '"')
                        break;
                    else p++;
                if (p >= Line.Length - 1)
                {
                    richTextBox1.Select(P + i, Line.Length);
                    richTextBox1.SelectionColor = SyntaxColor.String;
                    Last = p;
                    if (i < richTextBox1.Lines.Length - 1)
                        ProcessSyntax(i + 1, "String");
                }
                else
                {
                    richTextBox1.Select(P + i, p - j + 1);
                    richTextBox1.SelectionColor = SyntaxColor.String;
                    Last = p;
                }
            }
            if (State=="")
                if (Line == "") return;
                else
                    while (j<=Line.Length-1)
                    {
                        Last = j;
                        if (IsAlphabetic(Line[j]))
                        {
                            int p = j;
                            while (true)
                                if (p < Line.Length - 1)
                                    if (IsAlphabetic(Line[p + 1]))
                                        p++;
                                    else break;
                                else break;
                            string s = "";
                            for (int a = j; a <= p; a++) s += Line[a];
                            if (IsKeyWord(s))
                            {
                                richTextBox1.Select(j + P + i, p - j + 1);
                                richTextBox1.SelectionColor = SyntaxColor.Key;
                                Last = j + s.Length - 1;
                            }
                            else
                            {
                                richTextBox1.Select(j + P + i, p - j + 1);
                                richTextBox1.SelectionColor = Color.White;
                                Last = j + s.Length - 1;
                            }
                        }
                        else
                        {
                            if (j < Line.Length - 1)
                            {
                                if ((Line[j] == '/') && (Line[j + 1] == '/'))
                                {
                                    richTextBox1.Select(j + P + i, Line.Length - j + 1);
                                    richTextBox1.SelectionColor = SyntaxColor.Comment;
                                    Last = Line.Length;
                                }
                                if ((Line[j] == '/') && (Line[j + 1] == '*'))
                                {
                                    int p = j + 1;
                                    while (p < Line.Length - 1)
                                        if ((Line[p] == '/') && (Line[p - 1] == '*'))
                                            break;
                                        else p++;
                                    if (p >= Line.Length - 1)
                                    {
                                        richTextBox1.Select(j, Line.Length - j + 1);
                                        richTextBox1.SelectionColor = SyntaxColor.Comment;
                                        Last = p;
                                        richTextBox1.SelectionLength = 0;
                                        if (i < richTextBox1.Lines.Length - 1)
                                            ProcessSyntax(i + 1, "Comment");
                                    }
                                    else
                                    {
                                        richTextBox1.Select(j, p - j + 1);
                                        richTextBox1.SelectionColor = SyntaxColor.Comment;
                                        Last = p;
                                    }
                                }
                            }
                            if (Line[j]=='#')
                            {
                                richTextBox1.Select(j + P + i, Line.Length - j + 1);
                                richTextBox1.SelectionColor = SyntaxColor.Hashtag;
                                Last = Line.Length;
                            }
                            if (Line[j]=='"')
                            {
                                int p = j + 1;
                                while (p <= Line.Length - 1)
                                    if (Line[p] == '"')
                                        break;
                                    else p++;
                                if (p >= Line.Length - 1)
                                {
                                    richTextBox1.Select(P + j + i, Line.Length - j + 1);
                                    richTextBox1.SelectionColor = SyntaxColor.String;
                                    Last = p;
                                    if (i < richTextBox1.Lines.Length - 1)
                                        ProcessSyntax(i + 1, "String");
                                }
                                else
                                {
                                    richTextBox1.Select(P + i + j, p - j + 1);
                                    richTextBox1.SelectionColor = SyntaxColor.String;
                                    Last = p;
                                }
                            }
                        }   
                        j = Last + 1;
                    }
            richTextBox1.SelectionStart = Position;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionColor = Color.White;
        }

        private void customizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Customize Form = new Customize();
            Form.Show();
            switch (CustomizeData.Theme)
            {
                case "Dark":
                    richTextBox1.BackColor = Color.FromArgb(30,30,30);
                    richTextBox1.ForeColor = Color.White;
                    break;
                case "Light":
                    richTextBox1.BackColor = Color.White;
                    richTextBox1.ForeColor = Color.Black;
                    break;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Main.Directory;
            openFileDialog1.FileName = "";
            openFileDialog1.Title = "Open a text file or source code file";
            openFileDialog1.Filter = "Text files (*.txt)|*.txt|N++ Source Code Files (*.npp)|*.npp|C Source File|*.c|C++ Source File|*.cpp";
            openFileDialog1.FilterIndex = 2;
            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                string data;
                data = File.ReadAllText(openFileDialog1.FileName);
                richTextBox1.Text = data;
                Main.Processed = new bool[richTextBox1.Lines.Length];
                for (int i = 0; i < richTextBox1.Lines.Length; i++)
                    Main.Processed[i]=false;
                for (int i = 0; i <= richTextBox1.Lines.Length - 1;i++)
                    if (!Main.Processed[i]) ProcessSyntax(i, "");
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Main.Directory;
            saveFileDialog1.Title = "Save File";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|N++ Source Code Files (*.npp)|*.npp|C Source File|*.c|C++ Source File|*.cpp";
            string data = richTextBox1.Text;
            if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                File.WriteAllText(saveFileDialog1.FileName, data);
                Main.Saved = true;
                Main.FileName = saveFileDialog1.FileName;
            }
        }

        private void PushWhiteSpace()
        {
            int Pos = richTextBox1.SelectionStart;
            int Index = richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart), i = 0;
            if (Index > 0)
            {
                string Line = richTextBox1.Lines[Index - 1], tab = "";
                while (i <= Line.Length - 1)
                {
                    while ((Line[i] == '\t') || (Line[i] == ' '))
                    {
                        tab += Line[i];
                        i++;
                        if (i >= Line.Length) break;
                    }
                    break;
                }
                if (tab != "")
                {
                    richTextBox1.SelectedText = tab;
                    richTextBox1.SelectionStart = Pos + tab.Length;
                }
                else richTextBox1.SelectionStart = Pos;
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            Main.Saved = false;
            Main.Processed = new bool[richTextBox1.Lines.Length];
            int Length = richTextBox1.Text.Length;
            int SS;
            if (richTextBox1.SelectionStart == 0) SS = -1;
            else SS = 1;
            richTextBox1.SelectionColor = Color.White;
            if (Length != 0)
            {
                if ((richTextBox1.Text[richTextBox1.SelectionStart - SS] == '\n') && (Length > Main.PreviousLength))
                {
                    PushWhiteSpace();
                    ProcessSyntax(richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart) - 1, "");
                    ProcessSyntax(richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart), "");
                }
                else
                    ProcessSyntax(richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart), "");
            }
            Main.PreviousLength = Length;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About info = new About();
            info.Show();
        }

        public string ChangeFileExtension(string FileName, string Extension)
        {
            string Result = FileName;
            int DotIndex = Result.Length - 1;
            while (Result[DotIndex] != '.') DotIndex--;
            Result = Result.Remove(DotIndex);
            Result = Result + Extension;
            return Result;
        }

        public string QuoteFileName(string FileName)
        {
            return "\"" + FileName + "\"";
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Main.Saved)
                saveToolStripMenuItem_Click(null, null);
            string Code = richTextBox1.Text;
            for (int i = 0; i <= KeyWord.word.Length-1; i++)
                Code = Code.Replace(KeyWord.word[i], KeyWord.def[i]);
            File.WriteAllText(ChangeFileExtension(Main.FileName, ".cpp"), Code);
            Process process = new Process();
            string[] CompileCommand = new string[2];
            CompileCommand[0] = "cd /d " + Main.CompilerDirectory;
            CompileCommand[1] = "g++.exe " + QuoteFileName(ChangeFileExtension(Main.FileName, ".cpp")) + " 2> " + QuoteFileName(ChangeFileExtension(Main.FileName, ".result"));
            File.WriteAllLines("CompileCommand.bat", CompileCommand);
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "CompileCommand.bat";
            process.StartInfo = startInfo;
            process.Start();
            Thread.Sleep(3000);
            string ResultFilePath = ChangeFileExtension(Main.FileName, ".result");
            string result = File.ReadAllText(ResultFilePath);
            if (result == "")
            {
                listBox1.Items.Clear();
                string[] MakeEXECommand = new string[3];
                MakeEXECommand[0] = "cd /d " + Main.CompilerDirectory;
                MakeEXECommand[1] = "g++.exe -c " + QuoteFileName(ChangeFileExtension(Main.FileName, ".cpp")) + " -o " + QuoteFileName(ChangeFileExtension(Main.FileName, ".o"));
                MakeEXECommand[2] = "g++.exe -o " + QuoteFileName(ChangeFileExtension(Main.FileName, ".exe")) + " " + QuoteFileName(ChangeFileExtension(Main.FileName, ".o"));
                File.WriteAllLines("MakeFile.bat", MakeEXECommand);
                ProcessStartInfo startMakeFileInfo = new ProcessStartInfo();
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.FileName = "MakeFile.bat";
                process.StartInfo = startInfo;
                process.Start();
                string EXE = ChangeFileExtension(Main.FileName, ".exe");
                Thread.Sleep(500);
                Process.Start(EXE);
            }
            else
            {
                string[] errorlines = File.ReadAllLines(ResultFilePath);
                for (int i = 0; i <= errorlines.Length - 1; i++)
                    listBox1.Items.Add(errorlines[i]);
            }
        }
    }
}