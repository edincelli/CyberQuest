using RuntimeInspectorNamespace;
using System;
using System.Collections.Generic;


[Serializable]
public class LinkList : IValidateContent
{
    [RuntimeInspectorNamespace.ExpandArray]
    public List<Link> list = new List<Link>();

    public bool ValidateContent(out string validationMessage)
    {
        List<string> issues = new List<string>();
        issues.Add("Issues:");

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].ValidateContent(out string message) == false)
                issues.Add($"Link {i}: {message}");
        }

        validationMessage = string.Join("\n - ", issues);
        return issues.Count <= 1;
    }

    [RuntimeInspectorButton("Add Link", false, ButtonVisibility.InitializedObjects)]
    public void AddLink()
    {
        list.Add(new Link());
    }
}
