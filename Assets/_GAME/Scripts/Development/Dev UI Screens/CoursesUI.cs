using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevTools
{
    public class CoursesUI : UI_Screen
    {
        [SerializeField] private RectTransform coursesParent;
        [SerializeField] private GameObject courseButtonPrefab;

        public override void Back()
        {
            DevEditorUIManager.ShowToolsUI();
            base.Back();
        }

        public override void ShowScreen()
        {
            UpdateCoursesList();
            base.ShowScreen();
        }

        public void AddCourse()
        {
            InputTextUI.SetupInputTextUI("New Course", "New Course ID:");
            InputTextUI.BackButtonEvent.AddListener(DevEditorUIManager.ShowCoursesUI);
            InputTextUI.ContinueButtonEvent.AddListener((string courseID) =>
            {
                Course course = DevToolsManager.Instance.CreateCourse(courseID);

                if(course != null)
                {
                    DevToolsManager.Instance.SetActvieCourse(course);
                    DevEditorUIManager.ShowCourseEditorUI();
                }
                else
                {
                    DevEditorUIManager.HideAll();
                    ContentUI.SetupContentUI("Error", null, $"Course with this id already exists\n{courseID}", "OK");
                    ContentUI.ButtonEvents[0].AddListener(DevEditorUIManager.ShowCoursesUI);
                }
            });
        }

        public void LoadCourse(string courseID)
        {
            DevToolsManager.Instance.SetActvieCourse(CourseManager.GetCourseByID(courseID));
            DevEditorUIManager.ShowCourseEditorUI();
        }

        private void UpdateCoursesList()
        {
            int currentButtonsCount = coursesParent.childCount; 

            for (int i = 0; i < CourseManager.Courses.Count; i++)
            {
                DevContentEditorButton button;

                if (i >= currentButtonsCount)
                    Instantiate(courseButtonPrefab, coursesParent);

                button = coursesParent.GetChild(i).GetComponent<DevContentEditorButton>();
                button.SetupButton(CourseManager.Courses[i]);
            }

            for (int i = currentButtonsCount; i > CourseManager.Courses.Count; i--)
            {
                coursesParent.GetChild(i-1).gameObject.Destroy();
            }
        }
    }
}
