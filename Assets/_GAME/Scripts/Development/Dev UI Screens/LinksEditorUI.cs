using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeInspectorNamespace;
using TMPro;

namespace DevTools
{
    public class LinksEditorUI : UI_Screen
    {
        [SerializeField] private RuntimeInspector runtimeInspector;

        [Space]
        [SerializeField] private TextMeshProUGUI issuesTMP;

        public override void Back()
        {
            DevContentUtilities.SaveLinks(LinksManager.Instance.Links);
            DevEditorUIManager.ShowToolsUI();
            base.Back();
        }

        public override void ShowScreen()
        {
            runtimeInspector.Inspect(LinksManager.Instance.Links);
            base.ShowScreen();
        }

        public override void HideScreen()
        {
            base.HideScreen();
        }

        private void FixedUpdate()
        {
            bool validationGood = LinksManager.Instance.Links.ValidateContent(out string message);
            issuesTMP.gameObject.SetActiveOptimized(!validationGood);

            if (issuesTMP.text == message)
                issuesTMP.text = message + "<size=1>.</size>";

            if (validationGood == false)
                issuesTMP.text = message;
        }
    }
}
