using System;
using static Command;
using System.Collections.Generic;


[Serializable]
[RuntimeInspectorNamespace.ExpandArray]
public class Link : IValidateContent
{
    public string linkID;
    public string linkName;
    public string linkText;
    public LinkAction linkAction;

    [Serializable]
    public enum LinkAction
    {
        None = 0
    }

    public bool ValidateContent(out string validationMessage)
    {
        List<string> issues = new List<string>();
        issues.Add($"<b>{linkID}</b>");

        if (linkID.IsNullOrEmpty())
            issues.Add("Link ID is empty.");

        if (linkName.IsNullOrEmpty())
            issues.Add("Link name is empty.");

        if (linkText.IsNullOrEmpty())
            issues.Add("Link text is empty.");

        validationMessage = string.Join("\n - ", issues);

        return issues.Count <= 1;
    }
}
