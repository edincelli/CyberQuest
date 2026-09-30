using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using WPM;

public class PlayerController : GameSystemComponent
{
    public static PlayerController Instance;
    private WorldMapGlobe globe;
    private List<QuestReferences> questReferences = new List<QuestReferences>();
    private List<TaskReferences> taskReferences = new List<TaskReferences>();
    
    private TaskReferences activeTask = null;
    private QuestReferences activeQuest = null;
    private UnitReferences activeUnit = null;
    private CourseReferences activeCourse = null;

    private string lastQuestID = string.Empty;

    public TaskReferences ActiveTask { get => activeTask; private set => activeTask = value; }
    public QuestReferences ActiveQuest { get => activeQuest; private set => activeQuest = value; }
    public UnitReferences ActiveUnit { get => activeUnit; private set => activeUnit = value; }
    public CourseReferences ActiveCourse { get => activeCourse; private set => activeCourse = value; }

    [ShowInInspector]
    public static PlayerInfo PlayerInfo => GameController.CurrentPlayerInfo;
    public static SetupGameType SetupNewGame { get; set; } = SetupGameType.New;

    public UnityEvent OnProgressUpdate { get; set; } = new UnityEvent();

    #region Public Methods
    public void CompleteActiveTask(int score, bool done = true, bool skipped = false)
    {
        if (ActiveTask == null)
            return;

        ActiveTask.info.timerStarted = false;

        if (CheatsManager.CheatingActivated == false)
        {
            ActiveTask.info.isDone = done;
            ActiveTask.info.isSkipped = skipped;
        }
        else
        {
            ActiveTask.info.isCheated = true;
        }

        if (skipped == false)
            SD_MoenyLabel.ChangeMoney(400);

        MailsManager.Instance.RecieveMails(ActiveTask.data.mailsOnEnd);

        OnProgressUpdate.Invoke();

        if (ActiveQuest.info.IsDone)
            CompleteActiveQuest();

        GameController.SaveGame();
    }

    private void CompleteActiveQuest()
    {
        if (ActiveQuest == null)
            return;

        lastQuestID = ActiveQuest.questID;
        ActiveQuest.info.timerStarted = false;

        OnProgressUpdate.Invoke();
        StickerManager.Instance.UpdateAllQuestStickers();
    }

    public void OpenQuest(string questID)
    {
        GameplayUIManager.ShowQuestsUI();
    }

    public void FlyToQuest(QuestReferences quest)
    {
        if(quest == null)
            return;
        
        int cityIndex = quest.data.cityIndex;   
        globe.FlyToCity(cityIndex, 1f);
    }

    public void HandleNextQuestOrTask(QuestReferences questRef, TaskReferences taskRef)
    {
        if (ActiveQuest != null)
            if (ActiveQuest.info.IsDone)
                CompleteActiveQuest();

        NextQuestAction action = GetNextQuestAction(questRef, taskRef);

        switch (action)
        {
            case NextQuestAction.allDone:
                GameplayUIManager.ShowProgressUI();
                //GameplayUIManager.ShowQuestsUI();
                //QuestsUI.Instance.ShowQuestDetails(questRef);
                break;
            case NextQuestAction.nextQuest:
                StartNextQuest();
                break;
            case NextQuestAction.nextTask:
                StartNextTask();
                break;
            case NextQuestAction.showProgressUI:
                GameplayUIManager.ShowQuestsUI();
                QuestsUI.Instance.ShowQuestDetails(questRef);
                break;
            case NextQuestAction.error:
                GameplayUIManager.ShowQuestsUI();
                QuestsUI.Instance.ShowQuestDetails(questRef);
                break;
            default:
                GameplayUIManager.ShowQuestsUI();
                QuestsUI.Instance.ShowQuestDetails(questRef);
                break;
        }
    }

