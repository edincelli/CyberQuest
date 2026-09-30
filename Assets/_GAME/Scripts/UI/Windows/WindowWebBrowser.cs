using Michsky.UI.Beam;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VoltstroStudios.UnityWebBrowser;
using VoltstroStudios.UnityWebBrowser.Core;
using Resolution = VoltstroStudios.UnityWebBrowser.Shared.Resolution;

public class WindowWebBrowser : WindowBase
{
    [Header("Web Browser")]
    [SerializeField] private WebBrowserUIBasic browser;
    [SerializeField] private ButtonManager backButton;
    [SerializeField] private ButtonManager homeButton;
    [SerializeField] private TMP_InputField urlInputField;

    private WebBrowserClient browserClient;

    private bool preventClosing = false;


    public override void ClickClose()
    {
        if (preventClosing)
            return;

        ClientLoadURL(WebBrowserManager.Instance.HomeWebsite);
        base.ClickClose();
    }

    public void ClickBack()
    {
        browserClient.GoBack();
    }

    public void ClickHomeWebsite()
    {
        ClientLoadURL(WebBrowserManager.Instance.HomeWebsite);
    }

    public void OpenWebsite(string targetWebsite)
    {
        targetWebsite = WebBrowserManager.Instance.GetTransormedLink(targetWebsite);
        ClientLoadURL(targetWebsite);
    }

    public void OpenWebsite()
    {
        OpenWebsite(urlInputField.text);
    }

    public string GetActiveWebsite()
    {
        return urlInputField.text;
    }

    public void ToggleNavigationElements(bool isEnabled)
    {
        backButton.Interactable(isEnabled);
        homeButton.Interactable(isEnabled);
        urlInputField.interactable = isEnabled;
    }

    public void TogglePreventingClosing(bool preventClosing)
    {
        this.preventClosing = preventClosing;
    }

    protected override void Awake()
    {
        browserClient = browser.browserClient;
        base.Awake();
    }

    private void Start()
    {
        browserClient.initialUrl = WebBrowserManager.Instance.HomeWebsite;
        browserClient.OnLoadStart += UpdateUrl;
        browserClient.OnLoadFinish += UpdateUrl;
        browserClient.OnUrlChanged += UpdateUrl;

        WindowFlexibilityComponent.OnResizeingEndEvent.AddListener(() =>
        {
            RectTransform browserRectTransform = browser.GetComponent<RectTransform>();
            uint width = (uint)browserRectTransform.rect.width;
            uint height = (uint)browserRectTransform.rect.height;
            browserClient.Resize(new Resolution(width, height));
        });
    }

    private void OnEnable()
    {
        if (PlayerController.PlayerInfo == null)
            return;

        if (PlayerController.PlayerInfo.skills.IsNullOrEmpty())
            return;

        if (PlayerController.PlayerInfo.skills.Contains("anonsurf") == false)
            return;

        SetHeader("AnonSurf - Web Browser");
    }

    private void ClientLoadURL(string urlToOpen)
    {
        browserClient.LoadUrl(urlToOpen);
    }

    private void UpdateUrl(string newUrl)
    {
        Debug.Log("Updating URL: " + newUrl);

        if (WebBrowserManager.Instance.CanOpenWebsite(newUrl) == false)
        {
            ClientLoadURL(WebBrowserManager.Instance.ErrorWebsite);
            return;
        }

        string fakeUrl = WebBrowserManager.Instance.GetFakeLink(newUrl);
        urlInputField.SetTextWithoutNotify(fakeUrl);
    }
}
