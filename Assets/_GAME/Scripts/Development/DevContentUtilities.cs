using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DevTools
{
    public class DevContentUtilities : MonoBehaviour
    {
        private static string CoursesPath => ContentLoader.GetContentPath(ContentConstValues.FOLDER_COURSES);
        private static string QuestsPath = ContentLoader.GetContentPath(ContentConstValues.FOLDER_QUESTS);
        private static string MailsPath = ContentLoader.GetContentPath(ContentConstValues.FOLDER_MAILS);
        private static string CommandsPath = ContentLoader.GetContentPath(ContentConstValues.FOLDER_COMMANDS);
        private static string ConfigPath = ContentLoader.GetContentPath(ContentConstValues.FOLDER_CONFIG);
        private static string POIsPath = ContentLoader.GetContentPath(ContentConstValues.FOLDER_POIS);
        private static string AppsPath = ContentLoader.GetContentPath(ContentConstValues.FOLDER_APPS);

        public static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new EnhancedIgnoreContractResolver(),
            Formatting = Formatting.Indented,
            Converters = { new StringEnumConverter() },
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        private class EnhancedIgnoreContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var allProps = base.CreateProperties(type, memberSerialization);

                return allProps.Where(p =>
                {
                    // Exclude properties marked with [DoNotSerialize]
                    if (p.AttributeProvider.GetAttributes(typeof(DoNotSerializeAttribute), true).Any())
                        return false;

                    // Include fields (they're fine)
                    if (p.PropertyType != null && p.UnderlyingName != null)
                    {
                        var member = type.GetMember(p.UnderlyingName).FirstOrDefault();
                        if (member is FieldInfo) return true;
                    }

                    // Allow read-only properties if they have [JsonProperty] attribute
                    if (!p.Writable)
                    {
                        if (p.AttributeProvider.GetAttributes(typeof(JsonPropertyAttribute), true).Any())
                            return true;
                        return false;
                    }

                    return true;
                })
                .ToList();
            }
        }

        #region Courses and Units
        public static void SaveCourse(Course course)
        {
            string courseID = course.courseID;
            string jsonText = JsonConvert.SerializeObject(course, JsonSettings);

            string directoryPath = ContentLoader.CombinePath(CoursesPath, courseID);
            string filePath = ContentLoader.CombinePath(directoryPath, $"{courseID}.{ContentConstValues.EXTENSION_COURSE}");

            PreparePath(directoryPath);
            File.WriteAllText(filePath, jsonText);
        }

        public static void DeleteCourse(Course course)
        {
            string courseID = course.courseID;
            string courseDirectoryPath = ContentLoader.CombinePath(CoursesPath, courseID);
            string questsDirectoryPath = ContentLoader.CombinePath(QuestsPath, courseID);

            PreparePath(courseDirectoryPath);
            PreparePath(questsDirectoryPath);
            Directory.Delete(courseDirectoryPath, true);
            Directory.Delete(questsDirectoryPath, true);
        }
        #endregion

        #region Quests
        public static void SaveQuest(Quest quest, string courseID, string unitID)
        {
            string questID = quest.questID;
            string jsonText = JsonConvert.SerializeObject(quest, Formatting.Indented, JsonSettings);

            string directoryPathCourse = ContentLoader.CombinePath(QuestsPath, courseID);
            string directoryPathUnit = ContentLoader.CombinePath(directoryPathCourse, unitID);
            string directoryPathQuest = ContentLoader.CombinePath(directoryPathUnit, questID);
            string filePath = ContentLoader.CombinePath(directoryPathQuest, $"{questID}.{ContentConstValues.EXTENSION_QUEST}");

            PreparePath(directoryPathCourse);
            PreparePath(directoryPathUnit);
            PreparePath(directoryPathQuest);
            File.WriteAllText(filePath, jsonText);
        }

        public static void DeleteQuest(Quest quest, string courseID, string unitID)
        {
            string questID = quest.questID;
            string directoryPath = ContentLoader.CombinePath(QuestsPath, courseID, unitID, questID);

            PreparePath(directoryPath);
            Directory.Delete(directoryPath, true);
        }
        #endregion

        #region Tasks
        public static void SaveTask(TaskBase task, string courseID, string unitID, string questID)
        {
            string taskID = task.taskID;
            string jsonText = JsonConvert.SerializeObject(task, JsonSettings);

            string directoryPathCourse = ContentLoader.CombinePath(QuestsPath, courseID);
            string directoryPathUnit = ContentLoader.CombinePath(directoryPathCourse, unitID);
            string directoryPathQuest = ContentLoader.CombinePath(directoryPathUnit, questID);
            string filePath = ContentLoader.CombinePath(directoryPathQuest, $"{taskID}.{ContentConstValues.EXTENSION_TASK}");

            PreparePath(directoryPathCourse);
            PreparePath(directoryPathUnit);
            PreparePath(directoryPathQuest);
            File.WriteAllText(filePath, jsonText);
        }

        public static void DeleteTask(TaskBase task, string courseID, string unitID, string questID)
        {
            string taskID = task.taskID;
            string directoryPath = ContentLoader.CombinePath(QuestsPath, courseID, unitID, questID);
            string filePath = ContentLoader.CombinePath(directoryPath, $"{taskID}.{ContentConstValues.EXTENSION_TASK}");

            if (Directory.Exists(directoryPath))
                File.Delete(filePath);
        }

        #endregion

        #region Mails
        public static void SaveMail(MailData mail, string courseID, string unitID)
        {
            string mailID = mail.mailID;
            string jsonText = JsonConvert.SerializeObject(mail, Formatting.Indented, JsonSettings);

            string directoryPathCourse = ContentLoader.CombinePath(MailsPath, courseID);
            string directoryPathUnit = ContentLoader.CombinePath(directoryPathCourse, unitID);
            string filePath = ContentLoader.CombinePath(directoryPathUnit, $"{mailID}.{ContentConstValues.EXTENSION_MAIL}");

            PreparePath(directoryPathCourse);
            PreparePath(directoryPathUnit);
            File.WriteAllText(filePath, jsonText);
        }

        public static void DeleteMail(MailData mail, string courseID, string unitID)
        {
            string mailID = mail.mailID;
            string directoryPath = ContentLoader.CombinePath(MailsPath, courseID, unitID);
            string filePath = ContentLoader.CombinePath(directoryPath, $"{mailID}.{ContentConstValues.EXTENSION_MAIL}");

            PreparePath(directoryPath);
            File.Delete(filePath);
        }
        #endregion

        #region Links
        public static void SaveLinks(LinkList links)
        {
            string jsonText = JsonConvert.SerializeObject(links, Formatting.Indented, JsonSettings);

            string directoryPathConfig = ContentLoader.CombinePath(ConfigPath);
            string filePath = ContentLoader.CombinePath(directoryPathConfig, $"links.{ContentConstValues.EXTENSION_LINKS}");

            PreparePath(directoryPathConfig);
            File.WriteAllText(filePath, jsonText);
        }
        #endregion

        #region POIs
        public static void SavePOIs(POIs pois)
        {
            string jsonText = JsonConvert.SerializeObject(pois, Formatting.Indented, JsonSettings);

            string directoryPathPOIs = ContentLoader.CombinePath(POIsPath);
            string filePath = ContentLoader.CombinePath(directoryPathPOIs, $"{pois.unitID}.{ContentConstValues.EXTENSION_POIS}");

            PreparePath(directoryPathPOIs);
            File.WriteAllText(filePath, jsonText);
        }
        #endregion

        #region Commands
        public static void SaveCommand(Command command)
        {
            string commandID = command.command;
            string jsonText = JsonConvert.SerializeObject(command, Formatting.Indented);

            string directoryPath = ContentLoader.CombinePath(CommandsPath);
            string filePath = ContentLoader.CombinePath(directoryPath, $"{commandID}.{ContentConstValues.EXTENSION_COMMAND}");

            PreparePath(directoryPath);
            File.WriteAllText(filePath, jsonText);
        }

        public static void DeleteCommand(Command command)
        {
            string directoryPath = ContentLoader.CombinePath(CommandsPath);
            string filePath = ContentLoader.CombinePath(directoryPath, $"{command.command}.{ContentConstValues.EXTENSION_COMMAND}");

            PreparePath(directoryPath);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        #endregion

        #region Apps
        public static void SaveApp(AppCore appCore)
        {
            string appCoreID = appCore.appID;
            string jsonText = JsonConvert.SerializeObject(appCore, JsonSettings);

            // TODO: Implement saving app content.
            string directoryPath = ContentLoader.CombinePath(AppsPath, appCoreID);
            string filePath = ContentLoader.CombinePath(directoryPath, $"{appCoreID}.{ContentConstValues.EXTENSION_APP}");

            PreparePath(directoryPath);
            File.WriteAllText(filePath, jsonText);
        }

        public static void DeleteApp(AppCore appCore)
        {
            string appID = appCore.appID;
            string appDirectoryPath = ContentLoader.CombinePath(AppsPath, appID);

            PreparePath(appDirectoryPath);
            Directory.Delete(appDirectoryPath, true);
        }

        public static void SaveAppScenario(AppScenario appScenario)
        {
            string appCoreID = appScenario.appID;
            string appScenarioID = appScenario.scenarioID;
            string jsonText = JsonConvert.SerializeObject(appScenario, JsonSettings);

            string directoryPath = ContentLoader.CombinePath(AppsPath, appCoreID, appScenarioID);
            string filePath = ContentLoader.CombinePath(directoryPath, $"{appScenarioID}.{ContentConstValues.EXTENSION_APPSCENARIO}");

            PreparePath(directoryPath);
            File.WriteAllText(filePath, jsonText);
        }

        public static void DeleteAppScenario(AppScenario appScenario)
        {
            string appCoreID = appScenario.appID;
            string appScenarioID = appScenario.scenarioID;
            string directoryPath = ContentLoader.CombinePath(AppsPath, appCoreID, appScenarioID);

            PreparePath(directoryPath);
            Directory.Delete(directoryPath, true);
        }
        #endregion

        #region Utilities

        private static void PreparePath(string path)
        {
            if (Directory.Exists(path) == false)
                Directory.CreateDirectory(path);
        }

        #endregion
    }
}
