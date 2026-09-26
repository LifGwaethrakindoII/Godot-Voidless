using Godot;
using System;
using System.Collections.Generic;

namespace Voidless
{
    public static class VCollection
    {
        public static bool IsNullOrEmpty<T>(this T[] array)
        {
            return (array == null || array.Length <= 0);
        }

        /*public static T[] WithConcatenated(this T[] array, params T[] aggregate)
        {
            if(array.IsNullOrEmpty() || aggregate.IsNullOrEmpty()) return null;

            T[] newArray = new T[array.Length + aggregate.Length];


        }*/
    }
}