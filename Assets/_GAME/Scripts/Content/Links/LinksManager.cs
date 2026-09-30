using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LinksManager : GameSystemComponent
{
    public static LinksManager Instance { get; private set; }

    [SerializeField] private Sprite tempVideoSprite;


    private LinkList links = new LinkList();

    public LinkList Links => links;

    public void ClickLink(string linkID, LinksSupportTMP.UIAfterClick uiAfterClick)
    {
        VideosManager.Instance.ShowVideo(linkID);
        //if (links == null)
        //    return;

        //if (links.list.IsNullOrEmpty())
        //    return;

        //Link link = GetLinkByID(linkID);

        //if(link == null) 
        //    return;

        //switch (link.linkAction)
        //{
        //    case Link.LinkAction.None:
        //        ContentUI.SetupContentUI(link.linkName, tempVideoSprite, link.linkText, "Back");
        //        SetupUIAfterLink(uiAfterClick);
        //        break;
        //    default:
        //        break;
        //}
    }

    private Link GetLinkByID(string linkID)
    {
        for (int i = 0; i < links.list.Count; i++)
        {
            if(links.list[i].linkID == linkID)
                return links.list[i];
        }

        return null;
    }

    private void SetupUIAfterLink(LinksSupportTMP.UIAfterClick uiAfterClick)
    {
        switch (uiAfterClick)
        {
            case LinksSupportTMP.UIAfterClick.GameUI:
                ContentUI.ButtonEvents[0].AddListener(GameplayUIManager.ShowGameUI);
                break;

            default:
                ContentUI.ButtonEvents[0].AddListener(GameplayUIManager.ShowGameUI);
                break;
        }
    }

    private void Awake()
    {
        Instance = this;
        LoadLinks();
    }

    private void LoadLinks()
    {
        try
        {
            links.list.AddRange(ContentLoader.ReturnObjectOfType<LinkList>(ContentLoader.CombinePath(ContentConstValues.FOLDER_CONFIG, $"links.{ContentConstValues.EXTENSION_LINKS}")).list);
        }
        catch { }
    }

}
