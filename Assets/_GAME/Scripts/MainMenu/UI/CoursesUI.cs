using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MainMenu
{
    public class CoursesUI : UI_Screen
    {
        [SerializeField] private RectTransform coursesParent;
        [SerializeField] private GameObject courseButtonPrefab;

        public override void Back()
        {
            MainMenuUIManager.ShowHomeUI();
            MainMenuController.SelectUnit(null);
            base.Back();
        }

        public override void ShowScreen()
        {
            base.ShowScreen();
        }

        public override void HideScreen()
        {
            base.HideScreen();
        }

        private void Start()
        {
            PrepareCourseButtons();
        }

        private void PrepareCourseButtons()
        {
            for (int i = 0; i < CourseManager.Courses.Count; i++)
            {
                CoursesUI_Button button = Instantiate(courseButtonPrefab, coursesParent).GetComponent<CoursesUI_Button>();
                button.SetupButton(CourseManager.Courses[i]);
            }
        }
    }
}
