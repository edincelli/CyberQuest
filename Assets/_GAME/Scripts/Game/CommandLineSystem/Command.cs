using RuntimeInspectorNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Command : IValidateContent
{
    [LockedField] public string command;
    public bool isTool;
    [TextArea] public string output;
    [TextArea] public string errorSub;

    [ExpandArray] public List<Subcommand> subcommands = new List<Subcommand>();

    public override string ToString()
    {
        return $"{command}\n" +
            $"{string.Join(", ", subcommands.Select(x => x.subcommand).ToArray())}";
    }

    [Serializable]
    [ExpandArray]
    public class Subcommand
    {
        public string subcommand;
        [TextArea] public string output; 
    }

    public bool ValidateContent(out string validationMessage)
    {
        List<string> issues = new List<string>();
        issues.Add("Issues:");

        if (output.IsNullOrEmpty())
            issues.Add("Command output is empty.");

        if (errorSub.IsNullOrEmpty())
            issues.Add("Error subcommand is empty.");

        if (subcommands.IsNullOrEmpty())
            issues.Add("<color=#ffb300>(possible issue) Command has no subcommands.</color>");

        validationMessage = string.Join("\n - ", issues);

        return issues.Count <= 1;
    }

    [RuntimeInspectorButton("Add Subcommand", false, ButtonVisibility.InitializedObjects)]
    public void AddLink()
    {
        subcommands.Add(new Subcommand());
    }
}
