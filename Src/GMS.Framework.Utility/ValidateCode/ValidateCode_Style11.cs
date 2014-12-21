using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

namespace GMS.Framework.Utility
{
    /// <summary>
    /// GIF闪烁动画(彩色)
    /// </summary>
    public class ValidateCode_Style11 : ValidateCodeType
    {
        private Color backgroundColor = Color.White;
        private bool chaos = true;
        private Color chaosColor = Color.FromArgb(170, 170, 0x33);
        private int chaosMode = 1;
        private List<Color> colors = new List<Color>();
        private Color[] drawColors = new Color[] { Color.FromArgb(0x6b, 0x42, 0x26), Color.FromArgb(0x4f, 0x2f, 0x4f), Color.FromArgb(50, 0x99, 0xcc), Color.FromArgb(0xcd, 0x7f, 50), Color.FromArgb(0x23, 0x23, 0x8e), Color.FromArgb(0x70, 0xdb, 0x93), Color.Red, Color.FromArgb(0xbc, 0x8f, 0x8e) };
        private bool fontTextRenderingHint;
        private int imageHeight = 30;
        private int padding = 1;
        private int validataCodeLength = 4;
        private int validataCodeSize = 0x10;
        private string validateCodeFont = "Arial";

        public override byte[] CreateImage(out string validataCode)
        {
            Bitmap bitmap;
            string formatString = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            GetRandom(formatString, this.ValidataCodeLength, out validataCode);
            MemoryStream stream = new MemoryStream();
            AnimatedGifEncoder encoder = new AnimatedGifEncoder();
            encoder.Start();
            encoder.SetDelay(1);
            encoder.SetRepeat(0);
            Random random = new Random();
            for (int i = 0; i < validataCode.Length; i++)
            {
                this.colors.Add(this.DrawColors[random.Next(this.DrawColors.Length)]);
            }
            for (int j = 0; j < 3; j++)
            {
                string[] strArray = this.SplitCode(validataCode);
                for (int k = 0; k < 2; k++)
                {
                    if (k == 0)
                    {
                        this.ImageBmp(out bitmap, strArray[0]);
                    }
                    else
                    {
                        this.ImageBmp(out bitmap, strArray[1]);
                    }
                    bitmap.Save(stream, ImageFormat.Png);
                    encoder.AddFrame(Image.FromStream(stream));
                    stream = new MemoryStream();
                    bitmap.Dispose();
                }
            }
            encoder.OutPut(ref stream);
            bitmap = null;
            stream.Close();
            stream.Dispose();
            return stream.GetBuffer();
        }

        private void CreateImageBmp(ref Bitmap bitMap, string validateCode)
        {
            Graphics graphics = Graphics.FromImage(bitMap);
            if (this.fontTextRenderingHint)
            {
                graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
            }
            else
            {
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            }
            Font font = new Font(this.validateCodeFont, (float) this.validataCodeSize, FontStyle.Regular);
            int maxValue = Math.Max((this.ImageHeight - this.validataCodeSize) - 4, 0);
            Random random = new Random();
            for (int i = 0; i < this.validataCodeLength; i++)
            {
                Brush brush = new SolidBrush(this.colors[i]);
                int[] numArray = new int[] { ((i * this.validataCodeSize) + random.Next(1)) + 3, random.Next(maxValue) - 4 };
                Point point = new Point(numArray[0], numArray[1]);
                graphics.DrawString(validateCode[i].ToString(), font, brush, (PointF) point);
            }
            graphics.Dispose();
        }

