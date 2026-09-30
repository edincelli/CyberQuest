using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : GameSystemComponent
{
    public static GameController Instance { get; private set; }

    public static Course SelectedCourse { get; private set; }
    public static Unit SelectedUnit { get; private set; }

    public static PlayerInfo CurrentPlayerInfo { get; private set; }

    public static void SetupNewGameplay(Course course, Unit unit)
    {
        SetupNewGameplayWithoutReload(course, unit);
        SceneLoader.LoadSceneAsync(SceneLoader.GAME_SCENE_NAME);
    }

    public static void SetupNewGameplayWithoutReload(Course course, Unit unit)
    {
        string newSaveId = $"{course.courseID}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}";

        SelectedCourse = course;
        SelectedUnit = unit;
        CurrentPlayerInfo = new PlayerInfo(newSaveId, course.courseID, unit.unitID);

        PlayerController.SetupNewGame = PlayerController.SetupGameType.New;
        QuestManager.LoadQuestsAndTasks(course.courseID, unit.unitID, true);
        MailsManager.LoadMails(course.courseID, unit.unitID, true);
    }

    public static void SetupLoadedGameplay(PlayerInfo playerInfo)
    {
        SelectedCourse = CourseManager.GetCourseByID(playerInfo.courseID);

        if (SelectedCourse == null)
            return;

        SelectedUnit = CourseManager.GetUnitByID(SelectedCourse, playerInfo.unitID);

        if(SelectedUnit == null)
            return;

        CurrentPlayerInfo = playerInfo;
        PlayerController.SetupNewGame = PlayerController.SetupGameType.Load;
        QuestManager.LoadQuestsAndTasks(SelectedCourse.courseID, SelectedUnit.unitID, true);
        MailsManager.LoadMails(SelectedCourse.courseID, SelectedUnit.unitID, true);
        SceneLoader.LoadSceneAsync(SceneLoader.GAME_SCENE_NAME);
    }

    public static void SaveGame()
    {
        if (PlayerController.Instance == null)
            return;

        if (CurrentPlayerInfo == null)
            return;

        CurrentPlayerInfo.unitName = SelectedUnit.unitName;
        CurrentPlayerInfo.courseID = SelectedCourse.courseID;
        CurrentPlayerInfo.unitID = SelectedUnit.unitID;

        SaveLoad.Save(CurrentPlayerInfo);
    }

    private void Awake()
    {
        if(Instance != null)
        {
            gameObject.Destroy();
            return;
        }

        Instance = this;
        gameObject.DontDestroyOnLoadImproved();
    }

    private void Start()
    {
        
    }
}
