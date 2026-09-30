using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : GameSystemComponent
{
    public static MainMenuController Instance { get; private set; }

    public static Course SelectedCourse { get; private set; }
    public static Unit SelectedUnit { get; private set; }

    public static void SelectCourse(int courseIndex)
    {
        SelectCourse(CourseManager.Courses[courseIndex]);
    }

    public static void SelectCourse(Course course)
    {
        SelectedCourse = course;
    }

    public static void SelectUnit(int unitIndex)
    {
        SelectUnit(SelectedCourse.units[unitIndex]);
    }

    public static void SelectUnit(Unit unit)
    {
        SelectedUnit = unit;
    }

    public static void StartNewGame()
    {
        if (SelectedCourse == null)
            return;

        if (SelectedUnit == null)
            return;

        GameController.SetupNewGameplay(SelectedCourse, SelectedUnit);
    }

    public static void StartLoadGame(SaveInfo saveInfo)
    {
        PlayerInfo loadedPlayerInfo = SaveLoad.LoadGame(saveInfo.saveId);

        if(loadedPlayerInfo == null)
            return;

        GameController.SetupLoadedGameplay(loadedPlayerInfo);
    }

    private void Awake()
    {
        Instance = this;
    }
}
