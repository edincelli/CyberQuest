using System;
using System.Collections.Generic;

[Serializable]
public class PlayerInfo
{
    public string saveId = "game";
    public string userName = "Alex";
    public string unitName = "Unit001";
    public string currentMission;
    public string gameVersion;
    public int saveFileVersion = 0;

    public string courseID;
    public string unitID;

    public float playTime = 0; //in minutes
    public List<CourseInfo> courses = new List<CourseInfo>();

    public List<NoteInfo> notes = new List<NoteInfo>();
    public List<string> watchedVideos = new List<string>();
    public List<string> skills = new List<string>();

    public PlayerInfo(string saveId, string courseID, string unitID)
    {
        this.saveId = saveId;
        this.courseID = courseID;
        this.unitID = unitID;
    }
}
