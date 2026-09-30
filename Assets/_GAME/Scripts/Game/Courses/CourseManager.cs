using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CourseManager : GameSystemComponent
{
    public static CourseManager Instance { get; private set; }

    private static List<Course> courses = new List<Course>();
    private static Dictionary<string, Texture2D> pictures = new Dictionary<string, Texture2D>();

    [ShowInInspector]
    public static List<Course> Courses => courses;
    public static Dictionary<string, Texture2D> Pictures => pictures;

    public static Course GetCourseByID(string courseID)
    {
        if (Courses.IsNullOrEmpty())
            return null;

        for (int i = 0; i < Courses.Count; i++)
        {
            if (Courses[i].courseID == courseID)
                return Courses[i];
        }

        return null;
    }

    public static Unit GetUnitByID(Course course, string unitID)
    {
        if (course == null)
            return null;

        if (course.units.IsNullOrEmpty())
            return null;

        for (int i = 0; i < course.units.Count; i++)
        {
            if (course.units[i].unitID == unitID)
                return course.units[i];
        }
     
        return null;
    }

    private void Awake()
    {
        Instance = this;
        LoadCourses();
        LoadPictures();
    }

    private static void LoadCourses()
    {
        courses = ContentLoader.ReturnListOfType<Course>(ContentConstValues.FOLDER_COURSES, ContentConstValues.EXTENSION_COURSE);
        courses = courses.OrderBy(x => x.order).ToList();
    }

    private static void LoadPictures()
    {
        pictures = ContentLoader.ReturnPictures(ContentConstValues.FOLDER_COURSES);
    }
}