        private void DisposeImageBmp(ref Bitmap bitmap)
        {
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            Random random = new Random();
            Point[] pointArray = new Point[2];
            if (this.Chaos)
            {
                Pen pen;
                switch (this.chaosMode)
                {
                    case 1:
                        pen = new Pen(this.ChaosColor, 1f);
                        for (int i = 0; i < (this.validataCodeLength * 10); i++)
                        {
                            int x = random.Next(bitmap.Width);
                            int y = random.Next(bitmap.Height);
                            graphics.DrawRectangle(pen, x, y, 1, 1);
                        }
                        break;

                    case 2:
                        pen = new Pen(this.ChaosColor, (float) (this.validataCodeLength * 4));
                        for (int j = 0; j < (this.validataCodeLength * 10); j++)
                        {
                            int num5 = random.Next(bitmap.Width);
                            int num6 = random.Next(bitmap.Height);
                            graphics.DrawRectangle(pen, num5, num6, 1, 1);
                        }
                        break;

                    case 3:
                        pen = new Pen(this.ChaosColor, 1f);
                        for (int k = 0; k < (this.validataCodeLength * 2); k++)
                        {
                            pointArray[0] = new Point(random.Next(bitmap.Width), random.Next(bitmap.Height));
                            pointArray[1] = new Point(random.Next(bitmap.Width), random.Next(bitmap.Height));
                            graphics.DrawLine(pen, pointArray[0], pointArray[1]);
                        }
                        break;

                    default:
                        pen = new Pen(this.ChaosColor, 1f);
                        for (int m = 0; m < (this.validataCodeLength * 10); m++)
                        {
                            int num9 = random.Next(bitmap.Width);
                            int num10 = random.Next(bitmap.Height);
                            graphics.DrawRectangle(pen, num9, num10, 1, 1);
                        }
                        break;
                }
            }
            graphics.Dispose();
        }

        private static void GetRandom(string formatString, int len, out string codeString)
        {
            codeString = string.Empty;
            string[] strArray = formatString.Split(new char[] { ',' });
            Random random = new Random();
            for (int i = 0; i < len; i++)
            {
                int index = random.Next(0x186a0) % strArray.Length;
                codeString = codeString + strArray[index].ToString();
            }
        }

        private void ImageBmp(out Bitmap bitMap, string validataCode)
        {
            int width = (int) (((this.validataCodeLength * this.validataCodeSize) * 1.3) + 4.0);
            bitMap = new Bitmap(width, this.ImageHeight);
            this.DisposeImageBmp(ref bitMap);
            this.CreateImageBmp(ref bitMap, validataCode);
        }

        private string[] SplitCode(string srcCode)
        {
            Random random = new Random();
            string[] strArray = new string[2];
            foreach (char ch in srcCode)
            {
                if ((random.Next(Math.Abs((int)DateTime.Now.Ticks)) % 2) == 0)
                {
                    string[] strArray2;
                    string[] strArray3;
                    (strArray2 = strArray)[0] = strArray2[0] + ch.ToString();
                    (strArray3 = strArray)[1] = strArray3[1] + " ";
                }
                else
                {
                    string[] strArray4;
                    string[] strArray5;
                    (strArray4 = strArray)[1] = strArray4[1] + ch.ToString();
                    (strArray5 = strArray)[0] = strArray5[0] + " ";
                }
            }
            return strArray;
        }

        public Color BackgroundColor
        {
            get
            {
                return this.backgroundColor;
            }
            set
            {
                this.backgroundColor = value;
            }
        }

        public bool Chaos
        {
            get
            {
                return this.chaos;
            }
            set
            {
                this.chaos = value;
            }
        }

        public Color ChaosColor
        {
            get
            {
                return this.chaosColor;
            }
            set
            {
                this.chaosColor = value;
            }
        }

        public int ChaosMode
        {
            get
            {
                return this.chaosMode;
            }
            set
            {
                this.chaosMode = value;
            }
        }

        public Color[] DrawColors
        {
            get
            {
                return this.drawColors;
            }
            set
            {
                this.drawColors = value;
            }
        }

        private bool FontTextRenderingHint
        {
            get
            {
                return this.fontTextRenderingHint;
            }
            set
            {
                this.fontTextRenderingHint = value;
            }
        }

        public int ImageHeight
        {
            get
            {
                return this.imageHeight;
            }
            set
            {
                this.imageHeight = value;
            }
        }

        public override string Name
        {
            get
            {
                return "GIF闪烁动画(彩色)";
            }
        }

        public int Padding
        {
            get
            {
                return this.padding;
            }
            set
            {
                this.padding = value;
            }
        }

        public int ValidataCodeLength
        {
            get
            {
                return this.validataCodeLength;
            }
            set
            {
                this.validataCodeLength = value;
            }
        }

        public int ValidataCodeSize
        {
            get
            {
                return this.validataCodeSize;
            }
            set
            {
                this.validataCodeSize = value;
            }
        }

        public string ValidateCodeFont
        {
            get
            {
                return this.validateCodeFont;
            }
            set
            {
                this.validateCodeFont = value;
            }
        }
    }
}

