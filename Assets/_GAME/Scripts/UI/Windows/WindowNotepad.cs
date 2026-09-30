using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowNotepad: WindowBase
{
    [Space]
    [SerializeField] private TMP_InputField notepadInput;
    [Space]
    [SerializeField] private GameObject noteButtonPrefab;
    [SerializeField] private Transform noteButtonsContent;

    private NoteInfo noteInfo;

    public void NewNote()
    {
        NewNote(string.Empty);
    }

    public NoteInfo NewNote(string noteText)
    {
        noteInfo = new NoteInfo();
        noteInfo.noteID = "note_" + System.DateTime.Now.ToString();
        noteInfo.noteText = noteText;
        PlayerController.PlayerInfo.notes.Add(noteInfo);
        notepadInput.SetTextWithoutNotify(noteText);
        SelectInputField();
        SaveNote();
        return noteInfo;
    }

    public void SaveNote()
    {
        if (noteInfo == null)
            return;

        noteInfo.noteText = notepadInput.text;
        UpdateNoteButtons();
    }

    public void LoadNote(int noteIndex)
    {
        LoadNote(PlayerController.PlayerInfo.notes[noteIndex]);
        SelectInputField();
    }

    public void LoadNote(NoteInfo noteToLoad) 
    {
        noteInfo = noteToLoad;
        notepadInput.SetTextWithoutNotify(noteInfo.noteText);
    }

    public void UpdateNoteButtons()
    {
        for (int i = noteButtonsContent.childCount - 1; i >= PlayerController.PlayerInfo.notes.Count; i--)
        {
            noteButtonsContent.GetChild(i).gameObject.Destroy();
        }

        int existingButtonsCount = noteButtonsContent.childCount;

        for (int i = 0; i < PlayerController.PlayerInfo.notes.Count; i++)
        {
            ButtonExtended button;

            if (i < existingButtonsCount)
            {
                button = noteButtonsContent.GetChild(i).GetComponent<ButtonExtended>();
            }
            else
            {
                button = Instantiate(noteButtonPrefab, noteButtonsContent).GetComponent<ButtonExtended>();
                button.onClick.AddListener(() =>
                {
                    LoadNote(button.transform.GetSiblingIndex());
                });
            }

            button.TextTMP.text = PlayerController.PlayerInfo.notes[i].NoteTitle;
        }
    }

    public void SetupNotepad()
    {
        UpdateNoteButtons();

        if (PlayerController.PlayerInfo.notes.IsNullOrEmpty())
        {
            NewNote();
            SaveNote();
        }
        else
        {
            LoadNote(PlayerController.PlayerInfo.notes[0]);
        }

        SelectInputField();
    }

    public override void ClickClose()
    {
        base.ClickClose();
    }

    private void Start()
    {
        WindowFlexibilityComponent.OnActiveEvent.AddListener(SelectInputField);
    }

    private void SelectInputField()
    {
        notepadInput.Select();
        notepadInput.ActivateInputField();
        //StartCoroutine(MoveTextEnd_NextFrame());
    }

    private IEnumerator MoveTextEnd_NextFrame()
    {
        yield return 0;
        notepadInput.MoveTextEnd(false);
    }
}
