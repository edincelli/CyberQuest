using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public static class ContentLoader
{
    public static List<T> ReturnListOfType<T>(string path, string extension, bool showError = true)
    {
        string contentPath = GetContentPath(path);
        List<T> contentsList = new List<T>();
        
        if(Directory.Exists(contentPath) == false)
            return contentsList;

        string[] filePaths = Directory.GetFiles(contentPath, $"*.{extension}", SearchOption.AllDirectories);

        for (int i = 0; i < filePaths.Length; i++)
        {
            try
            {
                string tempContent = File.ReadAllText(filePaths[i]);
                T contentObject = JsonUtility.FromJson<T>(tempContent);
                contentsList.Add(contentObject);
            }
            catch (Exception e)
            {
                if (showError)
                    LogPrompter.ShowError($"Cannot read the file:\n{filePaths[i]}\n{e.Message}");

                Debug.LogError("Cannot read the file: " + filePaths[i] + "\n" + e.Message);
            }
        }

        return contentsList;
    }

    public static T ReturnObjectOfType<T>(string fileName, string path, string extension = "json", bool showError = true)
    {
        string combinedPath = CombinePath(path, $"{fileName}.{extension}");
        return ReturnObjectOfType<T>(combinedPath, showError);
    }

    public static T ReturnObjectOfType<T>(string path, bool showError = true)
    {
        string contentPath = GetContentPath(path);

        try
        {
            string tempContent = File.ReadAllText(contentPath);
            T contentObject = JsonUtility.FromJson<T>(tempContent);
            return contentObject;
        }
        catch (Exception e)
        {
            if (showError)
                LogPrompter.ShowError($"Cannot read the file:\n{contentPath}\n{e.Message}");

            Debug.LogError("Cannot read the file: " + contentPath + "\n" + e.Message);
            return default;
        }
    }

    public static (List<Quest> quests, List<TaskBase> tasks) ReturnListOfQuestsAndTasks(string coursePrefix, string unitPrefix, 
        string questExtension = ContentConstValues.EXTENSION_QUEST, string taskExtension = ContentConstValues.EXTENSION_TASK, 
        bool showError = true)
    {
        string questsPath = GetContentPath(ContentConstValues.FOLDER_QUESTS);

        if(coursePrefix.IsNullOrEmpty() == false)
        {
            questsPath = CombinePath(questsPath, coursePrefix);

            if(unitPrefix.IsNullOrEmpty() == false)
                questsPath = CombinePath(questsPath, unitPrefix);
        }

        List<Quest> questsList = ReturnListOfType<Quest>(questsPath, questExtension, showError);
        List<TaskBase> tasksList = new List<TaskBase>();

        string[] filePathsTasks = Directory.GetFiles(questsPath, $"{unitPrefix}*.{taskExtension}", SearchOption.AllDirectories);

        for (int i = 0; i < filePathsTasks.Length; i++)
        {
            try
            {
                string tempJson = File.ReadAllText(filePathsTasks[i]);
                JObject tempJsonObject = JObject.Parse(tempJson);

                string taskTypeString = tempJsonObject["TaskType"].ToString();
                Type taskType = QuestManager.GetTaskType(taskTypeString);

                tasksList.Add((TaskBase)tempJsonObject.ToObject(taskType));
            }
            catch (Exception e)
            {
                if (showError)
                    LogPrompter.ShowError($"Cannot read the file:\n{filePathsTasks[i]}\n{e.Message}");

                Debug.LogError("Cannot read the file: " + filePathsTasks[i] + "\n" + e.Message);
            }
        }

        return (questsList, tasksList);
    }

    public static Dictionary<string, Texture2D> ReturnPictures(string path, List<string> fileNames = null, string extension = "png", bool showError = true)
    {
        Dictionary<string, Texture2D> pictures = new Dictionary<string, Texture2D>();
        string contentPath = GetContentPath(path);
        string[] filePaths;

        if (fileNames.IsNullOrEmpty())
        {
            filePaths = Directory.GetFiles(contentPath, $"*.{extension}", SearchOption.AllDirectories);
        }
        else
        {
            filePaths = new string[fileNames.Count];

            for (int i = 0; i < fileNames.Count; i++)
            {
                filePaths[i] = Path.Combine(contentPath, $"{fileNames[i]}.{extension}");
            }
        }

        for (int i = 0; i < filePaths.Length; i++)
        {
            string filePath = filePaths[i];
            try
            {
                byte[] tempFileBytes = File.ReadAllBytes(filePath);

                if (tempFileBytes == null || tempFileBytes.Length == 0)
                {
                    Debug.LogError($"[ReturnPictures] Empty or unreadable file: {filePath}");
                    continue;
                }

                Texture2D newPicture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                bool success = newPicture.LoadImage(tempFileBytes, markNonReadable: false);
                if (!success)
                {
                    Debug.LogError($"[ReturnPictures] LoadImage failed: {filePath}");
                    continue;
                }

                string pictureName = Path.GetFileNameWithoutExtension(filePath);
                pictures[pictureName] = newPicture;
            }
            catch (Exception e)
            {
                if (showError)
                    LogPrompter.ShowError($"Cannot read the file:\n{filePath}\n{e.Message}");

                Debug.LogError($"[ReturnPictures] Cannot read the file: {filePath}\n{e}");
            }
        }

        return pictures;
    }

    public static string GetFileTextFromContent(string path)
    {
        return File.ReadAllText(CombinePath(GetContentPath(), path));
    }

    public static string GetContentPath(string path)
    {
        if (path == null)
            return GetContentPath();

        return CombinePath(GetContentPath(), path);
    }

    public static string GetContentPath()
    {
#if UNITY_EDITOR
        return CombinePath(Application.dataPath, ContentConstValues.FOLDER_CONTENT);
#elif UNITY_STANDALONE
        return CombinePath(Application.dataPath, ContentConstValues.FOLDER_CONTENT);

#elif UNITY_WEBGL
        LogPrompter.ShowError("Content Loader doesn't have path for CONTENT\nUNITY_WEBGL");
        return "";
#else
        LogPrompter.ShowError("Content Loader doesn't have path for CONTENT");
        return "";
#endif
    }

    public static string CombinePath(params string[] pathElements)
    {
        if (pathElements.Length == 0)
            throw new NullReferenceException("PathElements cannot be empty");

        string path = pathElements[0];

        for (int i = 1; i < pathElements.Length; i++)
        {
            path = Path.Combine(path, pathElements[i]);
        }

        return path;
    }
}
