using Michsky.UI.Beam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MainMenu
{
    [RequireComponent(typeof(GrayscaleUI))]
    public class CoursesUI_Button : MonoBehaviour
    {
        [SerializeField] private SpotButtonManager buttonManager;
        [Space]
        [SerializeField] private string unlockedText = "Select Course";
        [SerializeField] private string lockedText = "Locked";

        private GrayscaleUI grayscaleUI;
        private Course course;

        public void SetupButton(Course course)
        {
            this.course = course;
            string tempDesc = course.courseDescription;

            //if(tempDesc.Length > 350)
            //    tempDesc = tempDesc.Substring(0, 350) + "...";

            grayscaleUI = GetComponent<GrayscaleUI>();

            buttonManager.SetTitle(course.courseName);
            buttonManager.SetDescription(tempDesc);
            
            Sprite picture = course.GetPicture();

            if (picture != null)
            {
                buttonManager.SetBackground(picture);
            }

            if (course.placeholder == false)
            {
                grayscaleUI.ToDefault();
                buttonManager.SetActionText(unlockedText);
            }
            else
            {
                grayscaleUI.ToGrayscale();
                buttonManager.SetActionText(lockedText);
            }
        }

        public void Click()
        {
            if (course.placeholder)
                return;

            MainMenuController.SelectCourse(course);
            MainMenuUIManager.ShowUnitsUI();
        }
    }
}