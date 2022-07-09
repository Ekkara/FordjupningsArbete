using System;
using System.Numerics;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Ivec3 
{
    public int x, y, z;
    public Ivec3(int xValue, int yValue, int zValue)
    {
        x = xValue;
        y = yValue;
        z = zValue;
    }
    public Ivec3()
    {
        x = 0;
        y = 0;
    }
    public Ivec3(UnityEngine.Vector3 vector3)
    {
        x = (int)vector3.x;
        y = (int)vector3.y;
        z = (int)vector3.z;
    }
    public double magnitude
    {
        get { return getMagnitude(); }
    }
    double getMagnitude()
    {
        return Math.Abs(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
    }
    public Ivec3 normalized
    {
        get { return getNormalized(); }
    }
    Ivec3 getNormalized()
    {
        double mag = getMagnitude();
        return new Ivec3((int)(x / mag), (int)(y / mag),(int)(z / mag));
    }
    public string values
    {
        get { return x + ", " + y + ", " + z; }
    }
    public UnityEngine.Vector2 toVector2
    {
        get { return new UnityEngine.Vector2((float)x, (float)y); }
    }
    public UnityEngine.Vector3 toVector3
    {
        get { return new UnityEngine.Vector3((float)x, (float)y, 0); }
    }

    //deffine how my vector will handle operations
    public static Ivec3 operator +(Ivec3 a) => a;
    public static Ivec3 operator -(Ivec3 a) => a;// new Ivec3(-a.x, -a.y, -a.z);

    public static Ivec3 operator +(Ivec3 a, Ivec3 b)
        => new Ivec3(a.x + b.x, a.y + b.y, a.z + b.z);
    public static Ivec3 operator +(Ivec3 a, int b)
        => new Ivec3(a.x + b, a.y + b, a.z + b);
    public static Ivec3 operator +(int b, Ivec3 a)
        => new Ivec3(a.x + b, a.y + b, a.z +b);
    public static Ivec3 operator -(Ivec3 a, Ivec3 b)
        => new Ivec3(a.x - b.x, a.y - b.y, a.z - b.z);
    public static Ivec3 operator -(Ivec3 a, int b)
        => new Ivec3(a.x - b, a.y - b, a.z - b);
    public static Ivec3 operator *(Ivec3 a, Ivec3 b)
        => new Ivec3(a.x * b.x, a.y * b.y, a.z * b.z);
    public static Ivec3 operator *(Ivec3 a, int b)
      => new Ivec3(a.x * b, a.y * b, a.z * b);
    public static Ivec3 operator *(int b, Ivec3 a)
      => new Ivec3(a.x * b, a.y * b, a.z * b);

    public static Ivec3 operator /(Ivec3 a, Ivec3 b)
       => new Ivec3(a.x / b.x, a.y / b.y, a.z / b.z);

    public static Ivec3 operator /(Ivec3 a, int b)
      => new Ivec3(a.x / b, a.y / b, a.z / b);

    //allow my vector to be cast as unity's vectors and vice versa
    public static implicit operator Ivec3(UnityEngine.Vector2 v)
    => new Ivec3((int)v.x,(int)v.y,0);

    public static implicit operator Ivec3(UnityEngine.Vector3 v)
    => new Ivec3((int)v.x, (int)v.y, (int)v.z);

    public static implicit operator UnityEngine.Vector2(Ivec3 v)
    => new UnityEngine.Vector2(v.x, v.y);

    public static implicit operator UnityEngine.Vector3(Ivec3 v)
    => new UnityEngine.Vector3(v.x, v.y, v.z);

    //basic functions
    public static double ScalarProduct(Ivec3 firstVector, Ivec3 secondVector)
    {
        return (firstVector.x * secondVector.x) + (firstVector.y * secondVector.y);
    }
    public static double Distance(Ivec3 firstCord, Ivec3 secondCord)
    {
        return Math.Pow(((firstCord.x - secondCord.x) * (firstCord.x - secondCord.x)) +
            ((firstCord.y - secondCord.y) * (firstCord.y - secondCord.y)), 0.5f);
    }
    public static Ivec3 CrossProduct(Ivec3 firstVector, Ivec3 secondVector)
    {
        Debug.LogError("this function is not yet implemented!");
        return new Ivec3(-firstVector.y, firstVector.x,0);
    }
    public static Ivec3 Zero
    {
        get { return new Ivec3(0, 0, 0); }
    }


    public static bool Contains(List<Ivec3> list, Ivec3 target)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if(list[i].x == target.x && 
                list[i].y == target.y && 
                list[i].z == target.z)
            {
                return true;
            }
        }
        return false;
    }
    public bool Equals(Ivec3 compare)
    {
        return x == compare.x &&
               y == compare.y &&
               z == compare.z;
    }
}
[Serializable]
public class Ivec2
{
    public int x, y;
    public Ivec2(int xValue, int yValue)
    {
        x = xValue;
        y = yValue;
    }
    public Ivec2()
    {
        x = 0;
        y = 0;
    }
    public Ivec2(UnityEngine.Vector2 vector2)
    {
        x = (int)vector2.x;
        y = (int)vector2.y;
    }
    public double magnitude
    {
        get { return getMagnitude(); }
    }
    double getMagnitude()
    {
        return Math.Abs(Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)));
    }
    public Ivec2 normalized
    {
        get { return getNormalized(); }
    }
    Ivec2 getNormalized()
    {
        double mag = getMagnitude();
        return new Ivec2((int)(x / mag), (int)(y / mag));
    }
    public string values
    {
        get { return x + ", " + y; }
    }
    public UnityEngine.Vector2 toVector2
    {
        get { return new UnityEngine.Vector2((float)x, (float)y); }
    }
    public UnityEngine.Vector3 toVector3
    {
        get { return new UnityEngine.Vector3((float)x, (float)y, 0); }
    }

    //deffine how my vector will handle operations
    public static Ivec2 operator +(Ivec2 a) => a;
    public static Ivec2 operator -(Ivec2 a) => a;// new Ivec3(-a.x, -a.y, -a.z);

    public static Ivec2 operator +(Ivec2 a, Ivec2 b)
        => new Ivec2(a.x + b.x, a.y + b.y);
    public static Ivec2 operator +(Ivec2 a, int b)
        => new Ivec2(a.x + b, a.y + b);
    public static Ivec2 operator +(int b, Ivec2 a)
        => new Ivec2(a.x + b, a.y + b);
    public static Ivec2 operator -(Ivec2 a, Ivec2 b)
        => new Ivec2(a.x - b.x, a.y - b.y);
    public static Ivec2 operator -(Ivec2 a, int b)
        => new Ivec2(a.x - b, a.y - b);
    public static Ivec2 operator *(Ivec2 a, Ivec2 b)
        => new Ivec2(a.x * b.x, a.y * b.y);
    public static Ivec2 operator *(Ivec2 a, int b)
      => new Ivec2(a.x * b, a.y * b);
    public static Ivec2 operator *(int b, Ivec2 a)
      => new Ivec2(a.x * b, a.y * b);

    public static Ivec2 operator /(Ivec2 a, Ivec2 b)
       => new Ivec2(a.x / b.x, a.y / b.y);

    public static Ivec2 operator /(Ivec2 a, int b)
      => new Ivec2(a.x / b, a.y / b);

    //allow my vector to be cast as unity's vectors and vice versa
    public static implicit operator Ivec2(UnityEngine.Vector2 v)
    => new Ivec2((int)v.x, (int)v.y);

    public static implicit operator Ivec2(UnityEngine.Vector3 v)
    => new Ivec2((int)v.x, (int)v.y);

    public static implicit operator UnityEngine.Vector2(Ivec2 v)
    => new UnityEngine.Vector2(v.x, v.y);

    public static implicit operator UnityEngine.Vector3(Ivec2 v)
    => new UnityEngine.Vector3(v.x, v.y, 0);

    //basic functions
    public static double ScalarProduct(Ivec3 firstVector, Ivec3 secondVector)
    {
        return (firstVector.x * secondVector.x) + (firstVector.y * secondVector.y);
    }
    public static float Distance(Ivec2 firstCord, Ivec2 secondCord)
    {
        return (float)Math.Pow(((firstCord.x - secondCord.x) * (firstCord.x - secondCord.x)) +
            ((firstCord.y - secondCord.y) * (firstCord.y - secondCord.y)), 0.5f);
    }
    public static Ivec3 CrossProduct(Ivec3 firstVector, Ivec3 secondVector)
    {
        Debug.LogError("this function is not yet implemented!");
        return new Ivec3(-firstVector.y, firstVector.x, 0);
    }
    public static Ivec3 Zero
    {
        get { return new Ivec3(0, 0, 0); }
    }


    public static bool Contains(List<Ivec3> list, Ivec3 target)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].x == target.x &&
                list[i].y == target.y &&
                list[i].z == target.z)
            {
                return true;
            }
        }
        return false;
    }
    public bool Equals(Ivec2 compare)
    {
        return x == compare.x &&
               y == compare.y;
    }
}
