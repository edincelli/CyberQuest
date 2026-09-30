using System;
using System.IO;
using UnityEngine;

public class UtilityMethods
{
    public static void OpenLink(string url, bool useSteamOverlay = true)
    {
        if (SteamManager.Initialized && useSteamOverlay)
            Steamworks.SteamFriends.ActivateGameOverlayToWebPage(url);
        else
            Application.OpenURL(url);
    }

    public static void OpenPath(string path)
    {
        if (Directory.Exists(path))
            System.Diagnostics.Process.Start(path, "explorer.exe");
        else
            Debug.LogError("Invalid path:" + path);
    }

    public static string GetTimestamp(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd_HH-mm-ss_ffff");
    }

    public static string GetTimeDifference(DateTime startDate, DateTime endDate)
    {
        TimeSpan difference = endDate - startDate;

        //if (difference.TotalHours > 6)
            return $"<b>{difference.Days:D2}</b> days<br><b>{difference.Hours:D2}</b> hours <b>{difference.Minutes:D2}</b> minutes <b>{difference.Seconds:D2}</b> seconds";

        //return "<b>Just a few hours</b>";
    }

    public static string GetCurrentTimestamp()
    {
        return GetTimestamp(DateTime.Now);
    }

    public static string WrapText(string text, int maxLineLength)
    {
        if (string.IsNullOrWhiteSpace(text) || maxLineLength <= 0) return text ?? "";

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var sb = new System.Text.StringBuilder();
        int lineLen = 0;

        foreach (var word in words)
        {
            int extra = (lineLen == 0 ? 0 : 1) + word.Length;
            if (lineLen + extra > maxLineLength && lineLen > 0)
            {
                sb.Append('\n');
                sb.Append(word);
                lineLen = word.Length;
            }
            else
            {
                if (lineLen > 0) { sb.Append(' '); lineLen++; }
                sb.Append(word);
                lineLen += word.Length;
            }
        }

        return sb.ToString();
    }


    public static void CopyDirecoryRecirsively(string sourcePath, string targetPath)
    {
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }

    public static void ForceDeleteDirectory(string dir)
    {
        try
        {
            var di = new DirectoryInfo(dir);
            foreach (var info in di.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                try { info.Attributes = FileAttributes.Normal; }
                catch { }
            }
            Directory.Delete(dir, true);
            Debug.Log($"Directory force deleted : {dir}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to force delete directory: {dir}\n{ex.Message}");
        }
    }

}
