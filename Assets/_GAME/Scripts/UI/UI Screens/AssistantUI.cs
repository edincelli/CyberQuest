using Michsky.UI.Beam;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class AssistantUI : UI_Screen
{
    public static AssistantUI Instance => GameplayUIManager.Instance.AssistantUI;

    [Header("Ai-on")]
    [SerializeField, Range(float.Epsilon, 2f)] private float transitionTime = 1;
    [SerializeField] private CanvasGroup aionActiveUI;
    [SerializeField] private VideoPlayer aionActiveVideoPlayer;
    [SerializeField] private CanvasGroup aionIdleUI;
    [SerializeField] private VideoPlayer aionIdleVideoPlayer;

    [Header("Notifications")]
    [SerializeField] private Transform notificationsParent;
    [SerializeField] private FeedNotification infoNotificationPrefab;
    [SerializeField] private FeedNotification errorNotificationPrefab;

    private float targetAionState = 0;
    private float currentAionState = 0;

    public override void ShowScreen()
    {
        base.ShowScreen();
        ChangeAionState(0);
        FixNotifications();
    }

    public override void HideScreen()
    {
        aionActiveVideoPlayer.Pause();
        aionIdleVideoPlayer.Pause();
        base.HideScreen();
    }

    public void SpawnInfoNotification(string text)
    {
        SpawnNotification(infoNotificationPrefab, text);
    }

    public void SpawnErrorNotifiaction(string text)
    {
        SpawnNotification(errorNotificationPrefab, text);
    }

    public void ChangeAionState(float newState)
    {
        targetAionState = newState;

        if (aionActiveVideoPlayer.isPlaying == false)
            aionActiveVideoPlayer.Play();

        if (aionIdleVideoPlayer.isPlaying == false)
            aionIdleVideoPlayer.Play();
    }

    private void Update()
    {
        UpdateAionVisuals();
    }

    private void UpdateAionVisuals()
    {
        if (currentAionState == targetAionState)
            return;

        currentAionState = Mathf.MoveTowards(currentAionState, targetAionState, Time.deltaTime / transitionTime);

        aionActiveUI.alpha = currentAionState;
        aionIdleUI.alpha = 1 - currentAionState;

        if (currentAionState == targetAionState)
            ToggleVideos();
    }

    private void ToggleVideos()
    {
        if(currentAionState == 1)
        {
            aionActiveVideoPlayer.Play();
            aionIdleVideoPlayer.Pause();
        }
        else
        {
            aionActiveVideoPlayer.Pause();
            aionIdleVideoPlayer.Play();
        }
    }

    private void SpawnNotification(FeedNotification notificationPrefab, string text)
    {
        FeedNotification notification = Instantiate(notificationPrefab, notificationsParent);
        notification.notificationText = text;
        notification.ExpandNotification();
    }

    private void FixNotifications()
    {
        List<FeedNotification> notifications = notificationsParent.GetComponentsInChildren<FeedNotification>().ToList();
        notifications.ForEach(notification => notification.ExpandNotification());
    }
}
