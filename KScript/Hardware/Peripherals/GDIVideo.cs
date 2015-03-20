using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KScript.Hardware.GraphicsAdapters;

namespace KScript.Hardware.Peripherals
{
    public partial class GDIVideo : Form
    {
        DisplayAdapter videoCard;
        Bitmap screen;

        public GDIVideo(DisplayAdapter videoCard)
        {
            InitializeComponent();
            this.videoCard = videoCard;
            Width = videoCard.PixelResolution.Width;
            Height = videoCard.PixelResolution.Height;

            timer1.Start();
        }

        private void GDIVideo_Load(object sender, EventArgs e)
        {
            screen = videoCard.Rasterize();
            BackgroundImage = screen;
            DoubleBuffered = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            screen = videoCard.Rasterize();
            Invalidate();
        }
    }
}
