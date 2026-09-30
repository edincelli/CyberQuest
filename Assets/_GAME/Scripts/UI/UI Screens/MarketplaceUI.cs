using GIGA.AutoRadialLayout;
using Michsky.UI.Beam;
using RuntimeInspectorNamespace;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketplaceUI : UI_Screen
{

    public override void Back()
    {
        GameplayUIManager.ShowGameUI();
        base.Back();
    }
}
