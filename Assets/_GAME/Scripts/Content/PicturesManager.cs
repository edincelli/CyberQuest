using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class PicturesManager : GameSystemComponent
{
    public const string QUEST_PICTURES_PATH = "Grpahics/QuestIcons";
    public const string CHARACTERS_PICTURES_PATH = "Grpahics/Characters";
    public const string POIS_PICTURES_PATH = "Grpahics/POIsIcons";
    public const string PERSONA_PICTURES_PATH = "Grpahics/PersonaIcons";

    public static PicturesManager Instance { get; private set; }

    [Header("Defaults")]
    [SerializeField, SpritePreview] private Sprite defaultPicture;
    [SerializeField, SpritePreview] private Sprite defaultQuestPicture;
    [SerializeField, SpritePreview] private Sprite defaultCharacterPicture;
    [SerializeField, SpritePreview] private Sprite defaultPOIPicture;
    [SerializeField, SpritePreview] private Sprite defaultPersonaPicture;
    [SerializeField, SpritePreview] private Sprite defaultAppIcon;

    [Header("Lists")]

    [ShowInInspector]
    public static List<PictureReferences> QuestPictures { get; private set; } = new List<PictureReferences>();
    [ShowInInspector]
    public static List<PictureReferences> CharacterPictures { get; private set; } = new List<PictureReferences>();
    [ShowInInspector]
    public static List<PictureReferences> POIsPictures { get; private set; } = new List<PictureReferences>();
    [ShowInInspector]
    public static List<PictureReferences> PersonaPictures { get; private set; } = new List<PictureReferences>();

    public static Sprite DefaultPicture => Instance.defaultPicture;
    public static Sprite DefaultQuestPicture => Instance.defaultQuestPicture;
    public static Sprite DefaultCharacterPicture => Instance.defaultCharacterPicture;
    public static Sprite DefaultPOIPicture => Instance.defaultPOIPicture;
    public static Sprite DefaultPersonaPicture => Instance.defaultPersonaPicture;
    public static Sprite DefaultAppIcon => Instance.defaultAppIcon;

    public static PictureReferences GetPictureReferences(string pictureID, PictureType pictureType)
    {
        List<PictureReferences> selectedList;

        switch (pictureType)
        {
            case PictureType.Quest:
                selectedList = QuestPictures;
                break;
            case PictureType.Character:
                selectedList = CharacterPictures;
                break;
            case PictureType.POI:
                selectedList = POIsPictures;
                break;
            case PictureType.Persona:
                selectedList = PersonaPictures;
                break;
            default:
                selectedList = new List<PictureReferences>();
                break;
        }

        for (int i = 0; i < selectedList.Count; i++)
        {
            if (selectedList[i].pictureID == pictureID)
                return selectedList[i];
        }

        return null;
    }

    public static Sprite GetPicture(string pictureID, PictureType pictureType)
    {
        PictureReferences picRef = GetPictureReferences(pictureID, pictureType);

        if (picRef != null)
            return picRef.Picture;

        switch (pictureType)
        {
            case PictureType.Quest:
                return Instance.defaultQuestPicture;

            case PictureType.Character:
                return Instance.defaultCharacterPicture;

            case PictureType.POI:
                return Instance.defaultPOIPicture;

            case PictureType.Persona:
                return Instance.defaultPersonaPicture;

            default:
                return Instance.defaultPicture;
        }
    }

    public static void LoadPictures(List<PictureReferences> selectedList, string path)
    {
        selectedList.Clear();

        Dictionary<string, Texture2D> questPNGs = ContentLoader.ReturnPictures(path);

        for (int i = 0; i < questPNGs.Count; i++)
        {
            KeyValuePair<string, Texture2D> png = questPNGs.ElementAt(i);

            PictureReferences picture = new PictureReferences(png.Key, png.Value);
            selectedList.Add(picture);
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.Destroy();
            return;
        }

        Instance = this;
        gameObject.DontDestroyOnLoadImproved();

        LoadAllPictures();
    }

    private void LoadAllPictures()
    {
        LoadPictures(QuestPictures, QUEST_PICTURES_PATH);
        LoadPictures(CharacterPictures, CHARACTERS_PICTURES_PATH);
        LoadPictures(POIsPictures, POIS_PICTURES_PATH);
        LoadPictures(PersonaPictures, PERSONA_PICTURES_PATH);
    }

    [Serializable]
    public enum PictureType
    {
        None,
        Quest,
        Character,
        POI,
        Persona
    }
}
