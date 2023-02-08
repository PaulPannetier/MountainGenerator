using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public delegate float basicFunction(in float x);
public delegate string SerialyseFunction<T>(T obj);
public delegate T DeserialyseFunction<T>(string s);

public struct Rectangle
{
    public float x, y, width, height;
    public float area { get => width * height; }
    public Vector2 center { get => new Vector2(x + width / 2f, y + height / 2f); }
    public Vector2 topRight { get => new Vector2(x, y); }
    public Vector2 topLeft { get => new Vector2(x + width, y); }
    public Vector2 botttomRight { get => new Vector2(x, y + height); }
    public Vector2 bottomLeft { get => new Vector2(x + width, y + height); }
    public Vector2[] summit { get => new Vector2[4] { topRight, topLeft, bottomLeft, botttomRight }; }

    public Rectangle(in float x, in float y, in float width, in float height)
    {
        this.x = x; this.y = y; this.width = width; this.height = height;
    }

    public bool Contain(in Vector2 a) => a.x >= x && a.x <= x + width && a.y >= y && a.y <= y + height;
    public bool Contain(in Rectangle rectangle) => Contain(rectangle.topRight) && Contain(rectangle.topLeft) && Contain(rectangle.botttomRight) && Contain(rectangle.bottomLeft);

    public bool Intersect(in Rectangle rectangle)
    {
        bool pointInterior = false, pointExterior = false;
        Vector2[] summit = this.summit;
        for (int i = 0; i < 4; i++)
        {
            if (Contain(summit[i]))
            {
                if (pointExterior)
                {
                    return true;
                }
                pointInterior = true;
            }
            else
            {
                if (pointInterior)
                {
                    return true;
                }
                pointExterior = true;
            }
        }
        return false;
    }

    public bool Collide(in Rectangle rectangle) => Intersect(rectangle) || Contain(rectangle);
}

public static class Save
{
    /// <summary>
    /// Convert any Serializable object in JSON string.
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <returns> A string represent the object in parameter</returns>
    public static string ConvertObjectToJSONString(object obj) => JsonUtility.ToJson(obj);
    /// <summary>
    /// Convert any string reprensent a Serializable object to the object.
    /// </summary>
    /// <typeparam name="T">The type of the object return</typeparam>
    /// <param name="JSONString">The string represent the object return</param>
    /// <returns> A Serializable object describe by the string in parameter</returns>
    public static T ConvertJSONStringToObject<T>(string JSONString) => JsonUtility.FromJson<T>(JSONString);
    
