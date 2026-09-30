using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SaveLoad
{
    public static PlayerInfo LoadedPlayerInfo;
    private static readonly int saveFileVersion = 2;

    public static string NewPlayerInfoName => UtilityMethods.GetCurrentTimestamp();
    public const string SaveInfoPrefix = "info_";
    public const string PlayerInfoPrefix = "save_";
    public const string Extension = ".json";

    public static string DevSavesPath => Application.persistentDataPath + "/DevSaves/";
    public static string GameSavesPath => Application.persistentDataPath + "/Saves/";

    public static string SavesPath
#if UNITY_EDITOR
        => DevSavesPath;
#else
        => GameSavesPath;
#endif

    public static string GetSaveInfoPath(string playerInfoId) => SavesPath + SaveInfoPrefix + playerInfoId + Extension;
    public static string GetPlayerInfoPath(string playerInfoId) => SavesPath + PlayerInfoPrefix + playerInfoId + Extension;

    public static List<SaveInfo> GetAllSaveInfo()
    {
        CheckSavesPath();

        List<SaveInfo> loadedSaveInfos = new List<SaveInfo>();

        string[] infoPaths = Directory.EnumerateFiles(SavesPath)
            .Where(x =>
                Path.GetFileNameWithoutExtension(x).StartsWith(SaveInfoPrefix) &&
                x.EndsWith(Extension))
            .OrderByDescending(x => File.GetLastWriteTime(x))
            .ToArray();


        for (int i = 0; i < infoPaths.Length; i++)
        {
            try
            {
                string jsonContent = File.ReadAllText(infoPaths[i]);
                SaveInfo loadedInfo = JsonUtility.FromJson<SaveInfo>(jsonContent);
                CheckFileVersion(loadedInfo);
                loadedSaveInfos.Add(loadedInfo);
            }
            catch (Exception e)
            {
                if (e is FileNotFoundException == false)
                    Debug.LogException(e);

            }
        }

        return loadedSaveInfos;
    }

    public static void Save(PlayerInfo playerInfo)
    {
        CheckSavesPath();

        string gameVersion = "v" + Application.version;

        //playerInfo
        playerInfo.gameVersion = gameVersion;
        playerInfo.saveFileVersion = saveFileVersion;

        string jsonContent = JsonUtility.ToJson(playerInfo, true);
        string savePath = GetPlayerInfoPath(playerInfo.saveId);

        File.WriteAllText(savePath, jsonContent);

        DateTime now = DateTime.Now;
        //saveInfo
        SaveInfo saveInfo = new SaveInfo(playerInfo.saveId);
        saveInfo.userName = playerInfo.userName;
        saveInfo.unitName = playerInfo.unitName;
        saveInfo.courseID = playerInfo.courseID;
        saveInfo.unitID = playerInfo.unitID;
        saveInfo.currentMission = playerInfo.currentMission;
        saveInfo.gameVersion = gameVersion;
        saveInfo.saveFileVersion = saveFileVersion;
        saveInfo.saveTime = $"{now.ToString("yyyy.MM.dd")} {now.ToShortTimeString()}";

        jsonContent = JsonUtility.ToJson(saveInfo, true);
        savePath = GetSaveInfoPath(saveInfo.saveId);

        File.WriteAllText(savePath, jsonContent);

    }

    public static PlayerInfo LoadGame(string playerInfoId)
    {
        CheckSavesPath();

        try
        {
            string jsonContent = File.ReadAllText(GetPlayerInfoPath(playerInfoId));
            LoadedPlayerInfo = JsonUtility.FromJson<PlayerInfo>(jsonContent);
            CheckFileVersion(LoadedPlayerInfo);
            return LoadedPlayerInfo;
        }
        catch (Exception e)
        {
            if (e is FileNotFoundException == false)
                Debug.LogException(e);

            return null;
        }

    }

    public static void Delete(string gameInfoId)
    {
        CheckSavesPath();

        TryDeleteFile(GetPlayerInfoPath(gameInfoId));
        TryDeleteFile(GetSaveInfoPath(gameInfoId));
    }

    public static void CheckSavesPath()
    {
        if (!Directory.Exists(SavesPath))
            Directory.CreateDirectory(SavesPath);
    }

    private static void TryDeleteFile(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    private static void CheckFileVersion(SaveInfo saveInfo)
    {
        //Examples
        /*if (playerInfo.saveFileVersion < 3)
        {
            playerInfo.askReview = false;
        }

        if (playerInfo.saveFileVersion < 4)
        {
            playerInfo.AnalyticsInfo.started_games++;
        }*/
    }

    private static void CheckFileVersion(PlayerInfo playerInfo)
    {
        //Examples
        /*if (playerInfo.saveFileVersion < 3)
        {
            playerInfo.askReview = false;
        }

        if (playerInfo.saveFileVersion < 4)
        {
            playerInfo.AnalyticsInfo.started_games++;
        }*/
    }
}
