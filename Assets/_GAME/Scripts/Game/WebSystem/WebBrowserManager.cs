using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class WebBrowserManager : GameSystemComponent
{
    public static WebBrowserManager Instance;

    [SerializeField] private string realPrefix = "http://127.0.0.1:8080/";
    [SerializeField] private string homeWebsite = "cybersearch.com";
    [SerializeField] private string errorWebsite = "error";


    public string HomeWebsite { get => RealPrefix + homeWebsite; }
    public string ErrorWebsite { get => RealPrefix + errorWebsite; }
    private string RealPrefix => realPrefix;

    public string GetFakeLink(string url)
    {
        string tempUrl = url.Replace(RealPrefix, "");

        if(tempUrl.StartsWith("error") == false)
            tempUrl = "https://" + tempUrl;

        if(tempUrl.EndsWith("index.html"))
            tempUrl = tempUrl.Replace("index.html", "");
        
        return tempUrl;
    }

    public string GetTransormedLink(string url)
    {
        string pattern = @"^.*://";
        return RealPrefix + Regex.Replace(url, pattern, "");
    }

    public bool CanOpenWebsite(string url)
    {
        if (url.StartsWith(RealPrefix))
            return true;

        return false;
    }

    private void Awake()
    {
        Instance = this;
    }
}
