using System;
using System.Collections.Generic;


[Serializable]
public class Video
{
    public string videoID;
    public string videoURL;
    public string videoName;
    public string videoDescription;
    public List<string> links = new List<string>();

    public bool IsVideoURLValid => videoURL.Length >= 10;

    public static Video Example => new Video
    {
        videoID = "id",
        videoName = "name",
        videoDescription = "description",
        videoURL = "https://",
        links = new List<string>()
    };

    public override string ToString()
    {
        return $"<b>{videoName}</b>\n{videoDescription}";
    }
}
