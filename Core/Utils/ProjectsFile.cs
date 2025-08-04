using Godot;
using Godot.Collections;
using Iode.Models;
using System.Collections.Generic;
using System.Data.Common;

namespace Iode.Core.Utils
{
    public static class ProjectsFile
    {
        private const string JsonEmpty = "{}";
        private const string ProjectsFilePath = "user://projects.json";

        /// <summary>
        /// We cache the project files content so we do not have many I/O operations.
        /// </summary>
        private static string CachedProjectFile = string.Empty;

        public static string GetProjectsFile()
        {
            if (CachedProjectFile != string.Empty)
            {
                return CachedProjectFile;
            }

            CachedProjectFile = JsonEmpty;

            if (!FileAccess.FileExists(ProjectsFilePath))
            {
                using var createFile = FileAccess.Open(ProjectsFilePath, FileAccess.ModeFlags.Write);
                createFile.StoreString(JsonEmpty);
                createFile.Close();

                return JsonEmpty;
            }

            string faContents = FileAccess.GetFileAsString(ProjectsFilePath);

            if (string.IsNullOrWhiteSpace(faContents))
            {
                using var fixFile = FileAccess.Open(ProjectsFilePath, FileAccess.ModeFlags.Write);
                fixFile.StoreString(JsonEmpty);
                fixFile.Close();

                return JsonEmpty;
            }

            CachedProjectFile = faContents;

            return faContents;
        }

        public static void RemoveProject(string projectName)
        {
            var parsed = Json.ParseString(CachedProjectFile);
            if (parsed.Obj is Dictionary projectsDict)
            {
                projectsDict.Remove(projectName);
                CachedProjectFile = Json.Stringify(projectsDict, "\t");
            }
        }

        public static void AddProject(string projectName, Dictionary projectAttributes)
        {
            var parsed = Json.ParseString(CachedProjectFile);
            if (parsed.Obj is Dictionary projectsDict)
            {
                projectsDict.Add(projectName, projectAttributes);
                CachedProjectFile = Json.Stringify(projectsDict, "\t");
            }
        }

        public static List<ProjectMetadata> GetProjects()
        {
            List<ProjectMetadata> projects = [];

            string projectsFile = GetProjectsFile();
            if (string.IsNullOrWhiteSpace(projectsFile))
            {
                return projects;
            }

            var parsed = Json.ParseString(projectsFile);
            if (parsed.Obj is not Dictionary dict)
            {
                PopupManager.Singleton.AlertPopup("projects.json file is corrupted.");
                return projects;
            }

            foreach (var key in dict.Keys)
            {
                if (dict[key].Obj is not Dictionary properties)
                    continue;

                ProjectMetadata metadata = new()
                {
                    Name = key.ToString()
                };

                if (properties.TryGetValue("CreatedAt", out Variant createdAt))
                {
                    metadata.CreatedAt = createdAt.AsString();
                }

                projects.Add(metadata);
            }

            return projects;
        }


        /// <summary>
        /// Flushes the cache string into the file. Should be called when:
        /// - Scene is changing
        /// - Exitting the SceneTree
        /// </summary>
        public static void FlushToDisk()
        {
            using var file = FileAccess.Open(ProjectsFilePath, FileAccess.ModeFlags.Write);
            file.StoreString(CachedProjectFile);
            file.Close();
        }
    }
}