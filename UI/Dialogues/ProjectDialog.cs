using System;
using Godot;
using Iode.Core;
using Iode.Core.Utils;

namespace Iode.UI.Dialogues
{
    public sealed partial class ProjectDialog : PopupPanel
    {
        [Export]
        private Button CreateButton, CancelButton;

        [Export]
        private LineEdit ProjectName;

        [Export]
        private CheckBox EditNow;

        public static ProjectDialog Singleton { get; private set; }

        public override void _EnterTree()
        {
            if (Singleton != null)
            {
                throw new InvalidOperationException("Singleton for ProjectDialog already exists.");
            }

            Singleton = this;

            CreateButton.Pressed += CreationSubmitted;
            CancelButton.Pressed += () => Visible = false;
        }

        private void CreationSubmitted()
        {
            string projectName = ProjectName.Text;

            if (!StringVerifier.IsValid(projectName) || !StringVerifier.IsOnlyEnglishLetters(projectName))
            {
                PopupManager.Singleton.AlertPopup("The project name must only contain english letters and the length to be under 100 characters!");
                return;
            }

            if (EditNow.ButtonPressed)
            {
                GD.Print("Open in editor");
            }

            ProjectManager.Singleton.MakeProject(new()
            {
                Name = projectName,
                CreatedAt = DateTime.Now,
                Tags = [],
            });


            Visible = false;
        }
    }
}