using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
namespace CMPH_BlogProject.Helper
{
    public class RandomGenerator

    {

        public int RandomItem { get; set; }
    }

        public static class CollectionExtension
        {
            private static Random item = new Random();

            public static T RandomItem<T>(this IEnumerable<T> model)
            {                    
                return model.ElementAt(item.Next(0, model.Count()));
            }

            public static T RandomItem<T>(this T[] array)
            {
                return array[item.Next(array.Length)];
            }
        }
    
}