    public void ResetProgressToQuestIndex(int questIndex, bool startImmediately)
    {
        string questID = ActiveUnit.info.quests[questIndex].questID;

        for (int i = ActiveUnit.info.quests.Count - 1; i >= questIndex; i--)
        {
            QuestInfo qInfo = ActiveUnit.info.quests[i];
            qInfo.questTimer = 0;
            qInfo.timerStarted = false;

            QuestReferences tempQuestRef = GetQuestReferencesByID(qInfo.questID);
            if (tempQuestRef.info != null)
            {
                tempQuestRef.info.questTimer = 0;
                tempQuestRef.info.timerStarted = false;
            }


            for (int t = 0; t < qInfo.tasks.Count; t++)
            {
                TaskInfo tInfo = qInfo.tasks[t];
                tInfo.isDone = false;
                tInfo.isCheated = false;
                tInfo.isSkipped = false;
                tInfo.timerStarted = false;
                tInfo.taskTimer = 0;

                TaskReferences tempTaskRef = GetTaskReferencesByID(tInfo.taskID);
                if(tempTaskRef.info != null)
                {
                    tempTaskRef.info.isDone = false;
                    tempTaskRef.info.isCheated = false;
                    tempTaskRef.info.isSkipped = false;
                    tempTaskRef.info.timerStarted = false;
                    tempTaskRef.info.taskTimer = 0;

                    if(tempTaskRef.info.decisionTaken >= 0)
                        DecisionsManager.ReverseDecision(tempTaskRef);
                }
            }

            tempQuestRef.sticker.UpdateSticker();
            ActiveUnit.info.quests.RemoveAt(i);
        }

        ActiveUnit.info.quests.RemoveRange(questIndex, ActiveUnit.info.quests.Count - questIndex);

        ActiveQuest = null;
        ActiveTask = null;
        
        GameController.SaveGame();

        if (startImmediately)
        {
            StartQuest(questID);
        }
    }
    #endregion

    #region Getters
    public QuestReferences GetQuestReferencesByID(string questID)
    {
        for (int i = 0; i < questReferences.Count; i++)
        {
            if (questReferences[i].questID == questID)
                return questReferences[i];
        }

        return null;
    }

    public int GetQuestIndexByID(string questID)
    {
        if (ActiveUnit.data.questIds.Contains(questID) == false)
            return -1;

        return ActiveUnit.data.questIds.IndexOf(questID);
    }

    public TaskReferences GetTaskReferencesByID(string taskID)
    {
        for (int i = 0; i < taskReferences.Count; i++)
        {
            if (taskReferences[i].taskID == taskID)
                return taskReferences[i];
        }

        return null;
    }

    public CourseInfo GetCourseInfoByID(string courseID)
    {
        for (int i = 0; i < PlayerInfo.courses.Count; i++)
        {
            if (courseID == PlayerInfo.courses[i].courseID)
                return PlayerInfo.courses[i];
        }

        return null;
    }

    public UnitInfo GetUnitInfoByID(string unitID)
    {
        if (ActiveCourse == null)
            return null;

        for (int i = 0; i < ActiveCourse.info.units.Count; i++)
        {
            if (unitID == ActiveCourse.info.units[i].unitID)
                return ActiveCourse.info.units[i];
        }

        return null;
    }

    public QuestInfo GetQuestInfoByID(string questID)
    {
        if (ActiveUnit == null)
            return null;

        for (int i = 0; i < ActiveUnit.info.quests.Count; i++)
        {
            if (questID == ActiveUnit.info.quests[i].questID)
                return ActiveUnit.info.quests[i];
        }

        return null;
    }

