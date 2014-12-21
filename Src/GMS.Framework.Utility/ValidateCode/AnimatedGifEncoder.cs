using System;
using System.Drawing;
using System.IO;

namespace GMS.Framework.Utility
{

    public class AnimatedGifEncoder
    {
        protected int colorDepth;
        protected byte[] colorTab;
        protected int delay;
        protected int dispose = -1;
        protected bool firstFrame = true;
        protected int height;
        protected Image image;
        protected byte[] indexedPixels;
        protected MemoryStream Memory;
        protected int palSize = 7;
        protected byte[] pixels;
        protected int repeat = -1;
        protected int sample = 10;
        protected bool sizeSet;
        protected bool started;
        protected int transIndex;
        protected Color transparent = Color.Empty;
        protected bool[] usedEntry = new bool[0x100];
        protected int width;

        public bool AddFrame(Image im)
        {
            if ((im == null) || !this.started)
            {
                return false;
            }
            bool flag = true;
            try
            {
                if (!this.sizeSet)
                {
                    this.SetSize(im.Width, im.Height);
                }
                this.image = im;
                this.GetImagePixels();
                this.AnalyzePixels();
                if (this.firstFrame)
                {
                    this.WriteLSD();
                    this.WritePalette();
                    if (this.repeat >= 0)
                    {
                        this.WriteNetscapeExt();
                    }
                }
                this.WriteGraphicCtrlExt();
                this.WriteImageDesc();
                if (!this.firstFrame)
                {
                    this.WritePalette();
                }
                this.WritePixels();
                this.firstFrame = false;
            }
            catch (IOException)
            {
                flag = false;
            }
            return flag;
        }

        protected void AnalyzePixels()
        {
            int length = this.pixels.Length;
            int num2 = length / 3;
            this.indexedPixels = new byte[num2];
            NeuQuant quant = new NeuQuant(this.pixels, length, this.sample);
            this.colorTab = quant.Process();
            int num3 = 0;
            for (int i = 0; i < num2; i++)
            {
                int index = quant.Map(this.pixels[num3++] & 0xff, this.pixels[num3++] & 0xff, this.pixels[num3++] & 0xff);
                this.usedEntry[index] = true;
                this.indexedPixels[i] = (byte) index;
            }
            this.pixels = null;
            this.colorDepth = 8;
            this.palSize = 7;
            if (this.transparent != Color.Empty)
            {
                this.transIndex = this.FindClosest(this.transparent);
            }
        }

        protected int FindClosest(Color c)
        {
            if (this.colorTab == null)
            {
                return -1;
            }
            int r = c.R;
            int g = c.G;
            int b = c.B;
            int num4 = 0;
            int num5 = 0x1000000;
            int length = this.colorTab.Length;
            for (int i = 0; i < length; i++)
            {
                int num8 = r - (this.colorTab[i++] & 0xff);
                int num9 = g - (this.colorTab[i++] & 0xff);
                int num10 = b - (this.colorTab[i] & 0xff);
                int num11 = ((num8 * num8) + (num9 * num9)) + (num10 * num10);
                int index = i / 3;
                if (this.usedEntry[index] && (num11 < num5))
                {
                    num5 = num11;
                    num4 = index;
                }
            }
            return num4;
        }

        protected void GetImagePixels()
        {
            int width = this.image.Width;
            int height = this.image.Height;
            if ((width != this.width) || (height != this.height))
            {
                Image image = new Bitmap(this.width, this.height);
                Graphics graphics = Graphics.FromImage(image);
                graphics.DrawImage(this.image, 0, 0);
                this.image = image;
                graphics.Dispose();
            }
            this.pixels = new byte[(3 * this.image.Width) * this.image.Height];
            int index = 0;
            Bitmap bitmap = new Bitmap(this.image);
            for (int i = 0; i < this.image.Height; i++)
            {
                for (int j = 0; j < this.image.Width; j++)
                {
                    Color pixel = bitmap.GetPixel(j, i);
                    this.pixels[index] = pixel.R;
                    index++;
                    this.pixels[index] = pixel.G;
                    index++;
                    this.pixels[index] = pixel.B;
                    index++;
                }
            }
        }

