using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nonowaWindowResize
{
    public partial class Form1 : Form
    {
        Dictionary<string, EnumWindow> nonoWatch;
        string lastScreens;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (Screen s in Screen.AllScreens)
            {
                Console.WriteLine(s.Bounds);
            }
            
            nonoWatch = new Dictionary<string, EnumWindow>();
            lastScreens = getSerializedScreenList();
            screenWatchTimer.Start();
            Hide();
        }

        EnumWindow ew = new EnumWindow();

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ew.load();
            }
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            ew.save();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private string getSerializedScreenList()
        {
            StringBuilder sb = new StringBuilder();
            Screen[] s = System.Windows.Forms.Screen.AllScreens;
            for (int i = 0; i < s.Length; i++)
            {
                Rectangle r = s[i].Bounds;
                sb.Append(String.Format("{0}:({1},{2})-({3},{4}) ", i, r.Left, r.Top, r.Right, r.Bottom));
            }
            return sb.ToString();
        }

        private void screenWatchTimer_Tick(object sender, EventArgs e)
        {
            string scr = getSerializedScreenList(); 
            if (lastScreens == scr)
            {
                EnumWindow w;
                if (!nonoWatch.ContainsKey(scr))
                {
                    w = new EnumWindow();
                    nonoWatch.Add(scr, w);
                }
                else
                {
                    w = nonoWatch[scr];
                }
                w.save();
            }
            else
            {
                if (nonoWatch.ContainsKey(scr))
                {
                    screenWatchTimer.Stop();
                    System.Threading.Thread.Sleep(5000);
                    nonoWatch[scr].load();
                    screenWatchTimer.Start();
                }
            }
            lastScreens = scr;
        }

    }
}
