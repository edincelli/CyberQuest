using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimeInspectorNamespace;
using TMPro;

namespace DevTools
{
    public class CourseEditorUI : UI_Screen
    {
        [SerializeField] private RectTransform unitsParent;
        [SerializeField] private GameObject unitsButton;
        [SerializeField] private RuntimeInspector runtimeInspector;

        [Space]
        [SerializeField] private TextMeshProUGUI issuesTMP;

        private Course ActiveCourse => DevToolsManager.ActiveCourse;
        
        public override void Back()
        {
            DevContentUtilities.SaveCourse(ActiveCourse);
            DevEditorUIManager.ShowCoursesUI();
            DevToolsManager.Instance.SetActvieCourse(null);
            base.Back();
        }

        public override void ShowScreen()
        {
            UpdateUnitsList();
            runtimeInspector.Inspect(ActiveCourse);
            base.ShowScreen();
        }

        public override void HideScreen()
        {
            base.HideScreen();
        }

        public void AddUnit()
        {
            InputTextUI.SetupInputTextUI("New Unit", $"New Unit ID:\n{ActiveCourse.courseID}_");
            InputTextUI.BackButtonEvent.AddListener(DevEditorUIManager.ShowCourseEditorUI);
            InputTextUI.ContinueButtonEvent.AddListener((string unitID) =>
            {
                Unit unit = DevToolsManager.Instance.CreateUnit(ActiveCourse.courseID + "_" + unitID);

                if (unit != null)
                {
                    DevEditorUIManager.ShowCourseEditorUI();
                }
                else
                {
                    DevEditorUIManager.HideAll();
                    ContentUI.SetupContentUI("Error", null, $"Unit with this id already exists:\n{unitID}", "OK");
                    ContentUI.ButtonEvents[0].AddListener(DevEditorUIManager.ShowCourseEditorUI);
                }
            });
        }

        public void LoadCourse(string courseID)
        {
            DevToolsManager.Instance.SetActvieCourse(CourseManager.GetCourseByID(courseID));
            DevEditorUIManager.ShowCourseEditorUI();
        }

        private void UpdateUnitsList()
        {
            int currentButtonsCount = unitsParent.childCount;

            for (int i = 0; i < ActiveCourse.units.Count; i++)
            {
                DevContentEditorButton button;

                if (i >= currentButtonsCount)
                    Instantiate(unitsButton, unitsParent);

                button = unitsParent.GetChild(i).GetComponent<DevContentEditorButton>();
                button.SetupButton(ActiveCourse.units[i], ActiveCourse);
            }

            for (int i = currentButtonsCount; i > ActiveCourse.units.Count; i--)
            {
                unitsParent.GetChild(i - 1).gameObject.Destroy();
            }
        }

        private void FixedUpdate()
        {
            bool validationGood = ActiveCourse.ValidateContent(out string message);
            issuesTMP.gameObject.SetActiveOptimized(!validationGood);

            if (issuesTMP.text == message)
                issuesTMP.text = message + "<size=1>.</size>";

            if (validationGood == false)
                issuesTMP.text = message;
        }
    }
}