    /// <summary>
    /// Write in the customer machine a file with the object inside
    /// </summary>
    /// <param name="objToWrite">The object to save</param>
    /// <param name="filename">the save path, begining to the game's folder</param>
    /// <returns> true if the save complete successfully, false overwise</returns>
    public static bool WriteJSONData(object objToWrite, string fileName)
    {
        try
        {
            File.WriteAllText(Application.dataPath + fileName, ConvertObjectToJSONString(objToWrite));
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
    /// <typeparam name="T">The object to read's type</typeparam>
    /// <param name="fileName">The path of the file, begining to the game's folder</param>
    /// <param name="objRead"></param>
    /// <returns> true if the function complete successfully, false overwise</returns>
    public static bool ReadJSONData<T>(string fileName, out T objRead)
    {
        try
        {
            string jsonString = File.ReadAllText(Application.dataPath + fileName);
            objRead = ConvertJSONStringToObject<T>(jsonString);
            return true;
        }
        catch (Exception)
        {
            objRead = default(T);
            return false;
        }        
    }
}

public static class Random
{
    private static System.Random random = new System.Random();
    public static void SetRandomSeed(in int seed)
    {
        random = new System.Random(seed);
        UnityEngine.Random.InitState(seed);
    }
    public static void SetRandomSeed()
    {
        int seed = (int)DateTime.Now.Ticks;
        random = new System.Random(seed);
        UnityEngine.Random.InitState(seed);
    }
    /// <returns> A random integer between a and b, [|a, b|]</returns>
    public static int Rand(in int a, in int b) => random.Next(a, b + 1);
    /// <returns> A random float between a and b, [a, b]</returns>
    public static float Rand(in float a, in float b) => UnityEngine.Random.value * (Math.Abs(b - a)) + a;
    /// <returns> A random int between a and b, [|a, b|[</returns>
    public static int RandExclude(in int a, in int b) => random.Next(a, b);
    /// <returns> A random double between a and b, [a, b[</returns>
    public static float RandExclude(in float a, in float b) => (float)random.NextDouble() * (Math.Abs(b - a)) + a;
    public static Color RandomColor() => new Color(Rand(0, 255) / 255f, Rand(0, 255) / 255f, Rand(0, 255) / 255f, 1f);
    public static float PerlinNoise(in float x, in float y) => Mathf.PerlinNoise(x, y);
    /// <returns> A random color with a random opacity</returns>
    public static Color RandomColorTransparent() => new Color(Rand(0, 255) / 255f, Rand(0, 255) / 255f, Rand(0, 255) / 255f, UnityEngine.Random.value);
    /// <returns> A random Vector2 normalised</returns>
    public static Vector2 RandomVector2()
    {
        float angle = Rand(0f, Mathf.PI * 2f);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    /// <returns> A random Vector2 with de magnitude in param</returns>
    public static Vector2 RandomVector2(in float magnitude)
    {
        float angle = Rand(0f, Mathf.PI * 2f);
        return new Vector2(magnitude * Mathf.Cos(angle), magnitude * Mathf.Sin(angle));
    }
    /// <returns> A random Vector2 with a randoml magnitude</returns>
    public static Vector2 RandomVector2(in float minMagnitude, in float maxMagnitude)
    {
        float angle = Rand(0f, Mathf.PI * 2f);
        float magnitude = Rand(minMagnitude, maxMagnitude);
        return new Vector2(magnitude * Mathf.Cos(angle), magnitude * Mathf.Sin(angle));
    }
    /// <returns> A random Vector3 normalised</returns>
    public static Vector3 RandomVector3()
    {
        float teta = Rand(0f, Mathf.PI);
        float phi = Rand(0f, Mathf.PI * 2f);
        return new Vector3(Mathf.Sin(teta) * Mathf.Cos(phi), Mathf.Sin(teta) * Mathf.Sin(phi), Mathf.Cos(teta));
    }
    /// <returns> A random Vector3 with de magnitude in param</returns>
    public static Vector3 RandomVector3(in float magnitude)
    {
        float teta = Rand(0f, Mathf.PI);
        float phi = Rand(0f, Mathf.PI * 2f);
        return new Vector3(magnitude * Mathf.Sin(teta) * Mathf.Cos(phi), magnitude * Mathf.Sin(teta) * Mathf.Sin(phi), magnitude * Mathf.Cos(teta));
    }
    /// <returns> A random Vector3 with a random magnitude</returns>
    public static Vector3 RandomVector3(in float minMagnitude, in float maxMagnitude)
    {
        float teta = Rand(0f, Mathf.PI);
        float phi = Rand(0f, Mathf.PI * 2f);
        float magnitude = Rand(minMagnitude, maxMagnitude);
        return new Vector3(magnitude * Mathf.Sin(teta) * Mathf.Cos(phi), magnitude * Mathf.Sin(teta) * Mathf.Sin(phi), magnitude * Mathf.Cos(teta));
    }
}

public static class Geometry
{
    public static bool Collide(Cube c1, Cube c2)
    {
        Vector3[] s1 = c1.summits;
        for (int i = 0; i < s1.Length; i++)
        {
            if (c2.Contain(s1[i]))
                return true;
        }
        Vector3[] s2 = c2.summits;
        for (int i = 0; i < s2.Length; i++)
        {
            if (c1.Contain(s2[i]))
                return true;
        }
        return false;
    }
    /*
    public static bool Collide(Cube c, Sphere s)
    {

    }
    public static bool Collide(Sphere s, Cube c) => Collide(c, s);
    */
    public static bool Collide(Sphere s1, Sphere s2) => s1.center.SqrDistance(s2.center) <= (s1.radius + s2.radius) * (s1.radius + s2.radius);
}

public class Cube
{
    public Vector3 center, rotation;
    public float l, h, d;

    public Vector3[] summits { get => new Vector3[8] { s1, s2, s3, s4, s5, s6, s7, s8 }; }
    private Vector3 s1 { get => center + new Vector3(-l / 2f, h / 2f, d / 2); }
    private Vector3 s2 { get => center + new Vector3(l / 2f, h / 2f, d / 2); }
    private Vector3 s3 { get => center + new Vector3(-l / 2f, h / 2f, -d / 2); }
    private Vector3 s4 { get => center + new Vector3(l / 2f, h / 2f, -d / 2); }
    private Vector3 s5 { get => center + new Vector3(-l / 2f, -h / 2f, d / 2); }
    private Vector3 s6 { get => center + new Vector3(l / 2f, -h / 2f, d / 2); }
    private Vector3 s7 { get => center + new Vector3(-l / 2f, -h / 2f, -d / 2); }
    private Vector3 s8 { get => center + new Vector3(l / 2f, -h / 2f, -d / 2); }

    public Cube(in float l, in float h, in float d, in Vector3 center, in Vector3 rotation)
    {
        this.l = l; this.d = d; this.h = h; this.center = center; this.rotation = rotation;
    }
    public Cube(in float l, in float h, in float d, in Vector3 center)
    {
        this.l = l; this.d = d; this.h = h; this.center = center; this.rotation = Vector3.zero;
    }

    /// <summary>
    /// Si rotation == Vector3.zero
    /// </summary>
    public bool Contain(Vector3 point) => point.x >= s1.x && point.x <= s2.x && point.y >= s3.y && point.y <= s1.y && point.z >= s3.z && point.z <= s1.z;
}

public class Sphere
{
    public Vector3 center;
    public float radius;

    public Sphere(in Vector3 center, in float radius)
    {
        this.center = center; this.radius = radius;
    }
}

public static class Usefull
{
    #region Color
    /// <summary>
    /// Return a Color deeper than the color in argument
    /// </summary>
    /// <param name="c">The color to change</param>
    /// <param name="percent">le coeff €[0, 1] d'assombrissement</param>
    /// <returns></returns>
    public static Color ColorDeeper(in Color c, in float coeff) => new Color(c.r * (1f - coeff), c.g * (1f - coeff), c.b * (1f - coeff), c.a);
    /// <summary>
    /// Return a Color lighter than the color in argument
    /// </summary>
    /// <param name="c">The color to change</param>
    /// <param name="percent">le coeff €[0, 1] de luminosité</param>
    /// <returns></returns>
    public static Color ColorLighter(Color c, float coeff) => new Color(((1f - c.r) * coeff) + c.r, ((1f - c.g) * coeff) + c.g, ((1f - c.b) * coeff) + c.b, c.a);

    public static Color Lerp(in Color a, in Color b, in float t)
    {
        float rToAdd = (b.r - a.r) * t;
        float gToAdd = (b.g - a.g) * t;
        float bToAdd = (b.b - a.b) * t;
        float aToAdd = (b.a - a.a) * t;
        return new Color(a.r + rToAdd, a.g + gToAdd, a.b + bToAdd, a.a + aToAdd);
    }
    #endregion

    //Vector2
    public static float SqrDistance(this Vector2 v, in Vector2 a) => (a.x - v.x) * (a.x - v.x) + (a.y - v.y) * (a.y - v.y);
    public static float SqrDistance(this Vector2 v, in float x, in float y) => (x - v.x) * (x - v.x) + (y - v.y) * (y - v.y);
    public static float Distance(this Vector2 v, in Vector2 a) => Mathf.Sqrt(v.SqrDistance(a));
    public static float Distance(this Vector2 v, in float x, in float y) => Mathf.Sqrt(v.SqrDistance(x, y));
    //Vector3
    public static float SqrDistance(this Vector3 v, in Vector3 a) => (a.x - v.x) * (a.x - v.x) + (a.y - v.y) * (a.y - v.y) + (a.z - v.z) * (a.z - v.z);
    public static float Distance(this Vector3 v, in Vector3 a) => Mathf.Sqrt(v.SqrDistance(a));

    public static float ToRad(in float angle) => (float)(((angle * Math.PI) / 180f) % (2 * Math.PI));
    public static float ToDegrees(in float angle) => (float)(((angle * 180f) / Math.PI) % 360);

    /// <returns >angle entre a et b compris entre 0 et 2pi radian</returns>
    public static float Angle(in Vector2 a, in Vector2 b) => (float)(Math.Atan2(a.y - b.y, a.x - b.x) + Math.PI);
    public static float Angle(this Vector2 a, in Vector2 b) => (float)(Math.Atan2(a.y - b.y, a.x - b.x) + Math.PI);

    /// <summary>
    /// Renvoie l'angle minimal entre le segment [ca] et [cb]
    /// </summary>
    public static float Angle(in Vector2 c, in Vector2 a, in Vector2 b)
    {
        float ang1 = Angle(c, a);
        float ang2 = Angle(c, b);
        float diff = Math.Abs(ang1 - ang2);
        return Math.Min(diff, 2f * Mathf.PI - diff);
    }
    /// <summary>
    /// Renvoie si pour aller de l'angle 1 vers l'angle 2 le plus rapidement il faut tourner à droite ou à gauche, ang€[0, 2pi[
    /// </summary>
    public static void DirectionAngle(in float ang1, in float ang2, out bool right)
    {
        float diff = Math.Abs(ang1 - ang2);
        float angMin = Math.Min(diff, 2f * Mathf.PI - diff);
        right = Math.Abs((ang1 + angMin) % (2f * Mathf.PI) - ang2) < 0.1f;
    }
    /// <summary>
    /// Renvoie l'angle en radian égal à l'angle en param mais dans [0, 2pi[
    /// </summary>
    public static float SimplifieAngle(in float angle) => Lerp(0f, 2f * Mathf.PI, angle);
    /// <returns> value if value is in the range [a, b], a or b otherwise</returns>
    public static float MarkOut(in float min, in float max, in float value) => Mathf.Max(Mathf.Min(value, max), min);
    /// <returns> a like a = value % (end -  start) + start, a€[start, end[ /returns>
    public static float Lerp(in float start, in float end, in float value)
    {
        if (end < start)
            return Lerp(end, start, value);
        if (end == start)
            return start;

        if (value < end && value >= start)
            return value;
        else
        {
            float modulo = end - start;
            float result = (value % modulo) + start;
            if(result >= end)            
                return result - modulo;
            if (result < start)
                return result + modulo;
            return result;

        }
    }
    /// <summary>
    /// t € [0, 1]
    /// </summary>
    public static int Lerp(in int a, in int b, float t) => (int)(a + (b - a) * t);

    public static bool IsOdd(in int number) => number % 2 == 1;
    public static bool IsEven(int number) => number % 2 == 0;

    #region CopieArray
    public static T[,] CopieArray<T>(in T[,] Array)
    {
        T[,] a = new T[Array.GetLength(0), Array.GetLength(1)];
        for (int l = 0; l < a.GetLength(0); l++)
        {
            for (int c = 0; c < a.GetLength(1); c++)
            {
                a[l, c] = Array[l, c];
            }
        }
        return a;
    }
    #endregion
    #region GetSubArray
    /// <summary>
    /// Retourne le sous tableau de Array, cad Array[IndexStart]
    /// </summary>
    /// <param name="indexStart">l'index de la première dimension de Array</param>
    public static T[,,] GetSubArray<T>(in T[,,,] Array, in int indexStart = 0)
    {
        T[,,] a = new T[Array.GetLength(1), Array.GetLength(2), Array.GetLength(3)];
        for (int l = 0; l < a.GetLength(0); l++)
        {
            for (int c = 0; c < a.GetLength(1); c++)
            {
                for (int i = 0; i < a.GetLength(2); i++)
                {
                    a[l, c, i] = Array[indexStart, l, c, i];
                }
            }
        }
        return a;
    }

    /// <summary>
    /// Cut the huge array in parameter to a list of little array with the size in parameters
    /// </summary>
    /// <param name="array">The array to cut</param>
    /// <param name="widthNewArray">Width of the little new array</param>
    /// <param name="heightNewArray">Width of the little new array</param>
    public static List<T[,]> CutArray<T>(T[,] array, in int widthNewArray, in int heightNewArray)
    {
        //int colums = array.GetLength(0) / widthNewArray;
        //int lines = array.GetLength(1) / heightNewArray;

        List<T[,]> result = new List<T[,]>();
        
        for (int y = 0; y < array.GetLength(1); y += heightNewArray)
        {
            for (int x = 0; x < array.GetLength(0); x += widthNewArray)
            {
                T[,] arr = new T[widthNewArray, heightNewArray];
                for (int l = 0; l < heightNewArray; l++)
                {
                    for (int c = 0; c < heightNewArray; c++)
                    {
                        arr[c, l] = array[x + c, y + l];
                    }
                }
                result.Add(arr);
            }
        }
        return result;
    }

    /// <summary>
    /// Cut the huge array in parameter to a list of little array with the size in parameters
    /// </summary>
    /// <param name="array">The array to cut</param>
    /// <param name="widthNewArray">Width of the little new array</param>
    /// <param name="heightNewArray">Width of the little new array</param>
    public static List<T[,]> CutArrayWithMargin<T>(T[,] array, in int widthNewArray, in int heightNewArray)
    {
        //int colums = array.GetLength(0) / widthNewArray;
        //int lines = array.GetLength(1) / heightNewArray;

        List<T[,]> result = new List<T[,]>();

        for (int y = 0; y < array.GetLength(1) - 1; y += heightNewArray)
        {
            for (int x = 0; x < array.GetLength(0) - 1; x += widthNewArray)
            {
                T[,] arr = new T[widthNewArray, heightNewArray];
                for (int l = 0; l < heightNewArray; l++)
                {
                    for (int c = 0; c < heightNewArray; c++)
                    {
                        arr[c, l] = array[x + c, y + l];
                    }
                }
                result.Add(arr);
                x--;
            }
            y--;
        }
        return result;
    }
    #endregion

    public static bool CaseExistArray<T>(in T[,] tab, int l, int c) => l >= 0 && c >= 0 && l < tab.GetLength(0) && c < tab.GetLength(1);

    #region Affiche Array    
    public static void ShowArray<T>(in T[] tab)
    {
        string text = "[ ";
        for (int l = 0; l < tab.GetLength(0); l++)
        {
            text += tab[l].ToString() + ", ";
        }
        text = text.Remove(text.Length - 2, 2);
        text += " ]";
        Debug.Log(text);
    }
    public static void ShowArray<T>(in T[,] tab)
    {
        string text = "";
        for (int l = 0; l < tab.GetLength(0); l++)
        {
            text = "[ ";
            for (int c = 0; c < tab.GetLength(1); c++)
            {
                text += tab[l, c].ToString() + ", ";
            }
            text = text.Remove(text.Length - 2, 2);
            text += " ],";
            Debug.Log(text);
        }
        Debug.Log("----------------");
    }
    public static void ShowArray<T>(in T[,,] tab)
    {
        string text = "";
        for (int l = 0; l < tab.GetLength(0); l++)
        {
            text += "[ ";
            for (int c = 0; c < tab.GetLength(1); c++)
            {
                text += "[ ";
                for (int i = 0; i < tab.GetLength(2); i++)
                {
                    text += tab[l, c, i].ToString() + ", ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 2, 2);
            text += " ], ";
        }
        text = text.Remove(text.Length - 3, 3);
        text += "]";
        Debug.Log(text);
    }
    public static void ShowArray<T>(in T[,,,] tab)
    {
        string text = "";
        for (int l = 0; l < tab.GetLength(0); l++)
        {
            text += "[ ";
            for (int c = 0; c < tab.GetLength(1); c++)
            {
                text += "[ ";
                for (int i = 0; i < tab.GetLength(2); i++)
                {
                    text += "[ ";
                    for (int j = 0; j < tab.GetLength(3); j++)
                    {
                        text += tab[l, c, i, j].ToString() + ", ";
                    }
                    text = text.Remove(text.Length - 2, 2);
                    text += " ], ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 2, 2);
            text += " ], ";
        }
        text = text.Remove(text.Length - 3, 3);
        text += "]";
        Debug.Log(text);
    }
    public static void ShowArray<T>(in T[,,,,] tab)
    {
        string text = "";
        for (int l = 0; l < tab.GetLength(0); l++)
        {
            text += "[ ";
            for (int c = 0; c < tab.GetLength(1); c++)
            {
                text += "[ ";
                for (int i = 0; i < tab.GetLength(2); i++)
                {
                    text += "[ ";
                    for (int j = 0; j < tab.GetLength(3); j++)
                    {
                        text += "[ ";
                        for (int k = 0; k < tab.GetLength(4); k++)
                        {
                            text += tab[l, c, i, j, k].ToString() + ", ";
                        }
                        text = text.Remove(text.Length - 2, 2);
                        text += " ], ";
                    }
                    text = text.Remove(text.Length - 2, 2);
                    text += " ], ";
                }
                text = text.Remove(text.Length - 2, 2);
                text += " ], ";
            }
            text = text.Remove(text.Length - 2, 2);
            text += " ], ";
        }
        text = text.Remove(text.Length - 3, 3);
        text += "]";
        Debug.Log(text);
    }
    #endregion
    #region Normalise Array
    /// <summary>
    /// Normalise tout les éléments de l'array pour obtenir des valeur entre 0f et 1f, ainse le min de array sera 0f, et le max 1f.
    /// </summary>
    /// <param name="array">The array to normalised</param>
    public static void NormaliseArray(float[] array)
    {
        float min = float.MaxValue, max = float.MinValue;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < min)
                min = array[i];
            if (array[i] > max)
                max = array[i];
        }
        float maxMinusMin = max - min;
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (array[i] - min) / maxMinusMin;
        }
    }
    /// <summary>
    /// Change array like the sum of each element make 1f
    /// </summary>
    /// <param name="array">the array to change, each element must to be positive</param>
    public static void GetProbabilityArray(float[] array)
    {
        float sum = 0f;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < 0f)
            {
                Debug.LogWarning("Array[" + i + "] must to be positive : " + array[i]);
                return;
            }
            sum += array[i];
        }
        for (int i = 0; i < array.Length; i++)
        {
            array[i] /= sum;
        }
    }
    #endregion
    #region Sort
    public static void Sort<T>(List<T> lst)
    {
        lst.Sort();
    }
    //algo de tri : https://www.jesuisundev.com/comprendre-les-algorithmes-de-tri-en-7-minutes/
    #endregion

    private static string[] letter = new string[36] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

    private static string Troncate(string mot)
    {
        string newMot = mot;
        for (int i = 0; i < mot.Length; i++)
        {
            string s = mot.Substring(i, 1);
            if (s == "," || s == ".")
            {
                newMot = newMot.Remove(i, mot.Length - i);
                break;
            }
        }
        return newMot;
    }

    public static int ConvertStringToInt(string number)
    {
        int nb = 0;
        number = Troncate(number);
        for (int i = number.Length - 1; i >= 0; i--)
        {
            string carac = number.Substring(i, 1);
            for (int j = 26; j <= 35; j++)
            {
                if (carac == letter[j])
                {
                    int n = j - 26;
                    nb += n * (int)Math.Pow(10, number.Length - 1 - i);
                    break;
                }
            }
        }
        if (number.Substring(0, 1) == "-")
            nb *= -1;

        return nb;
    }

    public static float ConvertStringToFloat(string number)
    {
        float result = 0;
        string partieEntiere = number;
        string partieDecimal = "";

        int indexComa = 0;
        for (int i = 0; i < number.Length; i++)
        {
            string s = number.Substring(i, 1);
            if (s == "," || s == ".")
            {
                partieDecimal = partieEntiere.Substring(i + 1, partieEntiere.Length - (i + 1));
                partieEntiere = partieEntiere.Remove(i, partieEntiere.Length - i);
                indexComa = i;
                break;
            }
        }
        //part entière
        result = ConvertStringToInt(partieEntiere);
        //part decimal
        for (int i = 0; i < partieDecimal.Length; i++)
        {
            string carac = partieDecimal.Substring(i, 1);
            for (int j = 26; j <= 35; j++)
            {
                if (carac == letter[j])
                {
                    int n = j - 26; //n € {0,1,2,3,4,5,6,7,8,9}
                    result += n * (float)Math.Pow(10, -(i + 1));
                    break;
                }
            }
        }
        return result;
    }
    #region SumList
    /// <summary>
    /// Retourne lst1 union lst2
    /// </summary>
    /// <param name="lst1">La première liste</param>
    /// <param name="lst2">La seconde liste</param>
    /// <param name="doublon">SI on autorise ou pas les doublons</param>
    /// <returns></returns>        
    public static List<T> SumList<T>(in List<T> lst1, in List<T> lst2, bool doublon = false)//pas de doublon par defaut
    {
        List<T> result = new List<T>();
        foreach (T nb in lst1)
        {
            if (doublon || !result.Contains(nb))
                result.Add(nb);
        }
        foreach (T nb in lst2)
        {
            if (doublon || !result.Contains(nb))
                result.Add(nb);
        }
        return result;
    }
    public static void Add<T>(this List<T> lst1, in List<T> lstToAdd, bool doublon = false)//pas de doublon par defaut
    {
        if(doublon)
        {
            foreach (T element in lstToAdd)
            {
                lst1.Add(element);
            }
        }
        else
        {
            foreach (T element in lstToAdd)
            {
                if (lst1.Contains(element))
                {
                    continue;
                }
                lst1.Add(element);
            }
        }
        
    }
    #endregion
    #region ConvertStingToArray
    /// <typeparam name="T"></typeparam>
    /// <param name="array">The string to convert</param>
    /// <param name="convertFunction">The conversion function of string to T</param>
    /// <param name="dimension">The dimension of the array create</param>
    /// <returns>An object castable of an array of T with the dimension</returns>
    public static object ConvertStingToArray<T>(string array, DeserialyseFunction<T> convertFunction, out int dimension, in T nullElement = default(T))
    {
        array = array.Replace(" ", "");//on enlève tout les espaces
        int dim = 0;//on calcule la dim
        int compteurDim = 0;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].ToString() == "[")
                compteurDim++;
            if (array[i].ToString() == "]")
                compteurDim--;
            dim = Math.Max(dim, compteurDim);
        }
        dimension = dim;
        switch (dim)
        {
            case 1:
                return ConvertStringToArray1(array, convertFunction);
            case 2:
                return ConvertStringToArray2(array, convertFunction, nullElement);
            case 3:
                return ConvertStringToArray3(array, convertFunction, nullElement);
            case 4:
                return ConvertStringToArray4(array, convertFunction, nullElement);
            case 5:
                return ConvertStringToArray5(array, convertFunction, nullElement);
            default:
                throw new Exception("To many dim in " + array + " max dimension is 5");
        }
    }

    private static object ConvertStringToArray1<T>(string tab, DeserialyseFunction<T> f)
    {
        List<string> value = new List<string>();
        string val = "";
        for (int i = 0; i < tab.Length; i++)
        {
            if (tab[i].ToString() == "," || tab[i].ToString() == "]" || tab[i].ToString() == "[")
            {
                if (val != "")
                    value.Add(val);
                val = "";
            }
            else
            {
                val += tab[i].ToString();
            }
        }

        T[] result = new T[value.Count];
        for (int i = 0; i < value.Count; i++)
        {
            result[i] = f(value[i]);
        }
        return result;
    }
    private static object ConvertStringToArray2<T>(string tab, DeserialyseFunction<T> f, in T nullElement)
    {
        //"[[1,2,3,4],[4,5,6]]" va retourné new int[2, 4] { {1, 2, 3, 4}, {4, 5, 6, nullElement} };
        int nbline = -1, nbCol = 0;
        int compteurDim = -1;
        int compteurCol = 0;
        for (int i = 0; i < tab.Length; i++)
        {
            if (tab[i].ToString() == "[")
                compteurDim++;
            if (tab[i].ToString() == "]")
            {
                compteurDim--;
                nbline++;
            }
            if (compteurDim == 1)
            {
                if (tab[i].ToString() == ",")
                {
                    compteurCol++;
                    nbCol = Math.Max(nbCol, compteurCol + 1);
                }
            }
            else
            {
                compteurCol = 0;
            }
        }
        T[,] result = new T[nbline, nbCol];//on crée et initialise le tab;
        for (int l = 0; l < result.GetLength(0); l++)
        {
            for (int c = 0; c < result.GetLength(1); c++)
            {
                result[l, c] = nullElement;
            }
        }
        //on remplit le resulat
        compteurDim = -1;
        compteurCol = 0;
        int compteurLine = -2;
        string value = "";
        string text;

        for (int i = 0; i < tab.Length; i++)
        {
            text = tab[i].ToString();
            if (text != "[" && text != "]" && text != ",")
            {
                value += text;
            }
            if (text == "[")
            {
                compteurDim++;
                compteurLine++;
                compteurCol = 0;
            }
            else
            {
                if (text == "]")
                {
                    compteurDim--;
                    if (value != "")
                    {
                        result[compteurLine, compteurCol] = f(value);
                        value = "";
                    }
                }
                else
                {
                    if (text == ",")
                    {
                        if (value != "")
                        {
                            result[compteurLine, compteurCol] = f(value);
                            value = "";
                            compteurCol++;
                        }
                    }
                }
            }
        }

        return result;
    }
    private static object ConvertStringToArray3<T>(string tab, DeserialyseFunction<T> f, in T nullElement)
    {
        int nbDim0 = 1, nbDim1 = 1, nbDim2 = 1;
        int compteurDim0 = 0, compteurDim1 = 0, compteurDim2 = 0;
        int compteurDim = -1;
        for (int i = 0; i < tab.Length; i++)
        {
            if (tab[i].ToString() == "[")
                compteurDim++;
            if (tab[i].ToString() == "]")
                compteurDim--;
            switch (compteurDim)
            {
                case 0:
                    compteurDim1 = compteurDim2 = 0;
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim0++;
                        nbDim0 = Math.Max(nbDim0, compteurDim0 + 1);
                    }
                    break;
                case 1:
                    compteurDim2 = 0;
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim1++;
                        nbDim1 = Math.Max(nbDim1, compteurDim1 + 1);
                    }
                    break;
                case 2:
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim2++;
                        nbDim2 = Math.Max(nbDim2, compteurDim2 + 1);
                    }
                    break;
                default:
                    break;
            }
        }

        T[,,] result = new T[nbDim0, nbDim1, nbDim2];//on crée et initialise le tab;
        for (int a = 0; a < result.GetLength(0); a++)
        {
            for (int b = 0; b < result.GetLength(1); b++)
            {
                for (int c = 0; c < result.GetLength(2); c++)
                {
                    result[a, b, c] = nullElement;
                }
            }
        }
        compteurDim0 = compteurDim1 = compteurDim2 = 0;
        compteurDim = -1;
        string value = "";
        string text;

        for (int i = 0; i < tab.Length; i++)
        {
            text = tab[i].ToString();
            if (text != "[" && text != "]" && text != ",")
                value += text;
            if (text == "[")
            {
                compteurDim++;
                //compteurDim0++;
            }
            if (text == "]")
            {
                compteurDim--;
                if (value != "")
                {
                    result[compteurDim0, compteurDim1, compteurDim2] = f(value);
                    value = "";
                }
            }
            switch (compteurDim)
            {
                case 0:
                    compteurDim1 = compteurDim2 = 0;
                    if (text == ",")
                    {
                        compteurDim0++;
                    }
                    break;
                case 1:
                    compteurDim2 = 0;
                    if (text == ",")
                    {
                        compteurDim1++;
                    }
                    break;
                case 2:
                    if (text == ",")
                    {
                        result[compteurDim0, compteurDim1, compteurDim2] = f(value);
                        value = "";
                        compteurDim2++;
                    }
                    break;
                default:
                    break;
            }
        }
        return result;
    }
    private static object ConvertStringToArray4<T>(string tab, DeserialyseFunction<T> f, in T nullElement)
    {
        int nbDim0 = 1, nbDim1 = 1, nbDim2 = 1, nbDim3 = 1;
        int compteurDim0 = 0, compteurDim1 = 0, compteurDim2 = 0, compteurDim3 = 0;
        int compteurDim = -1;
        for (int i = 0; i < tab.Length; i++)
        {
            if (tab[i].ToString() == "[")
                compteurDim++;
            if (tab[i].ToString() == "]")
                compteurDim--;
            switch (compteurDim)
            {
                case 0:
                    compteurDim1 = compteurDim2 = compteurDim3 = 0;
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim0++;
                        nbDim0 = Math.Max(nbDim0, compteurDim0 + 1);
                    }
                    break;
                case 1:
                    compteurDim2 = compteurDim3 = 0;
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim1++;
                        nbDim1 = Math.Max(nbDim1, compteurDim1 + 1);
                    }
                    break;
                case 2:
                    compteurDim3 = 0;
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim2++;
                        nbDim2 = Math.Max(nbDim2, compteurDim2 + 1);
                    }
                    break;
                case 3:
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim3++;
                        nbDim3 = Math.Max(nbDim3, compteurDim3 + 1);
                    }
                    break;
                default:
                    break;
            }
        }

        T[,,,] result = new T[nbDim0, nbDim1, nbDim2, nbDim3];//on crée et initialise le tab;
        for (int a = 0; a < result.GetLength(0); a++)
        {
            for (int b = 0; b < result.GetLength(1); b++)
            {
                for (int c = 0; c < result.GetLength(2); c++)
                {
                    for (int d = 0; d < result.GetLength(3); d++)
                    {
                        result[a, b, c, d] = nullElement;
                    }
                }
            }
        }
        compteurDim0 = compteurDim1 = compteurDim2 = compteurDim3 = 0;
        compteurDim = -1;
        string value = "";
        string text;

        for (int i = 0; i < tab.Length; i++)
        {
            text = tab[i].ToString();
            if (text != "[" && text != "]" && text != ",")
                value += text;
            if (text == "[")
            {
                compteurDim++;
                //compteurDim0++;
            }
            if (text == "]")
            {
                compteurDim--;
                if (value != "")
                {
                    result[compteurDim0, compteurDim1, compteurDim2, compteurDim3] = f(value);
                    value = "";
                }
            }
            switch (compteurDim)
            {
                case 0:
                    compteurDim1 = compteurDim2 = compteurDim3 = 0;
                    if (text == ",")
                    {
                        compteurDim0++;
                    }
                    break;
                case 1:
                    compteurDim2 = compteurDim3 = 0;
                    if (text == ",")
                    {
                        compteurDim1++;
                    }
                    break;
                case 2:
                    compteurDim3 = 0;
                    if (text == ",")
                    {
                        compteurDim2++;
                    }
                    break;
                case 3:
                    if (text == ",")
                    {
                        result[compteurDim0, compteurDim1, compteurDim2, compteurDim3] = f(value);
                        value = "";
                        compteurDim3++;
                    }
                    break;
                default:
                    break;
            }
        }
        return result;
    }
    private static object ConvertStringToArray5<T>(string tab, DeserialyseFunction<T> f, in T nullElement)
    {
        int nbDim0 = 1, nbDim1 = 1, nbDim2 = 1, nbDim3 = 1, nbDim4 = 1;
        int compteurDim0 = 0, compteurDim1 = 0, compteurDim2 = 0, compteurDim3 = 0, compteurDim4 = 0;
        int compteurDim = -1;
        for (int i = 0; i < tab.Length; i++)
        {
            if (tab[i].ToString() == "[")
                compteurDim++;
            if (tab[i].ToString() == "]")
                compteurDim--;
            switch (compteurDim)
            {
                case 0:
                    compteurDim1 = compteurDim2 = compteurDim3 = compteurDim4 = 0;
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim0++;
                        nbDim0 = Math.Max(nbDim0, compteurDim0 + 1);
                    }
                    break;
                case 1:
                    compteurDim2 = compteurDim3 = compteurDim4 = 0;
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim1++;
                        nbDim1 = Math.Max(nbDim1, compteurDim1 + 1);
                    }
                    break;
                case 2:
                    compteurDim3 = compteurDim4 = 0;
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim2++;
                        nbDim2 = Math.Max(nbDim2, compteurDim2 + 1);
                    }
                    break;
                case 3:
                    compteurDim4 = 0;
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim3++;
                        nbDim3 = Math.Max(nbDim3, compteurDim3 + 1);
                    }
                    break;
                case 4:
                    if (tab[i].ToString() == ",")
                    {
                        compteurDim4++;
                        nbDim4 = Math.Max(nbDim4, compteurDim4 + 1);
                    }
                    break;
                default:
                    break;
            }
        }

        T[,,,,] result = new T[nbDim0, nbDim1, nbDim2, nbDim3, nbDim4];//on crée et initialise le tab;
        for (int a = 0; a < result.GetLength(0); a++)
        {
            for (int b = 0; b < result.GetLength(1); b++)
            {
                for (int c = 0; c < result.GetLength(2); c++)
                {
                    for (int d = 0; d < result.GetLength(3); d++)
                    {
                        for (int e = 0; e < result.GetLength(4); e++)
                        {
                            result[a, b, c, d, e] = nullElement;
                        }
                    }
                }
            }
        }
        compteurDim0 = compteurDim1 = compteurDim2 = compteurDim3 = compteurDim4 = 0;
        compteurDim = -1;
        string value = "";
        string text;

        for (int i = 0; i < tab.Length; i++)
        {
            text = tab[i].ToString();
            if (text != "[" && text != "]" && text != ",")
                value += text;
            if (text == "[")
            {
                compteurDim++;
                //compteurDim0++;
            }
            if (text == "]")
            {
                compteurDim--;
                if (value != "")
                {
                    result[compteurDim0, compteurDim1, compteurDim2, compteurDim3, compteurDim4] = f(value);
                    value = "";
                }
            }
            switch (compteurDim)
            {
                case 0:
                    compteurDim1 = compteurDim2 = compteurDim3 = compteurDim4 = 0;
                    if (text == ",")
                    {
                        compteurDim0++;
                    }
                    break;
                case 1:
                    compteurDim2 = compteurDim3 = compteurDim4 = 0;
                    if (text == ",")
                    {
                        compteurDim1++;
                    }
                    break;
                case 2:
                    compteurDim3 = compteurDim4 = 0;
                    if (text == ",")
                    {
                        compteurDim2++;
                    }
                    break;
                case 3:
                    compteurDim4 = 0;
                    if (text == ",")
                    {
                        compteurDim3++;
                    }
                    break;
                case 4:
                    if (text == ",")
                    {
                        result[compteurDim0, compteurDim1, compteurDim2, compteurDim3, compteurDim4] = f(value);
                        value = "";
                        compteurDim4++;
                    }
                    break;
                default:
                    break;
            }
        }
        return result;
    }
    #endregion
    #region ConvertArrayToString
    public static string ConvertArrayToString<T>(in object array, in int dimension)
    {
        return ConvertArrayToString<T>(array, dimension, ToString);
    }
    private static string ToString<T>(T obj) => obj.ToString();

    public static string ConvertArrayToString<T>(in object array, in int dimension, SerialyseFunction<T> convertFunction)
    {
        switch(dimension)
        {
            case 1:
                return ConvertArrayToString1((T[])array, convertFunction);
            case 2:
                return ConvertArrayToString2((T[,])array, convertFunction);
            case 3:
                return ConvertArrayToString3((T[,,])array, convertFunction);
            case 4:
                return ConvertArrayToString4((T[,,,])array, convertFunction);
            case 5:
                return ConvertArrayToString5((T[,,,,])array, convertFunction);
            default:
                throw new Exception("Too many dimension in ConvertArrayToString, maximun 5");                
        }
    }

    private static string ConvertArrayToString1<T>(in T[] array, SerialyseFunction<T> convertFunction)
    {
        string result = "[";
        for (int i = 0; i < array.Length; i++)
        {
            result += convertFunction(array[i]) + ",";
        }
        result = result.Remove(result.Length - 1, 1) + "]";
        return result;
    }
    private static string ConvertArrayToString2<T>(in T[,] array, SerialyseFunction<T> convertFunction)
    {
        string result = "[";
        for (int l = 0; l < array.GetLength(0); l++)
        {
            result += "[";
            for (int c = 0; c < array.GetLength(1); c++)
            {
                result += convertFunction(array[l, c]) + ",";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
        }
        result = result.Remove(result.Length - 1, 1) + "]";
        return result;
    }
    private static string ConvertArrayToString3<T>(in T[,,] array, SerialyseFunction<T> convertFunction)
    {
        string result = "";
        for (int l = 0; l < array.GetLength(0); l++)
        {
            result += "[";
            for (int c = 0; c < array.GetLength(1); c++)
            {
                result += "[";
                for (int i = 0; i < array.GetLength(2); i++)
                {
                    result += convertFunction(array[l, c, i]) + ",";
                }
                result = result.Remove(result.Length - 1, 1) + "]";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
        }
        result = result.Remove(result.Length - 1, 1) + "]";
        return result;
    }
    private static string ConvertArrayToString4<T>(in T[,,,] array, SerialyseFunction<T> convertFunction)
    {
        string result = "";
        for (int l = 0; l < array.GetLength(0); l++)
        {
            result += "[";
            for (int c = 0; c < array.GetLength(1); c++)
            {
                result += "[";
                for (int i = 0; i < array.GetLength(2); i++)
                {
                    result += "[";
                    for (int j = 0; j < array.GetLength(3); j++)
                    {
                        result += convertFunction(array[l, c, i, j]) + ",";
                    }
                    result = result.Remove(result.Length - 1, 1) + "]";
                }
                result = result.Remove(result.Length - 1, 1) + "]";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
        }
        result = result.Remove(result.Length - 1, 1) + "]";
        return result;
    }
    private static string ConvertArrayToString5<T>(in T[,,,,] array, SerialyseFunction<T> convertFunction)
    {
        string result = "";
        for (int l = 0; l < array.GetLength(0); l++)
        {
            result += "[";
            for (int c = 0; c < array.GetLength(1); c++)
            {
                result += "[";
                for (int i = 0; i < array.GetLength(2); i++)
                {
                    result += "[";
                    for (int j = 0; j < array.GetLength(3); j++)
                    {
                        result += "[";
                        for (int k = 0; k < array.GetLength(4); k++)
                        {
                            result += convertFunction(array[l, c, i, j, k]) + ",";
                        }
                        result = result.Remove(result.Length - 1, 1) + "]";
                    }
                    result = result.Remove(result.Length - 1, 1) + "]";
                }
                result = result.Remove(result.Length - 1, 1) + "]";
            }
            result = result.Remove(result.Length - 1, 1) + "]";
        }
        result = result.Remove(result.Length - 1, 1) + "]";
        return result;
    }
    #endregion

    public static float NearestFromZero(in float a, in float b) => Math.Abs(a) < Math.Abs(b) ? a : b;
    public static float FarestFromZero(in float a, in float b) => Math.Abs(a) > Math.Abs(b) ? a : b;

    #region Integrate
    /// <summary>
    /// Renvoie l'integrale de a à b de function(x)dx
    /// </summary>
    /// <param name="function">La function à intégré</param>
    /// <param name="a">le début de l'intégrale</param>
    /// <param name="b">la fin de l'intégrale</param>
    /// <param name="précision">le nombre de point évalué par unité</param>
    /// <returns></returns>
    public static float Integrate(basicFunction function, in float a, in float b, out float min, out float max, in int précision = 50)
    {
        if (a == b || précision <= 0)
        {
            min = max = 0f;
            return 0f;
        }
        if (a > b)
            return -Integrate(function, b, a, out min, out max, précision);

        int nbPoint = (int)((b - a) * précision) + 1;
        if (nbPoint == 1)
            return Integrate(function, b, a, out min, out max, précision * 2);

        float pas = (b - a) / (nbPoint - 1);
        max = min = 0f;
        float end = b - pas;
        for (float x = a; x <= end; x += pas)
        {
            float y1 = function(x);
            float y2 = function(x + pas);
            if ((y1 < 0f && y2 > 0f) || (y1 > 0f && y2 < 0f))//pas du meme signe
            {
                continue;
            }
            float areaMin, areaMax;
            if (y1 > 0f || y2 > 0f)//morceau d'integrale positive
            {
                areaMin = pas * NearestFromZero(y1, y2);
                areaMax = pas * FarestFromZero(y1, y2);
            }
            else// morceau d'integrale négative
            {
                areaMin = -1f * pas * FarestFromZero(y1, y2);
                areaMax = -1f * pas * NearestFromZero(y1, y2);
            }
            min += areaMin;
            max += areaMax;
        }
        return (max + min) / 2f;
    }
    #endregion
}