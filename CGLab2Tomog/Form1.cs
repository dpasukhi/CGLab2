using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGLab2Tomog
{
    public partial class Form1 : Form
    {
        bool IsLoad;
        View MyView;
        int MyLauer;
        Bin MyBin;
        bool NeedReload;
        int FrameCount;
        DateTime NextFPSUpdate;
        public Form1()
        {
            InitializeComponent();
            IsLoad = false;
            MyView = new View();
            MyLauer = 0;
            MyBin = new Bin();
            NeedReload = true;
            NextFPSUpdate = DateTime.Now.AddSeconds(1);
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog Mydialog = new OpenFileDialog();
            if (Mydialog.ShowDialog() == DialogResult.OK)
            {
                string path = Mydialog.FileName;
                MyBin.ReadBIN(path);
                View.SetupView(glControl1.Width, glControl1.Height);
                trackBar1.Maximum = Bin.z - 1;
                IsLoad = true;
                glControl1.Invalidate();
            }
        }
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (IsLoad)
            {
                if (Quads.Checked)
                    MyView.DrawQuads(MyLauer);
                else
                {
                    if (NeedReload)
                    {
                        MyView.generateTextureImage(MyLauer);
                        MyView.Load2DTexture();
                        NeedReload = false;
                    }
                    MyView.DrawTexture();
                }
                glControl1.SwapBuffers();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            MyLauer = trackBar1.Value;
            NeedReload = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Idle += Application_Idle;
        }
        private void Application_Idle(object sender, EventArgs e)
        {
            while (glControl1.IsIdle)
            {
                displayFPS();
                glControl1.Invalidate();
            }
        }
        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps={0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }
    }
}
