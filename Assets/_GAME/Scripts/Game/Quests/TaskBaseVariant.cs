using RuntimeInspectorNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Windows;

[Serializable, ExpandArray]
public abstract class TaskBaseVariant : IValidateContent
{
    [LockedField] public string persona;
    [TextArea] public string taskDescription;
    [TextArea] public string taskQuestion;
    [TextArea] public string taskCompleteText;

    [DoNotSerialize] public List<string> Links
    {
        get
        {
            List<string> results = new List<string>();

            if (string.IsNullOrEmpty(taskDescription))
                return results;

            var matches = Regex.Matches(taskDescription, @"<link=""(.*?)"">");

            foreach (Match match in matches)
            {
                results.Add(match.Groups[1].Value);
            }

            return results;
        }
    }


    public virtual bool ValidateContent(out string validationMessage)
    {
        List<string> issues = new List<string>();
        issues.Add($"{persona} Variant:");

        validationMessage = string.Join("\n - ", issues);

        return issues.Count <= 1;
    }
}
