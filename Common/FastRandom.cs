using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public class FastRandom
    {
        private long _seed;

        public FastRandom(long seed)
        {
            this._seed = seed;
        }

        long randomLong()
        {
            _seed ^= (_seed << 21);
            _seed ^= (_seed >> 35) & 0xFF;
            _seed ^= (_seed << 4);
            return _seed;
        }

        public int randomInt()
        {
            return (int)randomLong();
        }

        public int randomInt(int range)
        {
            return (int)randomLong() % range;
        }

        public int randomIntAbs()
        {
            return fastAbs(randomInt());
        }

        public int randomIntAbs(int range)
        {
            return fastAbs(randomInt() % range);
        }

        public double randomDouble()
        {
            return randomLong() / ((double)long.MaxValue - 1d);
        }

        public float randomFloat()
        {
            return randomLong() / ((float)long.MaxValue - 1f);
        }

        public float randomPosFloat()
        {
            return 0.5f * (randomFloat() + 1.0f);
        }

        public bool randomBoolean()
        {
            return randomLong() > 0;
        }

        public String randomCharacterString(int length)
        {
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < length / 2; i++)
            {
                s.Append((char)('a' + fastAbs(randomDouble()) * 26d));
                s.Append((char)('A' + fastAbs(randomDouble()) * 26d));
            }

            return s.ToString();
        }

        public double standNormalDistrDouble()
        {

            double q = Double.MaxValue;
            double u1 = 0;
            double u2;

            while (q >= 1d || q == 0)
            {
                u1 = randomDouble();
                u2 = randomDouble();

                q = Math.Pow(u1, 2) + Math.Pow(u2, 2);
            }

            double p = Math.Sqrt((-2d * (Math.Log(q))) / q);
            return u1 * p;
        }

        public static int fastAbs(int i)
        {
            return (i >= 0) ? i : -i;
        }

        public static float fastAbs(float d)
        {
            return (d >= 0) ? d : -d;
        }

        public static double fastAbs(double d)
        {
            return (d >= 0) ? d : -d;
        }
    }

    public struct SFastRandom
    {
        private long _seed;

        public SFastRandom(long seed)
        {
            this._seed = seed;
        }

        long randomLong()
        {
            _seed ^= (_seed << 21);
            _seed ^= (_seed >> 35) & 0xFF;
            _seed ^= (_seed << 4);
            return _seed;
        }

        public int randomInt()
        {
            return (int)randomLong();
        }

        public int randomInt(int range)
        {
            return (int)randomLong() % range;
        }

        public int randomIntAbs()
        {
            return fastAbs(randomInt());
        }

        public int randomIntAbs(int range)
        {
            return fastAbs(randomInt() % range);
        }

        public double randomDouble()
        {
            return randomLong() / ((double)long.MaxValue - 1d);
        }

        public float randomFloat()
        {
            return randomLong() / ((float)long.MaxValue - 1f);
        }

        public float randomPosFloat()
        {
            return 0.5f * (randomFloat() + 1.0f);
        }

        public bool randomBoolean()
        {
            return randomLong() > 0;
        }

        public String randomCharacterString(int length)
        {
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < length / 2; i++)
            {
                s.Append((char)('a' + fastAbs(randomDouble()) * 26d));
                s.Append((char)('A' + fastAbs(randomDouble()) * 26d));
            }

            return s.ToString();
        }

        public double standNormalDistrDouble()
        {

            double q = Double.MaxValue;
            double u1 = 0;
            double u2;

            while (q >= 1d || q == 0)
            {
                u1 = randomDouble();
                u2 = randomDouble();

                q = Math.Pow(u1, 2) + Math.Pow(u2, 2);
            }

            double p = Math.Sqrt((-2d * (Math.Log(q))) / q);
            return u1 * p;
        }

        public static int fastAbs(int i)
        {
            return (i >= 0) ? i : -i;
        }

        public static float fastAbs(float d)
        {
            return (d >= 0) ? d : -d;
        }

        public static double fastAbs(double d)
        {
            return (d >= 0) ? d : -d;
        }
    }
}
