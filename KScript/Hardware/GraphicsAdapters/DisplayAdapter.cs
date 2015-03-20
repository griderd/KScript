using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace KScript.Hardware.GraphicsAdapters
{
    public abstract class DisplayAdapter : DataStore, IDisposable 
    {
        protected Bitmap raster;
        protected Size pixelResolution;
        protected bool debugMode;

        public Size PixelResolution { get { return pixelResolution; } }

        public DisplayAdapter(int memorySize, Size pixelResolution, bool debugMode = false)
            : base(memorySize, HardwareType.VideoDevice)
        {
            this.pixelResolution = pixelResolution;
            raster = new Bitmap(pixelResolution.Width, pixelResolution.Height);

            if (debugMode) InitiateDebugMode();
        }

        protected abstract void InitiateDebugMode();

        /// <summary>
        /// Generates the next frame of video.
        /// </summary>
        /// <returns></returns>
        public abstract Bitmap Rasterize();

        public abstract void WriteWord(int address, int value);

        public virtual void Dispose()
        {
            raster.Dispose();
        }
    }
}
