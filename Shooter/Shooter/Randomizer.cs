using System;

namespace Shooter
{
    public static class Randomizer
    {
        private static Random _random;

        public static void Setup()
        {
            _random = new Random((int)State.GameTime.ElapsedGameTime.Ticks * DateTime.Now.Millisecond);
        }
        public static Int32 Next(Int32 max)
        {
            return _random.Next(max);
        }

        public static Int32 Next(float low, float high)
        {
            return Next((int)low, (int)high);
        }

        public static Int32 Next(Int32 low, Int32 high)
        {
            return low + Next(high - low);
        }
    }
}
