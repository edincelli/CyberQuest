using Minigames;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskMinigameVariant : TaskBaseVariant, IValidateContent
{
    [JsonProperty]
    public MinigamesController.MinigameType minigameType = MinigamesController.MinigameType.None;
    public string minigameHeaderText;

    public override bool ValidateContent(out string validationMessage)
    {
        List<string> issues = new List<string>();

        base.ValidateContent(out string baseIssues);

        if (baseIssues.IsNullOrEmpty() == false)
            issues.Add(baseIssues);

        if(minigameType == MinigamesController.MinigameType.None)
            issues.Add("Minigame type is not set.");

        if(minigameHeaderText.IsNullOrEmpty())
            issues.Add("Minigame header text is empty.");

        validationMessage = string.Join("\n - ", issues);

        return issues.Count == 0;
    }
}