    public TaskReferences GetRecentTaskReferencesInQuest(QuestReferences questRef)
    {
        if (questRef == null)
        {
            LogPrompter.ShowError("questReferences.tasks.IsNullOrEmpty()", gameObject);
            return null;
        }

        if (questRef.tasks.IsNullOrEmpty())
        {
            LogPrompter.ShowError("questReferences.tasks.IsNullOrEmpty()", gameObject);
            return null;
        }

        TaskReferences recentTaskReferences = questRef.tasks[0];

        if (recentTaskReferences.info == null && SetupNewGame != SetupGameType.Load)
        {
            LogPrompter.ShowError("recentTaskReferences.info == null", gameObject);
            return recentTaskReferences;
        }

        for (int i = 0; i < questRef.tasks.Count; i++)
        {
            TaskInfo taskInfo = questRef.tasks[i].info;

            if (taskInfo == null)
            {
                break;
            }

            if (taskInfo.IsDone == false)
            {
                recentTaskReferences = questRef.tasks[i];
                break;
            }

            if(taskInfo.IsDone)
            {
                recentTaskReferences = questRef.tasks[i];
            }
        }

        return recentTaskReferences;
    }

    private NextQuestAction GetNextQuestAction(QuestReferences questRef, TaskReferences taskRef)
    {
        int finishedQuests = ActiveUnit.info.quests.Where(x => x.IsDone).Count();
        int allQuests = ActiveUnit.data.questIds.Count;

        if (finishedQuests == allQuests)
            return NextQuestAction.allDone;

        if (questRef.info.IsDone)
        {
            QuestReferences followingQuest = GetQuestReferencesByID(GetFollowingQuest(questRef.questID).questID);

            if (followingQuest == null)
                return NextQuestAction.showProgressUI;
            else
                return NextQuestAction.nextQuest;
        }

        int selectedTaskIndex = questRef.info.tasks.FindIndex(task => task.taskID == taskRef.taskID);
        int progress = questRef.info.tasks.Where(task => task.IsDone).Count();

        if (selectedTaskIndex + 1 != progress)
        {
            LogPrompter.ShowError("Selected Task is not the next one!", gameObject);
            return NextQuestAction.error;
        }

        return NextQuestAction.nextTask;
    }

    public (string, string) GetNextQuestButtonText(QuestReferences questRef, TaskReferences taskRef)
    {
        NextQuestAction action = GetNextQuestAction(questRef, taskRef);

        switch (action)
        {
            case NextQuestAction.allDone:
                return ("Finish Scenario", "Scenario Completed");
            case NextQuestAction.nextQuest:
                return ("Next Mission", "Quest Completed");
            case NextQuestAction.nextTask:
                return ("Continue", "Task Completed");
            case NextQuestAction.showProgressUI:
                return ("Show Available Quests", "Quest Completed");
            case NextQuestAction.error:
                return ("Error", "---");
            default:
                return ("Error", "---");
        }
    }

    private List<QuestReferences> GetUnlockedQuests()
    {
        List<QuestReferences> unlockedQuests = new List<QuestReferences>();

        for (int i = 0; i < questReferences.Count; i++)
        {
            if (questReferences[i].info == null)
                continue;

            if (questReferences[i].info.IsDone)
                continue;

            unlockedQuests.Add(questReferences[i]);
        }

        return unlockedQuests;
    }

    private Quest GetPreviousQuest(string questID)
    {
        int givenQuestIndex = ActiveUnit.data.questIds.IndexOf(questID);

        if (givenQuestIndex <= 0)
            return null;

        return QuestManager.GetQuestByID(ActiveUnit.data.questIds[givenQuestIndex - 1]);
    }

    private Quest GetFollowingQuest(string questID)
    {
        int givenQuestIndex = ActiveUnit.data.questIds.IndexOf(questID);

        if (givenQuestIndex < 0 || givenQuestIndex >= ActiveUnit.data.questIds.Count - 1)
            return null;

        return QuestManager.GetQuestByID(ActiveUnit.data.questIds[givenQuestIndex + 1]);
    }
    #endregion

    #region Lifecycle Methods

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        globe = WorldMapGlobe.instance;
        string status = "";

        switch (SetupNewGame)
        {
            case SetupGameType.New:
                status = "New Game";
                break;
            case SetupGameType.Load:
                status = "Loaded Game";
                break;
            default:
                status = "ERROR!!";
                break;
        }

