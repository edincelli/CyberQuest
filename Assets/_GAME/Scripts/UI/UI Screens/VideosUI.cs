using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Michsky.UI.Beam;
using UnityEngine.Video;
using UnityEditor;

public class VideosUI : UI_Screen
{
    public static VideosUI Instance => GameplayUIManager.Instance.VideosUI;

    [Header("Buttons List")]
    [SerializeField] private GameObject buttonPrefab; 
    [SerializeField] private Transform buttonsParent;

    [Header("Content")]
    [SerializeField] private LayoutGroupFix layoutFixer;
    [SerializeField] private GameObject videoPlayerParent;
    [SerializeField] private TextMeshProUGUI descriptionTMP;

    [Header("Video Player Elements")]
    [SerializeField] private RectTransform videoPlayerContent;
    [SerializeField] private Transform videoSmallParent;
    [SerializeField] private Transform videoFullParent;

    [Header("Video Player Controls")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider videoProgressSlider;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject pauseButton;

    private bool fullScreen = false;
    private VideoPlayer VideoPlayer => VideosManager.VideoPlayer;
    private string currentVideoID;

    private bool shouldUnmuteMusic = false;

    public static void SetupVideosUI(Video video)
    {
        GameplayUIManager.ShowVideosUI();
        Instance.descriptionTMP.text = video.ToString();
        Instance.layoutFixer.FixLayout();
        Instance.currentVideoID = video.videoID;
        Instance.videoPlayerParent.SetActiveOptimized(video.IsVideoURLValid);
    }

    public override void ShowScreen()
    {
        base.ShowScreen();

        fullScreen = true;
        volumeSlider.value = SettingsManager.SettingsInfo.volumeGeneral;
        ResizeVideoPlayer();
        UpdatePlayerButtons();
        RefreshListButtons();
        shouldUnmuteMusic = !AudioManager.IsMusicMuted;
        AudioManager.MuteMusic();
    }

    public override void HideScreen()
    {
        if (gameObject.activeSelf && shouldUnmuteMusic)
            AudioManager.UnmuteMusic();

        base.HideScreen();
    }

    public override void Back()
    {
        if (fullScreen)
        {
            ResizeVideoPlayer();
            return;
        }

        GameplayUIManager.ShowGameUI();
        VideosManager.Instance.StopVideo();
        base.Back();
    }

    public void SpawnButtons()
    {
        for (int i = 0; i < VideosManager.Instance.Videos.Count; i++)
        {
            ButtonManager newButton = Instantiate(buttonPrefab, buttonsParent).GetComponent<ButtonManager>();
            newButton.SetText(VideosManager.Instance.Videos[i].videoName);
            newButton.onClick.AddListener(() =>
            {
                VideosManager.Instance.ShowVideo(newButton.transform.GetSiblingIndex());
            });
        }
    }

    public void ResizeVideoPlayer()
    {
        fullScreen = !fullScreen;

        if (fullScreen)
        {
            videoPlayerContent.SetParent(videoFullParent);
        }
        else
        {
            videoPlayerContent.SetParent(videoSmallParent);
        }

        videoPlayerContent.offsetMin = Vector2.zero;
        videoPlayerContent.offsetMax = Vector2.zero;
    }

    public void PlayVideo()
    {
        VideoPlayer.Play();
        UpdatePlayerButtons();
    }

    public void PauseVideo()
    {
        VideoPlayer.Pause();
        UpdatePlayerButtons();
    }

    public void StopVideo()
    {
        VideoPlayer.Stop();
        UpdatePlayerButtons();
    }

    public void UpdateVolume()
    {
        VideoPlayer.SetDirectAudioVolume(0, volumeSlider.value);
    }

    public void UpdateVideoProgress()
    {
        if (VideoPlayer.frameCount <= 0)
        {
            videoProgressSlider.SetValueWithoutNotify(0);
            return;
        }

        VideoPlayer.frame = (long)(VideoPlayer.frameCount * videoProgressSlider.value);
    }

    private void Start()
    {
        VideoPlayer.loopPointReached += new ((VideoPlayer source) =>
        {
            fullScreen = true;
            ResizeVideoPlayer();
            UpdatePlayerButtons();
        });

        VideoPlayer.started += new((VideoPlayer source) =>
        {
            UpdatePlayerButtons();
        });
    }

    private void Update()
    {
        float progress = 0;

        if (VideoPlayer.frameCount > 0)
            progress = (float)VideoPlayer.frame / (float)VideoPlayer.frameCount;

        videoProgressSlider.SetValueWithoutNotify(progress);

        if (progress <= 0.9f)
            return;

        if (PlayerController.PlayerInfo.watchedVideos.Contains(currentVideoID))
            return;

        PlayerController.PlayerInfo.watchedVideos.Add(currentVideoID);
        RefreshListButtons();
    }

    private void UpdatePlayerButtons()
    {
        playButton.gameObject.SetActiveOptimized(!VideoPlayer.isPlaying);
        pauseButton.gameObject.SetActiveOptimized(VideoPlayer.isPlaying);
    }

    private void RefreshListButtons()
    {
        for (int i = 0; i < VideosManager.Instance.Videos.Count; i++)
        {
            string videoID = VideosManager.Instance.Videos[i].videoID;
            ButtonManager tempButton = buttonsParent.GetChild(i).GetComponent<ButtonManager>();

            tempButton.enableIcon = PlayerController.PlayerInfo.watchedVideos.Contains(videoID);
            tempButton.UpdateUI();
        }
    }
}
