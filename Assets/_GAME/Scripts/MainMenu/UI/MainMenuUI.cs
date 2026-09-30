using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MainMenu
{
    public class MainMenuUI : UI_Screen
    {
        public override void Back()
        {
            Quit();
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

        public void ShowHome()
        {
            MainMenuUIManager.ShowHomeUI();
        }

        public void ShowLoadGame()
        {
            MainMenuUIManager.ShowLoadGameUI();
        }

        public void ShowCourses()
        {
            MainMenuUIManager.ShowCoursesUI();
        }

        public void ShowAchievements()
        {
            MainMenuUIManager.ShowAchievementsUI();
        }

        public void OpenEditor()
        {
            SceneLoader.LoadSceneAsync(SceneLoader.TOOLS_SCENE_NAME);
        }

        public void ShowSettings()
        {
            MainMenuUIManager.ShowSettingsUI();
        }

        public void Quit()
        {
            ContentUI.SetupContentUI("EXIT", null, "Are you sure you want to exit the game?", "Back", "Quit");
            ContentUI.ButtonEvents[0].AddListener(MainMenuUIManager.ShowHomeUI);
            ContentUI.ButtonEvents[1].AddListener(() =>
            {
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                    Application.Quit();
                }
            });
            ContentUI.DefaultButtonIndex = 0;
        }
    }
}
