using Godot;
using Godot.Collections;
using Iode.Models;
using Iode.UI;
using Iode.UI.Dialogues;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Iode.Core
{
    public sealed partial class ProjectManager : Control
    {
        [Export]
        public PackedScene ProjectItem { get; set; }

        [Export]
        public required Button CreateButton, ImportButton, ScanButton;

        [Export]
        public VBoxContainer ProjectList { get; set; }

        public static ProjectManager Singleton { get; private set; } = null;

        public override void _Ready()
        {
            if (Singleton != null)
            {
                throw new InvalidOperationException("Singleton for ProjectManager already exists.");
            }

            Singleton = this;

            CreateButton.Pressed += () => ProjectDialog.Singleton.Popup();

            // Load projects
            foreach (ProjectMetadata projectMetadata in GetProjects())
            {
                ProjectList.AddChild(ProjectItem.Instantiate<ProjectItem>().ApplyProjectMetadata(projectMetadata));
            }
        }

        public void MakeProject(ProjectMetadata projectMetadata)
        {
            ProjectList.AddChild(ProjectItem.Instantiate<ProjectItem>().ApplyProjectMetadata(projectMetadata));

            Error dirCheck = DirAccess.MakeDirRecursiveAbsolute($"user://user_projects/{projectMetadata.Name.Replace(" ", "-")}");
            if (dirCheck != Error.Ok)
            {
                PopupManager.Singleton.AlertPopup("Failed to create the project directory.");
                return;
            }

            string jsonPath = "user://projects.json";

            Dictionary projectsDict = [];

            if (FileAccess.FileExists(jsonPath))
            {
                using var existingFile = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
                var content = existingFile.GetAsText();

                var parsed = Json.ParseString(content);
                if (parsed.Obj is Dictionary d)
                    projectsDict = d;
            }

            Dictionary projectData = new()
            {
                { "Tags", true },
                { "CreatedAt", projectMetadata.CreatedAt.ToString() },
            };

            projectsDict[projectMetadata.Name] = projectData;

            using var file = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Write);
            file.StoreString(Json.Stringify(projectsDict, indent: "\t"));
            file.Close();
        }


        private static List<ProjectMetadata> GetProjects()
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


        private static string GetProjectsFile()
        {
            string path = "user://projects.json";

            if (!FileAccess.FileExists(path))
            {
                using var createFile = FileAccess.Open(path, FileAccess.ModeFlags.Write);
                createFile.StoreString("{}");
                createFile.Close();
                return "{}";
            }

            using var fileAccess = FileAccess.Open(path, FileAccess.ModeFlags.Read);
            string faContents = fileAccess.GetAsText().Trim();

            if (string.IsNullOrWhiteSpace(faContents))
            {
                fileAccess.Close();
                using var fixFile = FileAccess.Open(path, FileAccess.ModeFlags.Write);
                fixFile.StoreString("{}");
                fixFile.Close();
                return "{}";
            }

            return faContents;
        }

    }
}