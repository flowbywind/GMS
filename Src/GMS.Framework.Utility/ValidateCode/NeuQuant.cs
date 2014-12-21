using System;

namespace GMS.Framework.Utility
{
    public class NeuQuant
    {
        protected static readonly int alphabiasshift = 10;
        protected int alphadec;
        protected static readonly int alpharadbias = (((int)1) << alpharadbshift);
        protected static readonly int alpharadbshift = (alphabiasshift + radbiasshift);
        protected static readonly int beta = (intbias >> betashift);
        protected static readonly int betagamma = (intbias << (gammashift - betashift));
        protected static readonly int betashift = 10;
        protected int[] bias = new int[netsize];
        protected int[] freq = new int[netsize];
        protected static readonly int gamma = (((int)1) << gammashift);
        protected static readonly int gammashift = 10;
        protected static readonly int initalpha = (((int)1) << alphabiasshift);
        protected static readonly int initrad = (netsize >> 3);
        protected static readonly int initradius = (initrad * radiusbias);
        protected static readonly int intbias = (((int)1) << intbiasshift);
        protected static readonly int intbiasshift = 0x10;
        protected int lengthcount;
        protected static readonly int maxnetpos = (netsize - 1);
        protected static readonly int minpicturebytes = (3 * prime4);
        protected static readonly int ncycles = 100;
        protected static readonly int netbiasshift = 4;
        protected int[] netindex = new int[0x100];
        protected static readonly int netsize = 0x100;
        protected int[][] network;
        protected static readonly int prime1 = 0x1f3;
        protected static readonly int prime2 = 0x1eb;
        protected static readonly int prime3 = 0x1e7;
        protected static readonly int prime4 = 0x1f7;
        protected static readonly int radbias = (((int)1) << radbiasshift);
        protected static readonly int radbiasshift = 8;
        protected static readonly int radiusbias = (((int)1) << radiusbiasshift);
        protected static readonly int radiusbiasshift = 6;
        protected static readonly int radiusdec = 30;
        protected int[] radpower = new int[initrad];
        protected int samplefac;
        protected byte[] thepicture;

        public NeuQuant(byte[] thepic, int len, int sample)
        {
            this.thepicture = thepic;
            this.lengthcount = len;
            this.samplefac = sample;
            this.network = new int[netsize][];
            for (int i = 0; i < netsize; i++)
            {
                int num2;
                this.network[i] = new int[4];
                int[] numArray = this.network[i];
                numArray[2] = num2 = (i << (netbiasshift + 8)) / netsize;
                numArray[0] = numArray[1] = num2;
                this.freq[i] = intbias / netsize;
                this.bias[i] = 0;
            }
        }

