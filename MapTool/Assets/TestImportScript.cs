using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class TestImportScript
{
    public static string path;
    public static int mapID; //Ex 01
    static List<string> strings;
    public static void ReadString()
    {
        //path = "MapFiles/pcgmap" + mapID + ".txt";
        path = "MapFiles/pcgmap01.txt";
        strings = new List<string>();
        StreamReader sr = new StreamReader(path);
        Debug.Log(sr.ReadToEnd());
        while (!sr.EndOfStream)
        {
            strings.Add(sr.ReadLine());
        }
        sr.Close();
    }

    static void SplitString()
    {
        for (int i = 0; i < strings.Count; i++)
        {
            string[] coordinates = strings[i].Split(' ');
            //for (int j = 0; j < coordinates.Length; j++)
            //{
            //    string[] xy = coordinates[j].Split();
            //}
        }
    }
}
