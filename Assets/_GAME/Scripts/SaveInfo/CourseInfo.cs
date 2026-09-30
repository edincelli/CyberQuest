using System;
using System.Collections.Generic;

[Serializable]
public class CourseInfo
{
    public string courseID;
    public bool isDone = false;

    public List<UnitInfo> units = new List<UnitInfo>();
}