        protected void Alterneigh(int rad, int i, int b, int g, int r)
        {
            int num = i - rad;
            if (num < -1)
            {
                num = -1;
            }
            int netsize = i + rad;
            if (netsize > NeuQuant.netsize)
            {
                netsize = NeuQuant.netsize;
            }
            int num3 = i + 1;
            int num4 = i - 1;
            int num5 = 1;
            while ((num3 < netsize) || (num4 > num))
            {
                int[] numArray;
                int num6 = this.radpower[num5++];
                if (num3 < netsize)
                {
                    numArray = this.network[num3++];
                    try
                    {
                        numArray[0] -= (num6 * (numArray[0] - b)) / alpharadbias;
                        numArray[1] -= (num6 * (numArray[1] - g)) / alpharadbias;
                        numArray[2] -= (num6 * (numArray[2] - r)) / alpharadbias;
                    }
                    catch (Exception)
                    {
                    }
                }
                if (num4 > num)
                {
                    numArray = this.network[num4--];
                    try
                    {
                        numArray[0] -= (num6 * (numArray[0] - b)) / alpharadbias;
                        numArray[1] -= (num6 * (numArray[1] - g)) / alpharadbias;
                        numArray[2] -= (num6 * (numArray[2] - r)) / alpharadbias;
                        continue;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        protected void Altersingle(int alpha, int i, int b, int g, int r)
        {
            int[] numArray = this.network[i];
            numArray[0] -= (alpha * (numArray[0] - b)) / initalpha;
            numArray[1] -= (alpha * (numArray[1] - g)) / initalpha;
            numArray[2] -= (alpha * (numArray[2] - r)) / initalpha;
        }

        public byte[] ColorMap()
        {
            int num;
            byte[] buffer = new byte[3 * netsize];
            int[] numArray = new int[netsize];
            for (num = 0; num < netsize; num++)
            {
                numArray[this.network[num][3]] = num;
            }
            int num2 = 0;
            for (num = 0; num < netsize; num++)
            {
                int index = numArray[num];
                buffer[num2++] = (byte)this.network[index][0];
                buffer[num2++] = (byte)this.network[index][1];
                buffer[num2++] = (byte)this.network[index][2];
            }
            return buffer;
        }

        protected int Contest(int b, int g, int r)
        {
            int num = 0x7fffffff;
            int num2 = num;
            int index = -1;
            int num4 = index;
            for (int i = 0; i < netsize; i++)
            {
                int[] numArray = this.network[i];
                int num6 = numArray[0] - b;
                if (num6 < 0)
                {
                    num6 = -num6;
                }
                int num7 = numArray[1] - g;
                if (num7 < 0)
                {
                    num7 = -num7;
                }
                num6 += num7;
                num7 = numArray[2] - r;
                if (num7 < 0)
                {
                    num7 = -num7;
                }
                num6 += num7;
                if (num6 < num)
                {
                    num = num6;
                    index = i;
                }
                int num8 = num6 - (this.bias[i] >> (intbiasshift - netbiasshift));
                if (num8 < num2)
                {
                    num2 = num8;
                    num4 = i;
                }
                int num9 = this.freq[i] >> betashift;
                this.freq[i] -= num9;
                this.bias[i] += num9 << gammashift;
            }
            this.freq[index] += beta;
            this.bias[index] -= betagamma;
            return num4;
        }

        public void Inxbuild()
        {
            int num;
            int index = 0;
            int num3 = 0;
            for (int i = 0; i < netsize; i++)
            {
                int[] numArray;
                int[] numArray2 = this.network[i];
                int num5 = i;
                int num6 = numArray2[1];
                num = i + 1;
                while (num < netsize)
                {
                    numArray = this.network[num];
                    if (numArray[1] < num6)
                    {
                        num5 = num;
                        num6 = numArray[1];
                    }
                    num++;
                }
                numArray = this.network[num5];
                if (i != num5)
                {
                    num = numArray[0];
                    numArray[0] = numArray2[0];
                    numArray2[0] = num;
                    num = numArray[1];
                    numArray[1] = numArray2[1];
                    numArray2[1] = num;
                    num = numArray[2];
                    numArray[2] = numArray2[2];
                    numArray2[2] = num;
                    num = numArray[3];
                    numArray[3] = numArray2[3];
                    numArray2[3] = num;
                }
                if (num6 != index)
                {
                    this.netindex[index] = (num3 + i) >> 1;
                    num = index + 1;
                    while (num < num6)
                    {
                        this.netindex[num] = i;
                        num++;
                    }
                    index = num6;
                    num3 = i;
                }
            }
            this.netindex[index] = (num3 + maxnetpos) >> 1;
            for (num = index + 1; num < 0x100; num++)
            {
                this.netindex[num] = maxnetpos;
            }
        }

        public void Learn()
        {
            int num;
            int num2;
            if (this.lengthcount < minpicturebytes)
            {
                this.samplefac = 1;
            }
            this.alphadec = 30 + ((this.samplefac - 1) / 3);
            byte[] thepicture = this.thepicture;
            int index = 0;
            int lengthcount = this.lengthcount;
            int num5 = this.lengthcount / (3 * this.samplefac);
            int num6 = num5 / ncycles;
            int initalpha = NeuQuant.initalpha;
            int initradius = NeuQuant.initradius;
            int rad = initradius >> radiusbiasshift;
            if (rad <= 1)
            {
                rad = 0;
            }
            for (num = 0; num < rad; num++)
            {
                this.radpower[num] = initalpha * ((((rad * rad) - (num * num)) * radbias) / (rad * rad));
            }
            if (this.lengthcount < minpicturebytes)
            {
                num2 = 3;
            }
            else if ((this.lengthcount % prime1) != 0)
            {
                num2 = 3 * prime1;
            }
            else if ((this.lengthcount % prime2) != 0)
            {
                num2 = 3 * prime2;
            }
            else if ((this.lengthcount % prime3) != 0)
            {
                num2 = 3 * prime3;
            }
            else
            {
                num2 = 3 * prime4;
            }
            num = 0;
            while (num < num5)
            {
                int b = (thepicture[index] & 0xff) << netbiasshift;
                int g = (thepicture[index + 1] & 0xff) << netbiasshift;
                int r = (thepicture[index + 2] & 0xff) << netbiasshift;
                int i = this.Contest(b, g, r);
                this.Altersingle(initalpha, i, b, g, r);
                if (rad != 0)
                {
                    this.Alterneigh(rad, i, b, g, r);
                }
                index += num2;
                if (index >= lengthcount)
                {
                    index -= this.lengthcount;
                }
                num++;
                if (num6 == 0)
                {
                    num6 = 1;
                }
                if ((num % num6) == 0)
                {
                    initalpha -= initalpha / this.alphadec;
                    initradius -= initradius / radiusdec;
                    rad = initradius >> radiusbiasshift;
                    if (rad <= 1)
                    {
                        rad = 0;
                    }
                    for (i = 0; i < rad; i++)
                    {
                        this.radpower[i] = initalpha * ((((rad * rad) - (i * i)) * radbias) / (rad * rad));
                    }
                }
            }
        }

        public int Map(int b, int g, int r)
        {
            int num = 0x3e8;
            int num2 = -1;
            int index = this.netindex[g];
            int num4 = index - 1;
            while ((index < netsize) || (num4 >= 0))
            {
                int[] numArray;
                int num5;
                int num6;
                if (index < netsize)
                {
                    numArray = this.network[index];
                    num5 = numArray[1] - g;
                    if (num5 >= num)
                    {
                        index = netsize;
                    }
                    else
                    {
                        index++;
                        if (num5 < 0)
                        {
                            num5 = -num5;
                        }
                        num6 = numArray[0] - b;
                        if (num6 < 0)
                        {
                            num6 = -num6;
                        }
                        num5 += num6;
                        if (num5 < num)
                        {
                            num6 = numArray[2] - r;
                            if (num6 < 0)
                            {
                                num6 = -num6;
                            }
                            num5 += num6;
                            if (num5 < num)
                            {
                                num = num5;
                                num2 = numArray[3];
                            }
                        }
                    }
                }
                if (num4 >= 0)
                {
                    numArray = this.network[num4];
                    num5 = g - numArray[1];
                    if (num5 >= num)
                    {
                        num4 = -1;
                    }
                    else
                    {
                        num4--;
                        if (num5 < 0)
                        {
                            num5 = -num5;
                        }
                        num6 = numArray[0] - b;
                        if (num6 < 0)
                        {
                            num6 = -num6;
                        }
                        num5 += num6;
                        if (num5 < num)
                        {
                            num6 = numArray[2] - r;
                            if (num6 < 0)
                            {
                                num6 = -num6;
                            }
                            num5 += num6;
                            if (num5 < num)
                            {
                                num = num5;
                                num2 = numArray[3];
                            }
                        }
                    }
                }
            }
            return num2;
        }

        public byte[] Process()
        {
            this.Learn();
            this.Unbiasnet();
            this.Inxbuild();
            return this.ColorMap();
        }

        public void Unbiasnet()
        {
            for (int i = 0; i < netsize; i++)
            {
                this.network[i][0] = this.network[i][0] >> netbiasshift;
                this.network[i][1] = this.network[i][1] >> netbiasshift;
                this.network[i][2] = this.network[i][2] >> netbiasshift;
                this.network[i][3] = i;
            }
        }
    }
}

