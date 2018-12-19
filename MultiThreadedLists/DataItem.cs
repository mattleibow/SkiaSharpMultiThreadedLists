using System;
using Xamarin.Forms;

namespace MultiThreadedLists
{
    public class DataItem
    {
        private static readonly Random random = new Random();

        public DataItem(int number)
        {
            Number = number.ToString();

            var components = new byte[3];
            random.NextBytes(components);
            Color = Color.FromRgb(components[0], components[1], components[2]);
        }

        public string Number { get; set; }

        public Color Color { get; set; }
    }
}
