using Godot;
using Iode.Models;
using Iode.UI;
using Iode.UI.Dialogues;
using System;
using System.Collections.Generic;

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

            foreach (ProjectMetadata projectMetadata in GetProjects())
            {
                ProjectItem projectItem = ProjectItem.Instantiate<ProjectItem>().ApplyProjectMetadata(projectMetadata);
                ProjectList.CallDeferred("add_child", projectItem);
            }
        }

        public void MakeProject(ProjectMetadata projectMetadata)
        {
            
        }

        private static List<ProjectMetadata> GetProjects()
        {
            List<ProjectMetadata> projects = [];



            return projects;
        }
    }
}