        Debug.Log($"PlayerController: {status}");

        SetupContent();
        LoadMails();
        PrepareQuests();

        if (SetupNewGame == SetupGameType.New)
        {
            StartCoroutine(ShowEntryMessage());
        }
        else
        {
            AssignLoadedElementsToReferences();
            StartLoadedQuest();
        }

        StickerManager.Instance.UpdateAllQuestStickers();
        SetupNewGame = SetupGameType.None;
    }

    private void Update()
    {
        PlayerInfo.playTime += Time.deltaTime / 60f;

        UpdateActiveQuest();
    }

    private IEnumerator ShowEntryMessage()
    {
        yield return 0;

        if (ActiveUnit.data.unitEntryMessage.IsNullOrEmpty())
            yield break;

        yield return 0;

        ContentUI.SetupContentUI(ActiveUnit.data.unitName, null, ActiveUnit.data.unitEntryMessage, "Start Tutorial (Recommended)", "Skip Tutorial");
        ContentUI.ButtonEvents[0].AddListener(()=>
        {
            GameplayUIManager.ShowGameUI();
            StartQuest(ActiveUnit.data.questIds[0]);
            TutorialSequences.StartNewGameTutorialSequence();
        });
        ContentUI.ButtonEvents[1].AddListener(() =>
        {
            GameplayUIManager.ShowGameUI();
            StartQuest(ActiveUnit.data.questIds[0]);
        });
        ContentUI.DefaultButtonIndex = 0;
    }

    private void StartLoadedQuest()
    {
        List<QuestInfo> questInfos = ActiveUnit.info.quests;
        string lastQuestId = null;
        int startRemovalIndex = -1;

        for (int i = 0; i < questInfos.Count; i++)
        {
            if (questInfos[i].timerStarted)
            {
                lastQuestId = questInfos[i].questID;
            }
            else if (questInfos[i].IsDone)
            {
                lastQuestId = questInfos[i].questID;
            }
            else
            {
                startRemovalIndex = i;
                break;
            }
        }

        if (startRemovalIndex >= 0)
            ResetProgressToQuestIndex(startRemovalIndex, false);

        StartQuest(lastQuestId);
    }
    #endregion

    #region General Content
    private void SetupContent()
    {
        if (CanSetupContent() == false)
            return;

        ActiveCourse = new CourseReferences(GameController.SelectedCourse);
        ActiveUnit = new UnitReferences(GameController.SelectedUnit);

        if (SetupNewGame == SetupGameType.New)
        {
            ActiveCourse.info = new CourseInfo();
            PlayerInfo.courses.Add(ActiveCourse.info);
            ActiveUnit.info = new UnitInfo();

            ActiveCourse.info.courseID = ActiveCourse.courseID;
            ActiveCourse.info.units = new List<UnitInfo>();
            ActiveCourse.info.units.Add(ActiveUnit.info);

            ActiveUnit.info.quests = new List<QuestInfo>();

            PersonasManager.Instance.CreatePersonas(ActiveUnit.info);
        }
        else if(SetupNewGame == SetupGameType.Load)
        {
            ActiveCourse.info = PlayerInfo.courses.Last();
            ActiveUnit.info = ActiveCourse.info.units.Last();
        }
        else
        {
            Debug.LogError("Wrong Setup Game Type!!");
        }
    }

    private void LoadMails()
    {
        MailsManager.LoadMails(ActiveCourse.courseID, ActiveUnit.unitID, true);
    }

    private bool CanSetupContent()
    {
        if (GameController.Instance == null)
        {
            HandleLoadingError("Cannot setup content\nGameController.Instance == null");
            return false;
        }

        if(SceneLoader.IsDevScene)
            GameController.SetupNewGameplayWithoutReload(CourseManager.Courses[0], CourseManager.Courses[0].units[0]);

        if (GameController.SelectedCourse == null)
        {
            HandleLoadingError("Cannot setup content\nGameController.SelectedCourse == null");
            return false;
        }

        if (GameController.SelectedUnit == null)
        {
            HandleLoadingError("Cannot setup content\nGameController.SelectedUnit == null");
            return false;
        }

        return true;
    }
    #endregion

    #region Quests
    private void PrepareQuests()
    {
        QuestManager.LoadQuestsAndTasks(ActiveCourse.courseID, ActiveUnit.unitID, true);

        for (int i = 0; i < QuestManager.Quests.Count; i++)
        {
            Quest quest = QuestManager.Quests[i];
            QuestReferences newReferences = new QuestReferences(quest);
            newReferences.sticker = StickerManager.Instance.SpawnSticker(newReferences);
            questReferences.Add(newReferences);
            PrepareTasks(newReferences);
        }
    }

    private void StartNextQuest()
    {
        string nextQuestID = string.Empty;

        for (int i = 0; i < ActiveUnit.data.questIds.Count; i++)
        {
            if (ActiveUnit.info.quests.FirstOrDefault(obj => obj.questID == ActiveUnit.data.questIds[i]) == null)
            {
                StartQuest(ActiveUnit.data.questIds[i]);
                return;
            }
        }

        AssistantUI.Instance.SpawnInfoNotification("<b>Congratulations!</b>\nThat was the last quest!");
    }

    private void StartQuest(string questID)
    {
        QuestReferences questRef = GetQuestReferencesByID(questID);
        Quest questData = QuestManager.GetQuestByID(questID);
        QuestInfo newQuestInfo = null;
        string taskToStart = string.Empty;

        if (questData == null)
            throw new NullReferenceException("Quest Data cannot be null");

        List<QuestInfo> startedQuests = ActiveUnit.info.quests.Where(x => x.questID == questData.questID).ToList();

        if (startedQuests.Count > 1)
        {
            Debug.LogError("More than one started quests with the same ID!");
            ActiveUnit.info.quests.RemoveAll(q => q.questID == questData.questID);
            startedQuests = null;
        }

        //start as new
        if (startedQuests.IsNullOrEmpty())
        {
            newQuestInfo = new QuestInfo(questData.questID);
            questRef.info = newQuestInfo;
            questRef.info.persona = PersonasManager.Instance.CurrentPersonaIndex;
            ActiveUnit.info.quests.Add(newQuestInfo);

            AddTaskInfoToQuest(questRef);

            taskToStart = questRef.info.tasks[0].taskID;
        }
        //load if already started
        else
        {
            newQuestInfo = startedQuests[0];
            questRef.info = newQuestInfo;

            taskToStart = questRef.info.tasks[0].taskID;

            for (int i = 1; i < questRef.info.tasks.Count; i++)
            {
                if (questRef.info.tasks[i].IsDone)
                    taskToStart = questRef.info.tasks[i].taskID;
                else if (questRef.info.tasks[i].timerStarted)
                    taskToStart = questRef.info.tasks[i].taskID;
                else
                    break;
            }
        }

        ActiveQuest = questRef;
        ActiveQuest.info.timerStarted = true;
        PlayerInfo.currentMission = ActiveQuest.data.GetQuestName();

        TaskReferences taskRef = GetTaskReferencesByID(taskToStart);

        StartTask(questRef, taskRef);

        StickerManager.Instance.UpdateAllQuestStickers();
        FlyToQuest(ActiveQuest);
        OnProgressUpdate.Invoke();
    }

    public void StartTask(QuestReferences questRef, TaskReferences taskRef)
    {
        TaskBase taskData = taskRef.data;
        TaskInfo taskInfo = taskRef.info;

        ActiveTask = taskRef;

        if (taskInfo.isDone == false)
            taskInfo.timerStarted = true;

        questRef.window = GameUI.Instance.SpawnQuestWindow(questRef, taskRef);

        if (questRef.info.IsDone == false)
            questRef.info.timerStarted = true;

        MailsManager.Instance.RecieveMails(ActiveTask.data.mailsOnStart);
        OnProgressUpdate.Invoke();
    }

    private void UpdateActiveQuest()
    {
        if (ActiveQuest == null)
            return;

        if (ActiveQuest.info.timerStarted == false)
            return;

        if (ActiveQuest.info.IsDone == true)
            return;

        ActiveQuest.info.questTimer += Time.deltaTime / 60f;
    }
    #endregion

    #region Tasks
    private void PrepareTasks(QuestReferences questRef)
    {
        Quest questData = questRef.data;

        for (int i = 0; i < questData.taskIDs.Count; i++)
        {
            TaskBase task = QuestManager.GetTaskByID(questData.taskIDs[i]);
            TaskReferences newTaskRef = new TaskReferences(task, questRef);
            taskReferences.Add(newTaskRef);
            questRef.tasks.Add(newTaskRef);
        }
    }

    private void AddTaskInfoToQuest(QuestReferences questRef)
    {
        Quest questData = questRef.data;
        QuestInfo questInfo = questRef.info;

        for (int i = 0; i < questData.taskIDs.Count; i++)
        {
            TaskReferences taskRef = GetTaskReferencesByID(questData.taskIDs[i]);
            TaskInfo newTaskInfo = null;
            
            if(SetupNewGame == SetupGameType.Load)
                newTaskInfo = questInfo.tasks.Where(x => x.taskID == questData.taskIDs[i]).FirstOrDefault();

            if(newTaskInfo == null)
            { 
                newTaskInfo = new TaskInfo(questData.taskIDs[i]); 
                
                if(SetupNewGame == SetupGameType.Load)
                    Debug.LogError("Couldn't find existing TaskInfo for non-new game setup!");
            }

            if(SetupNewGame != SetupGameType.Load)
                questRef.info.tasks.Add(newTaskInfo);

            taskRef.info = newTaskInfo;
            taskRef.info.persona = PersonasManager.Instance.CurrentPersonaIndex;
        }
    }

    private void AssignLoadedElementsToReferences()
    {
        for (int iQ = 0; iQ < ActiveUnit.info.quests.Count; iQ++)
        {
            QuestInfo tempQI = ActiveUnit.info.quests[iQ];
            QuestReferences questRef = GetQuestReferencesByID(tempQI.questID);
            questRef.info = tempQI;

            for (int iT = 0; iT < tempQI.tasks.Count; iT++)
            {
                TaskInfo tempTI = tempQI.tasks[iT];
                TaskReferences taskRef = GetTaskReferencesByID(tempTI.taskID);
                taskRef.info = tempTI;
            }
        }
    }

    private void StartNextTask()
    {
        if (ActiveQuest.info.IsDone)
        {
            CompleteActiveQuest();
            return;
        }

        string nextTaskID = string.Empty;

        for (int i = 0; i < ActiveQuest.info.tasks.Count; i++)
        {
            if (ActiveQuest.info.tasks[i].IsDone)
                continue;

            nextTaskID = ActiveQuest.info.tasks[i].taskID;
            break;
        }

        if(nextTaskID.IsNullOrEmpty())
        {
            Debug.LogError("No next task found to start!");
            return;
        }

        TaskReferences taskRef = GetTaskReferencesByID(nextTaskID);
        StartTask(ActiveQuest, taskRef);
    }

    
    #endregion

    private void HandleLoadingError(string message)
    {
        Debug.LogError(message);
        SceneLoader.LoadSceneAsync(SceneLoader.MAIN_MENU_SCENE_INDEX);
    }

    public enum SetupGameType
    {
        None,
        New,
        Load
    }

    private enum NextQuestAction
    {
        allDone,
        nextQuest,
        nextTask,
        showProgressUI,
        error
    }
}
