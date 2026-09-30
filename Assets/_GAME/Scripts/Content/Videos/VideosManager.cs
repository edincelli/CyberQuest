using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Video;

public class VideosManager : GameSystemComponent
{
    public static VideosManager Instance { get; private set; }

    private VideoPlayer videoPlayer;
    private List<Video> videos = new List<Video>();

    public List<Video> Videos => videos;

    public static VideoPlayer VideoPlayer => Instance.videoPlayer;

    public static string GetStringWithLinks(string baseText)
    {
        if (baseText.IsNullOrEmpty() || Instance.Videos.IsNullOrEmpty())
            return baseText;

        string result = baseText;

        var ordered = Instance.Videos
            .Where(v => v.links != null)
            .SelectMany(v => v.links.Select(l => new { v.videoID, keyword = l }))
            .OrderByDescending(x => x.keyword.Length);

        foreach (var item in ordered)
        {
            if (string.IsNullOrWhiteSpace(item.keyword))
                continue;

            string pattern = Regex.Escape(item.keyword);

            result = Regex.Replace(
                result,
                pattern,
                match => $@"<link=""{item.videoID}""><u>{match.Value}</u></link>",
                RegexOptions.IgnoreCase
            );
        }

        return result;
    }

    public Video GetVideoByID(string videoID)
    {
        for (int i = 0; i < Videos.Count; i++)
        {
            if(Videos[i].videoID == videoID)
                return Videos[i];
        }

        return null;
    }

    public void ShowVideo(string videoID)
    {
        Video video = GetVideoByID(videoID);

        if(video == null)
        {
            LogPrompter.ShowError($"Cannot find video (id):\n{videoID}");
            return;
        }

        ShowVideo(video);
    }

    public void ShowVideo(int videoIndex)
    {
        if(videoIndex< 0 || videoIndex >= Videos.Count)
        {
            LogPrompter.ShowError($"Cannot find video (index):\n{videoIndex}");
            return;
        }

        ShowVideo(Videos[videoIndex]);
    }

    public void ShowVideo(Video video)
    {
        StopVideo();
        VideosUI.SetupVideosUI(video);

        if (video.IsVideoURLValid)
        {
            videoPlayer.url = video.videoURL;
            videoPlayer.Play();
        }
        else
        {
            videoPlayer.Stop();
        }
    }

    public void StopVideo()
    {
        if(videoPlayer.isPlaying)
            videoPlayer.Stop();
    }

    private void Awake()
    {
        Instance = this;
        videoPlayer = GetComponent<VideoPlayer>();
        //LoadAllVideos();
    }

    private void Start()
    {
        LoadVideos();
        VideosUI.Instance.SpawnButtons();
    }

    private void LoadAllVideos()
    {
        try
        {
            videos.AddRange(ContentLoader.ReturnListOfType<Video>(ContentConstValues.FOLDER_VIDEOS, ContentConstValues.EXTENSION_VIDEO));
        }
        catch { }
    }

    private void LoadVideos()
    {
        //List<string> usedVideos = new List<string>();

        //for (int i = 0; i < QuestManager.Quests.Count; i++)
        //{
        //    if (QuestManager.Tasks[i].videoID.IsNullOrEmpty() == false)
        //        usedVideos.Add(QuestManager.Tasks[i].videoID);
        //    if(QuestManager.Tasks[i].Links.IsNullOrEmpty() == false)
        //        usedVideos.AddRange(QuestManager.Tasks[i].Links);
        //}

        //usedVideos = usedVideos.Distinct().ToList();

        //for (int i = 0; i < usedVideos.Count; i++)
        //{
        //    try
        //    {
        //        videos.Add(ContentLoader.ReturnObjectOfType<Video>(usedVideos[i], 
        //            ContentConstValues.FOLDER_VIDEOS, ContentConstValues.EXTENSION_VIDEO, true));
        //    }
        //    catch { }
        //}

        videos.AddRange(
            ContentLoader.ReturnListOfType<Video>(
                ContentConstValues.FOLDER_VIDEOS,
                ContentConstValues.EXTENSION_VIDEO,
                true
            ).OrderBy(x => x.videoName)
            .ToArray());
    }
}
