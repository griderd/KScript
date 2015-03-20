using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KScript.Hardware.GraphicsAdapters
{
    public class MonochromeDisplayAdapter : DisplayAdapter
    {
        private object lockObject = new object();

        Bitmap charmap;
        Graphics g;

        public void Snapshot()
        {
            Bitmap image = Rasterize();
            image.Save("screenshot.bmp");
        }

        public MonochromeDisplayAdapter(bool debugMode = false)
            : base(2000, new Size(720, 400), debugMode)
        {
            charmap = new Bitmap("codepage.bmp");
            g = Graphics.FromImage(raster);
        }

        private void WriteString(string s, int address)
        {
            Write(address, Encoding.ASCII.GetBytes(s));
        }

        protected override void InitiateDebugMode()
        {
            WriteString("The display adapter is in debug mode.", 0);
        }

        public override Bitmap Rasterize()
        {
            int cellWidth = 9;
            int cellHeight = 16;

            g.Clear(Color.Black);
            for (int row = 0; row < 25; row++)
            {
                for (int column = 0; column < 80; column++)
                {
                    byte cnum = data[column + (80 * row)];
                    if (cnum == 0)
                        continue;

                    int mapRow = cnum / 32;
                    int mapColumn = cnum % 32;

                    int sourceX = mapColumn * cellWidth;
                    int sourceY = mapRow * cellHeight;
                    int destX = column * cellWidth;
                    int destY = row * cellHeight;

                    //g.DrawImage(charmap, (float)(mapColumn * cellWidth), (float)(mapRow * cellHeight), (float)cellWidth, (float)cellHeight);
                    g.DrawImage(charmap, new Rectangle(destX, destY, cellWidth, cellHeight), new Rectangle(sourceX, sourceY, cellWidth, cellHeight), GraphicsUnit.Pixel);
                }
            }
            return raster;
        }

        public override void Dispose()
        {
            base.Dispose();
            charmap.Dispose();
            g.Dispose();
        }

        protected override void _Start()
        {
            Clear();
        }

        protected override void _Stop()
        {
            Clear();
        }

        public override void WriteWord(int address, int value)
        {
            if ((address < 0) | (address >= data.Length))
            {
                RaiseError(Interrupts.HardwareInterruptType.SegmentationFault);
                return;
            }

            data[address] = (byte)value;
        }
    }
}
