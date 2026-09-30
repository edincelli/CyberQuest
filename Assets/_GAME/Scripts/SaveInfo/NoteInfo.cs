using System;

public class NoteInfo
{
    public string noteID;
    public string noteText;

    public string NoteTitle
    {
        get
        {
            if (noteText.IsNullOrEmpty())
                return "<i>empty note</i>";

            string firstLine = noteText.Split('\n')[0];

            if(firstLine.Length > 14)
                firstLine = firstLine.Substring(0, 12) + "...";

            return firstLine;
        }
    }
}
