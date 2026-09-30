using ChartAndGraph;
using GIGA.AutoRadialLayout;
using Michsky.UI.Beam;
using RuntimeInspectorNamespace;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUI : UI_Screen
{
    public static ProgressUI Instance => GameplayUIManager.Instance.ProgressUI;

    private const string PersonaLabelTemplate = "Current Persona: <b>{0}</b>";
    private const string ReputationLabelTemplate = "Reputation: <b>{0}</b>";
    private const string EthicalLabelTemplate = "Ethical Standing: <b>{0}</b>";//Ethical / Questionable / Malicious
    private const string AffiliationLabelTemplate = "Affiliation: <b>{0}</b>";//Freelancer / Corporate Security / Cybercrime Group
    private const string TitleLabelTemplate = "Title: <b>{0}</b>";//Novice / Security Analyst / Penetration Tester / Ethical Hacker / Elite Hacker

    [Header("Persona Stats")]
    [SerializeField] private RadarChart radar;
    [SerializeField] private Image reputationBar;
    [SerializeField] private Gradient reputationGradient;
    [Space]
    [SerializeField] private TextMeshProUGUI personaTMP;
    [SerializeField] private TextMeshProUGUI reputationTMP;
    [SerializeField] private TextMeshProUGUI ethicalTMP;
    [SerializeField] private TextMeshProUGUI affiliationTMP;
    [SerializeField] private TextMeshProUGUI titleTMP;


    [Header("Unit Stats")]

    [Header("Quest Map")]
    [SerializeField] private RectTransform mapObject;
    [SerializeField] private GameObject mapQuestButton;


    [Header("Quest Stats")]

    [SerializeField, Range(-100, 100), OnValueChanged("TestReputationBar")]
    [FoldoutGroup("Testing Reputation Bar")] private float testReputation = 47;

    private void TestReputationBar()
    {
        testReputation = 47;
        personaTMP.text = string.Format(PersonaLabelTemplate, PersonasManager.Instance.CurrentPersonaName);

        float fillAmount = (testReputation + 100) / 200;
        reputationBar.fillAmount = fillAmount;
        reputationBar.color = reputationGradient.Evaluate(fillAmount);
        reputationTMP.text = string.Format(ReputationLabelTemplate, (int)testReputation);

        string ethicalType = "Ethical";

        if (testReputation < -20)
            ethicalType = "Malicious";
        else if (testReputation < 40)
            ethicalType = "Questionable";


        ethicalTMP.text = string.Format(EthicalLabelTemplate, ethicalType);
        affiliationTMP.text = string.Format(AffiliationLabelTemplate, "Freelancer");
        titleTMP.text = string.Format(TitleLabelTemplate, "Novice");
    }

    private void Update()
    {
        //testReputation = Mathf.PingPong(Time.timeSinceLevelLoad, 2) * 100 - 100;
        TestReputationBar();
    }

    private List<QuestNode_Visuals> questNodes = new List<QuestNode_Visuals>();

    private List<string> QuestIDs => PlayerController.Instance.ActiveUnit.data.questIds;
    public QuestReferences VisibleQuest { get; set; } = new QuestReferences();


    public override void Back()
    {
        GameplayUIManager.ShowGameUI();
        base.Back();
    }

    public override void ShowScreen()
    {
        VisibleQuest.info = PlayerController.Instance.ActiveUnit.info.quests.LastOrDefault();
        VisibleQuest.data = QuestManager.GetQuestByID(VisibleQuest.info.questID);

        base.ShowScreen();
        UpdatePersonaStats();
        ShowQuestDetails();
        //UpdateNodeVisuals();
    }

    public void ShowQuestsUI()
    {
        GameplayUIManager.ShowQuestsUI();
    }

    public void ShowQuestDetails(QuestReferences questRef)
    {
        VisibleQuest.data = questRef.data;
        VisibleQuest.info = questRef.info;
        ShowQuestDetails();
    }

    public void ShowQuestDetails(QuestNode_Visuals questNode_Visuals)
    {
        VisibleQuest.data = QuestManager.GetQuestByID(questNode_Visuals.QuestID);
        VisibleQuest.info = PlayerController.Instance.GetQuestReferencesByID(questNode_Visuals.QuestID).info;
        ShowQuestDetails();
    }

    public void ShowQuestDetails()
    {
        HighlightSelectedNodeVisuals();

        //titleTMP.text = VisibleQuest.data.GetQuestName(VisibleQuest.info.persona);
        //descriptionTMP.text = VisibleQuest.data.GetQuestDescription();

        //if (titleTMP.text.Length > 25)
        //    titleTMP.text = titleTMP.text.Substring(0, 22) + "...";

        //if (descriptionTMP.text.Length > 500)
        //    descriptionTMP.text = descriptionTMP.text.Substring(0, 500) + "...";

        //layoutRebuilder.FixLayout();

        //if (VisibleQuest.info == null)
        //{
        //    statusTMP.text = "LOCKED";
        //    return;
        //}

        //if (VisibleQuest.info.IsDone)
        //    statusTMP.text = "COMPLETED";
        //else if (VisibleQuest.info.timerStarted)
        //    statusTMP.text = "IN PROGRESS";
        //else
        //    statusTMP.text = "READY TO START";
    }

    private void Start()
    {
        radar.DataSource.ClearGroups();

        for (int i = 0; i < PersonasManager.Instance.Personas.Count; i++)
        {
            Persona persona = PersonasManager.Instance.Personas[i];
            radar.DataSource.AddGroup(persona.personaID);
        }

        radar.DataSource.MinValue = -10;
        radar.DataSource.MaxValue = 100;
    }

    private void UpdatePersonaStats()
    {
        for (int i = 0; i < PersonasManager.Instance.PersonaInfos.Count; i++)
        {
            PersonaInfo personaInfo = PersonasManager.Instance.PersonaInfos[i];
            radar.DataSource.SetValue("Player 1", personaInfo.personaID, personaInfo.personaValue);
        }
    }


    private void HighlightSelectedNodeVisuals()
    {
        //if (VisibleQuest == null)
        //    return;

        //if(VisibleQuest.data == null)
        //    return;

        //for (int i = 0; i < questNodes.Count; i++)
        //{
        //    questNodes[i].ToggleSelection(questNodes[i].QuestID == VisibleQuest.data.questID);
        //}
    }
}
