using System;
using System.Collections.Generic;
using System.Linq;

namespace Kata
{
    public class Kata
    {
        public static string DuplicateEncode(string word)
        {
            var a = " ";
            return string.Join(' ', string.Join(string.Empty, a.Split(" ").Select(sign => sign == "" ? " " : "m"))
        .Split(' ')
        .Aggregate()
        .Select(word => word.Normalize())); ;
           
            return word;

            word.ToCharArray().ToList().Reverse()
        }
    }
}
