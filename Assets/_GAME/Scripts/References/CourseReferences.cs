using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseReferences
{
    public string courseID;
    public Course data;
    public CourseInfo info;

    public CourseReferences() { }

    public CourseReferences(Course course)
    {
        this.courseID = course.courseID;
        this.data = course;
    }
}
