using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class FileManager
{
    public static string SaveFileName = "save.json";
    public static string SettingsFileName = "settings.json";

    #region Game settings
    public static GameSettings GetGameSettings()
    {
        GameSettings gs = new GameSettings();

        if (NoFileController.NoFileEnabled)
        {
            GameObject goNoFile = GameObject.FindGameObjectWithTag("NoFileController");
            NoFileController nfCtrl = goNoFile.GetComponent<NoFileController>();

            gs = nfCtrl.GetGameSettings();
        }
        else
        {
            string szFilesPath = Application.persistentDataPath;
            string szPathComplet = szFilesPath + "/" + SettingsFileName;

            if (!File.Exists(szPathComplet))
            {
                InitGameSettings();
            }

            StreamReader sr = new StreamReader(szPathComplet);
            string szJson = sr.ReadToEnd();
            gs = (GameSettings)JsonConvert.DeserializeObject<GameSettings>(szJson);
            sr.Close();
            sr.Dispose();
        }

        return gs;
    }

    private static void InitGameSettings()
    {
        GameSettings gs = new GameSettings();

        gs.SoundEnabled = true;
        gs.SoundVolume = 1;

        SaveGameSettings(gs);
    }

    public static void SaveGameSettings(GameSettings gs)
    {
        if (NoFileController.NoFileEnabled)
        {
            GameObject goNoFile = GameObject.FindGameObjectWithTag("NoFileController");
            NoFileController nfCtrl = goNoFile.GetComponent<NoFileController>();

            nfCtrl.SaveGameSettings(gs);
        }
        else
        {
            string szFilesPath = Application.persistentDataPath;
            string szPathComplet = szFilesPath + "/" + SettingsFileName;

            string jsonString = JsonConvert.SerializeObject(gs);

            StreamWriter sw = new StreamWriter(szPathComplet);
            sw.Write(jsonString);
            sw.Close();
            sw.Dispose();
        }
    }
    #endregion

    #region Game save

    public static GameSave GetGameSave()
    {
        GameSave gv = new GameSave();

        if (NoFileController.NoFileEnabled)
        {
            GameObject goNoFile = GameObject.FindGameObjectWithTag("NoFileController");
            NoFileController nfCtrl = goNoFile.GetComponent<NoFileController>();

            gv = nfCtrl.GetGameSave();
        }
        else
        {

            string szFilesPath = Application.persistentDataPath;
            string szPathComplet = szFilesPath + "/" + SaveFileName;

            if (!File.Exists(szPathComplet))
            {
                InitGameSave();
            }

            StreamReader sr = new StreamReader(szPathComplet);
            string szJson = sr.ReadToEnd();
            gv = (GameSave)JsonConvert.DeserializeObject<GameSave>(szJson);

            sr.Close();
            sr.Dispose();
        }

        return gv;
    }

    public static void InitGameSave()
    {
        GameSave gv = new GameSave();

        gv.LastLevelCleared = 0;

        SaveGameSave(gv);
    }

    public static void SaveGameSave(GameSave gv)
    {
        if (NoFileController.NoFileEnabled)
        {
            GameObject goNoFile = GameObject.FindGameObjectWithTag("NoFileController");
            NoFileController nfCtrl = goNoFile.GetComponent<NoFileController>();

            nfCtrl.SaveGameSave(gv);
        }
        else
        {
            string szFilesPath = Application.persistentDataPath;
            string szPathComplet = szFilesPath + "/" + SaveFileName;

            string jsonString = JsonConvert.SerializeObject(gv);

            StreamWriter sw = new StreamWriter(szPathComplet);
            sw.Write(jsonString);
            sw.Close();
            sw.Dispose();
        }
    }

    #endregion

}

[Serializable]
public class GameSave
{
    public int LastLevelCleared { get; set; }
}

[Serializable]
public class GameSettings
{
    public bool SoundEnabled { get; set; }
    public float SoundVolume { get; set; }
}