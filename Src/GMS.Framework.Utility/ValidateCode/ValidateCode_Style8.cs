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
    /// 2年级算术(蓝色) 
    /// </summary>
    public class ValidateCode_Style8 : ValidateCodeType
    {
        private Color backgroundColor = Color.White;
        private Color chaosColor = Color.FromArgb(170, 170, 0x33);
        private Color drawColor = Color.FromArgb(50, 0x99, 0xcc);
        private bool fontTextRenderingHint;
        private int imageHeight = 30;
        private int padding = 1;
        private int validataCodeLength = 5;
        private int validataCodeSize = 0x10;
        private string validateCodeFont = "Arial";

        public override byte[] CreateImage(out string resultCode)
        {
            string str2;
            Bitmap bitmap;
            string formatString = "1,2,3,4,5,6,7,8,9,0";
            GetRandom(formatString, out str2, out resultCode);
            MemoryStream stream = new MemoryStream();
            this.ImageBmp(out bitmap, str2);
            bitmap.Save(stream, ImageFormat.Png);
            bitmap.Dispose();
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
            Brush brush = new SolidBrush(this.drawColor);
            int maxValue = Math.Max((this.ImageHeight - this.validataCodeSize) - 5, 0);
            Random random = new Random();
            for (int i = 0; i < this.validataCodeLength; i++)
            {
                int[] numArray = new int[] { (i * this.validataCodeSize) + (i * 5), random.Next(maxValue) };
                Point point = new Point(numArray[0], numArray[1]);
                graphics.DrawString(validateCode[i].ToString(), font, brush, (PointF) point);
            }
            graphics.Dispose();
        }

        private void DisposeImageBmp(ref Bitmap bitmap)
        {
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            Pen pen = new Pen(this.DrawColor, 1f);
            Random random = new Random();
            pen = new Pen(this.ChaosColor, 1f);
            for (int i = 0; i < (this.validataCodeLength * 10); i++)
            {
                int x = random.Next(bitmap.Width);
                int y = random.Next(bitmap.Height);
                graphics.DrawRectangle(pen, x, y, 1, 1);
            }
            graphics.Dispose();
        }

        private static void GetRandom(string formatString, out string codeString, out string resultCode)
        {
            Random random = new Random();
            string s = string.Empty;
            string str2 = string.Empty;
            string[] strArray = formatString.Split(new char[] { ',' });
            for (int i = 0; i < 2; i++)
            {
                int index = random.Next(strArray.Length);
                if ((i == 0) && (strArray[index] == "0"))
                {
                    i--;
                }
                else
                {
                    s = s + strArray[index].ToString();
                }
            }
            for (int j = 0; j < 2; j++)
            {
                int num4 = random.Next(strArray.Length);
                if ((j == 0) && (strArray[num4] == "0"))
                {
                    j--;
                }
                else
                {
                    str2 = str2 + strArray[num4].ToString();
                }
            }
            if ((random.Next(100) % 2) == 1)
            {
                codeString = s + "+" + str2;
                resultCode = (int.Parse(s) + int.Parse(str2)).ToString();
            }
            else
            {
                if (int.Parse(s) > int.Parse(str2))
                {
                    codeString = s + "─" + str2;
                }
                else
                {
                    codeString = str2 + "─" + s;
                }
                resultCode = Math.Abs((int) (int.Parse(s) - int.Parse(str2))).ToString();
            }
        }

        private void ImageBmp(out Bitmap bitMap, string validataCode)
        {
            int width = (int) (((this.validataCodeLength * this.validataCodeSize) * 1.3) + 10.0);
            bitMap = new Bitmap(width, this.ImageHeight);
            this.DisposeImageBmp(ref bitMap);
            this.CreateImageBmp(ref bitMap, validataCode);
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

        public Color DrawColor
        {
            get
            {
                return this.drawColor;
            }
            set
            {
                this.drawColor = value;
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
                return "2年级算术(蓝色)";
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

        public override string Tip
        {
            get
            {
                return "输入计算结果";
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

