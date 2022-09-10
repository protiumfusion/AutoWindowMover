using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_SHOWWINDOW = 0x0040;

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        private bool _canShow = false;


        public Form1()
        {
            InitializeComponent();

            // loading settings into textboxes from file
            using (StreamReader readtext = new StreamReader("AWM_settings.dat"))
            {
                textBox1.Text = readtext.ReadLine();
                textBox2.Text = readtext.ReadLine();
                textBox3.Text = readtext.ReadLine();
                textBox4.Text = readtext.ReadLine();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    string wndTitle = process.MainWindowTitle;
                    listBox1.Items.Add(wndTitle);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // save settings from textboxes to file
            using (StreamWriter writer = new StreamWriter("AWM_settings.dat"))
            {
                writer.WriteLine(textBox1.Text);
                writer.WriteLine(textBox2.Text);
                writer.WriteLine(textBox3.Text);
                writer.WriteLine(textBox4.Text);
            }

            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    string wndTitle = process.MainWindowTitle;
                    if (wndTitle.Contains(textBox1.Text))
                    {
                        IntPtr handle = process.MainWindowHandle;
                        if (handle != IntPtr.Zero)
                        {
                            RECT rct;
                            GetWindowRect(handle, out rct);
                            Rectangle screen = Screen.FromHandle(handle).Bounds;
                            Point pt = new Point(screen.Left + screen.Width / 2 - (rct.Right - rct.Left) / 2, screen.Top + screen.Height / 2 - (rct.Bottom - rct.Top) / 2);
                            SetWindowPos(handle, IntPtr.Zero, Int32.Parse(textBox3.Text), pt.Y, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                        }

                    }
                    else if (wndTitle.Contains(textBox2.Text))
                    {
                        IntPtr handle = process.MainWindowHandle;
                        if (handle != IntPtr.Zero)
                        {
                            RECT rct;
                            GetWindowRect(handle, out rct);
                            Rectangle screen = Screen.FromHandle(handle).Bounds;
                            Point pt = new Point(screen.Left + screen.Width / 2 - (rct.Right - rct.Left) / 2, screen.Top + screen.Height / 2 - (rct.Bottom - rct.Top) / 2);
                            SetWindowPos(handle, IntPtr.Zero, Int32.Parse(textBox4.Text), pt.Y, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
                        }

                    }

                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        protected override void SetVisibleCore(bool value)
        {
            if (_canShow)
            {
                base.SetVisibleCore(value);
            }
            else
            {
                base.SetVisibleCore(false);
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _canShow = true;
            Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                _canShow = false;
                Visible = false;
            }
        }
    }
}
