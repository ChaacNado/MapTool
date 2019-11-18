using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public static class ExportScript
{
    public static string path;
    static string[] files;

    public static void OpenFolderPanel(string fileName)
    {
        path = EditorUtility.OpenFolderPanel("Export to: ", "", "");
        files = Directory.GetFiles(path);
        if (!fileName.Contains("."))
        {
            fileName += ".txt";
        }
        path += "\\" + fileName;    
        

        //foreach (string file in files)
        //{
        //    Debug.Log(file);
        //    if (file == fileName)
        //    {
        //        Debug.Log("File already exczsist");
        //    }
        //}
        Debug.Log(path);
    }

    public static void WriteBuildingToFile(int ID)
    {
        string line = "b " + ID;
        var sr = File.AppendText(path);
        sr.WriteLine(line);
        sr.Close();
    }

    public static void WriteWallsToFile(float posX, float posY, float width, float height)
    {
        string line = "w " + posX + " " + posY + " " + width + " " + height;
        var sr = File.AppendText(path);
        sr.WriteLine(line);
        sr.Close();
    }

    public static void WriteRoadToFile(int posX, int posY)
    {

    }
}
