using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace amorphie.fact.core.Constants
{
    public class IphoneModelMapping
    {
        public static readonly string ModelsMap =
        @"
            iPhone1,1 : iPhone|
            iPhone1,2 : iPhone 3G|
            iPhone2,1 : iPhone 3GS|
            iPhone3,1 : iPhone 4|
            iPhone3,2 : iPhone 4|
            iPhone3,3 : iPhone 4|
            iPhone4,1 : iPhone 4S|
            iPhone5,1 : iPhone 5|
            iPhone5,2 : iPhone 5|
            iPhone5,3 : iPhone 5C|
            iPhone5,4 : iPhone 5C|
            iPhone6,1 : iPhone 5S|
            iPhone6,2 : iPhone 5S|
            iPhone7,1 : iPhone 6 Plus|
            iPhone7,2 : iPhone 6|
            iPhone8,1 : iPhone 6s|
            iPhone8,2 : iPhone 6s Plus|
            iPhone8,4 : iPhone SE|
            iPhone9,1 : iPhone 7|
            iPhone9,2 : iPhone 7 Plus|
            iPhone9,3 : iPhone 7|
            iPhone9,4 : iPhone 7 Plus|
            iPhone10,1 : iPhone 8|
            iPhone10,2 : iPhone 8 Plus|
            iPhone10,3 : iPhone X Global|
            iPhone10,4 : iPhone 8|
            iPhone10,5 : iPhone 8 Plus|
            iPhone10,6 : iPhone X|
            iPhone11,2 : iPhone XS|
            iPhone11,4 : iPhone XS Max|
            iPhone11,6 : iPhone XS Max Global|
            iPhone11,8 : iPhone XR|
            iPhone12,1 : iPhone 11|
            iPhone12,3 : iPhone 11 Pro|
            iPhone12,5 : iPhone 11 Pro Max|
            iPhone12,8 : iPhone SE 2nd Gen|
            iPhone13,1 : iPhone 12 Mini|
            iPhone13,2 : iPhone 12|
            iPhone13,3 : iPhone 12 Pro|
            iPhone13,4 : iPhone 12 Pro Max|
            iPhone14,2 : iPhone 13 Pro|
            iPhone14,3 : iPhone 13 Pro Max|
            iPhone14,4 : iPhone 13 Mini|
            iPhone14,5 : iPhone 13|
            iPhone14,6 : iPhone SE 3rd Gen|
            iPhone14,7 : iPhone 14|
            iPhone14,8 : iPhone 14 Plus|
            iPhone15,2 : iPhone 14 Pro|
            iPhone15,3 : iPhone 14 Pro Max|
            iPhone15,4 : iPhone 15|
            iPhone15,5 : iPhone 15 Plus|
            iPhone16,1 : iPhone 15 Pro|
            iPhone16,2 : iPhone 15 Pro Max
        ";
        public static ImmutableDictionary<string,string> Models = ModelsMap.Split("|").ToImmutableDictionary(m => m.Split(":")[0].Replace("\n",string.Empty).Trim(),m => m.Split(":")[1].Replace("\n",string.Empty).Trim());
    }
}