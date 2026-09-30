using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Course : IValidateContent
{
    [RuntimeInspectorNamespace.LockedField]
    public string courseID;
    public bool placeholder = false;
    [Range(0, 100)]
    public int order = 0;
    public string courseName;
    [TextArea] public string courseDescription;
    public List<Unit> units = new List<Unit>();

    public Sprite GetPicture()
    {
        if (CourseManager.Pictures.TryGetValue(courseID, out Texture2D picture))
        {
            return Sprite.Create(picture,
                new Rect(0, 0, picture.width, picture.height),
                new Vector2(picture.width / 2, picture.height / 2));
        }

        return null;
    }

    public override string ToString()
    {
        return $"{courseName}\n" +
        $"{courseID}\n" +
            $"{courseDescription}";
    }

    public string ToFormattedString()
    {
        return $"<b>{courseName}</b>\n" +
            $"<size=40%><i>{courseID}</i></size>\n" +
            $"<size=55%>{courseDescription}</size>";
    }

    public bool ValidateContent(out string validationMessage)
    {
        List<string> issues = new List<string>();
        issues.Add("Issues:");

        if (courseName.IsNullOrEmpty())
            issues.Add("Course name is empty.");

        if (courseDescription.IsNullOrEmpty())
            issues.Add("Course description is empty.");

        if (units.IsNullOrEmpty())
            issues.Add("Course has no units.");

        validationMessage = string.Join("\n - ", issues);

        return issues.Count <= 1;
    }
}
