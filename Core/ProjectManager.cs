using Godot;
using Godot.Collections;
using Iode.Core.Utils;
using Iode.Models;
using Iode.UI;
using Iode.UI.Dialogues;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Iode.Core
{
    public sealed partial class ProjectManager : Control
    {
        [Export]
        public PackedScene ProjectItem { get; set; }

        [Export]
        public VBoxContainer ProjectList { get; set; }

        [Export]
        public required Button CreateButton, ImportButton, ScanButton, EditButton, RunButton, RenameButton, DeleteButton;

        public static ProjectManager Singleton { get; private set; } = null;

        private readonly System.Collections.Generic.Dictionary<string, ProjectItem> BuiltProjectItems = [];

        private ProjectItem _selectedProjectItem = null;
        public ProjectItem SelectedProjectItem
        {
            get => _selectedProjectItem;
            set
            {
                if (_selectedProjectItem != null)
                {
                    _selectedProjectItem.ThemeTypeVariation = "IodeSecondary";
                }
                _selectedProjectItem = value;
                UpdateButtonStates();
            }
        }

        public override void _Ready()
        {
            if (Singleton != null)
            {
                throw new InvalidOperationException("Singleton for ProjectManager already exists.");
            }

            Singleton = this;

            // Establish signals
            CreateButton.Pressed += () => ProjectDialog.Singleton.Popup();

            DeleteButton.Pressed += RemoveProject;

            // Load projects
            List<ProjectMetadata> projects = ProjectsFile.GetProjects();
            projects.Reverse();
            foreach (ProjectMetadata projectMetadata in projects)
            {
                ProjectItem builtProjectItem = ProjectItem.Instantiate<ProjectItem>().ApplyProjectMetadata(projectMetadata);
                BuiltProjectItems[projectMetadata.Name] = builtProjectItem;
                ProjectList.AddChild(builtProjectItem);
            }
        }

        public override void _ExitTree()
        {
            ProjectsFile.FlushToDisk();
        }

        public void MakeProject(ProjectMetadata projectMetadata)
        {
            ProjectItem projectItem = ProjectItem.Instantiate<ProjectItem>().ApplyProjectMetadata(projectMetadata);
            BuiltProjectItems[projectMetadata.Name] = projectItem;

            ProjectList.AddChild(projectItem);
            ProjectList.MoveChild(projectItem, 0);

            Error dirCheck = DirAccess.MakeDirRecursiveAbsolute($"user://user_projects/{projectMetadata.Name}");
            if (dirCheck != Error.Ok)
            {
                PopupManager.Singleton.AlertPopup("Failed to create the project directory.");
                return;
            }


            Dictionary projectData = new()
            {
                { "Tags", true },
                { "CreatedAt", projectMetadata.CreatedAt.ToString() },
            };

            ProjectsFile.AddProject(projectMetadata.Name, projectData);
        }

        public bool ProjectExists(string projectName) =>
            BuiltProjectItems.ContainsKey(projectName);
        
        private void RemoveProject()
        {
            if (SelectedProjectItem == null)
            {
                return;
            }

            string projectName = SelectedProjectItem.ProjectMetadata.Value.Name;
            BuiltProjectItems[projectName].QueueFree();
            BuiltProjectItems.Remove(projectName);

            DirAccess dirAccess = DirAccess.Open($"user://user_projects/{projectName}");
            if (dirAccess == null)
            {
                PopupManager.Singleton.AlertPopup("Project folder to delete was not found.");
                return;
            }

            ProjectsFile.RemoveProject(projectName);
            GD.Print(dirAccess.Remove($"user://user_projects/{projectName}"));
            SelectedProjectItem = null;
        }

        private void UpdateButtonStates()
        {
            bool enabled = _selectedProjectItem != null;
            EditButton.Disabled = !enabled;
            RunButton.Disabled = !enabled;
            RenameButton.Disabled = !enabled;
            DeleteButton.Disabled = !enabled;
        }
    }
}