        public void OutPut(ref MemoryStream MemoryResult)
        {
            this.started = false;
            this.Memory.WriteByte(0x3b);
            this.Memory.Flush();
            MemoryResult = this.Memory;
            this.Memory.Close();
            this.Memory.Dispose();
            this.transIndex = 0;
            this.Memory = null;
            this.image = null;
            this.pixels = null;
            this.indexedPixels = null;
            this.colorTab = null;
            this.firstFrame = true;
        }

        public void SetDelay(int ms)
        {
            this.delay = (int) Math.Round((double) (((float) ms) / 10f));
        }

        public void SetDispose(int code)
        {
            if (code >= 0)
            {
                this.dispose = code;
            }
        }

        public void SetFrameRate(float fps)
        {
            if (fps != 0f)
            {
                this.delay = (int) Math.Round((double) (100f / fps));
            }
        }

        public void SetQuality(int quality)
        {
            if (quality < 1)
            {
                quality = 1;
            }
            this.sample = quality;
        }

        public void SetRepeat(int iter)
        {
            if (iter >= 0)
            {
                this.repeat = iter;
            }
        }

        public void SetSize(int w, int h)
        {
            if (!this.started || this.firstFrame)
            {
                this.width = w;
                this.height = h;
                if (this.width < 1)
                {
                    this.width = 320;
                }
                if (this.height < 1)
                {
                    this.height = 240;
                }
                this.sizeSet = true;
            }
        }

        public void SetTransparent(Color c)
        {
            this.transparent = c;
        }

        public void Start()
        {
            this.Memory = new MemoryStream();
            this.WriteString("GIF89a");
            this.started = true;
        }

        protected void WriteGraphicCtrlExt()
        {
            int num;
            int num2;
            this.Memory.WriteByte(0x21);
            this.Memory.WriteByte(0xf9);
            this.Memory.WriteByte(4);
            if (this.transparent == Color.Empty)
            {
                num = 0;
                num2 = 0;
            }
            else
            {
                num = 1;
                num2 = 2;
            }
            if (this.dispose >= 0)
            {
                num2 = this.dispose & 7;
            }
            num2 = num2 << 2;
            this.Memory.WriteByte(Convert.ToByte((int) (num2 | num)));
            this.WriteShort(this.delay);
            this.Memory.WriteByte(Convert.ToByte(this.transIndex));
            this.Memory.WriteByte(0);
        }

        protected void WriteImageDesc()
        {
            this.Memory.WriteByte(0x2c);
            this.WriteShort(0);
            this.WriteShort(0);
            this.WriteShort(this.width);
            this.WriteShort(this.height);
            if (this.firstFrame)
            {
                this.Memory.WriteByte(0);
            }
            else
            {
                this.Memory.WriteByte(Convert.ToByte((int) (0x80 | this.palSize)));
            }
        }

        protected void WriteLSD()
        {
            this.WriteShort(this.width);
            this.WriteShort(this.height);
            this.Memory.WriteByte(Convert.ToByte((int) (240 | this.palSize)));
            this.Memory.WriteByte(0);
            this.Memory.WriteByte(0);
        }

        protected void WriteNetscapeExt()
        {
            this.Memory.WriteByte(0x21);
            this.Memory.WriteByte(0xff);
            this.Memory.WriteByte(11);
            this.WriteString("NETSCAPE2.0");
            this.Memory.WriteByte(3);
            this.Memory.WriteByte(1);
            this.WriteShort(this.repeat);
            this.Memory.WriteByte(0);
        }

        protected void WritePalette()
        {
            this.Memory.Write(this.colorTab, 0, this.colorTab.Length);
            int num = 0x300 - this.colorTab.Length;
            for (int i = 0; i < num; i++)
            {
                this.Memory.WriteByte(0);
            }
        }

        protected void WritePixels()
        {
            new LZWEncoder(this.width, this.height, this.indexedPixels, this.colorDepth).Encode(this.Memory);
        }

        protected void WriteShort(int value)
        {
            this.Memory.WriteByte(Convert.ToByte((int) (value & 0xff)));
            this.Memory.WriteByte(Convert.ToByte((int) ((value >> 8) & 0xff)));
        }

        protected void WriteString(string s)
        {
            char[] chArray = s.ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                this.Memory.WriteByte((byte) chArray[i]);
            }
        }
    }
}

