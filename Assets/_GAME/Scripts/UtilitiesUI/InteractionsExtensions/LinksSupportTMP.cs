using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LinksSupportTMP : UIInteractions
{
    [SerializeField] private UIAfterClick actionAfterLink;

    private TextMeshProUGUI tmp;
    private Canvas m_Canvas;
    private Camera m_Camera;

    private bool isHoveringObject;
    private int m_selectedLink = -1;

    private void Awake()
    {
        tmp = gameObject.GetComponent<TextMeshProUGUI>();

        m_Canvas = gameObject.GetComponentInParent<Canvas>();

        if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            m_Camera = null;
        else
            m_Camera = m_Canvas.worldCamera;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (isHoveringObject == false)
            return;

        if (m_selectedLink < 0)
            return;

        TMP_LinkInfo linkInfo = tmp.textInfo.linkInfo[m_selectedLink];
        string linkID = linkInfo.GetLinkID();

        if (LinksManager.Instance != null)
            LinksManager.Instance.ClickLink(linkID, actionAfterLink);
        else
            Debug.Log(linkID);

        base.OnPointerClick(eventData);
    }

    void LateUpdate()
    {
        if (isHoveringObject)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmp, Input.mousePosition, m_Camera);

            if ((linkIndex == -1 && m_selectedLink != -1) || linkIndex != m_selectedLink)
                m_selectedLink = -1;

            if (linkIndex != -1 && linkIndex != m_selectedLink)
                m_selectedLink = linkIndex;
        }
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        isHoveringObject = true;
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        isHoveringObject = false;
        base.OnPointerExit(eventData);
    }

    [Serializable]
    public enum UIAfterClick
    {
        GameUI
    }
}
