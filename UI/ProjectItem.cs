using Godot;
using Iode.Core;
using Iode.Core.Utils;
using Iode.Models;

namespace Iode.UI
{
    public partial class ProjectItem : Control
    {

        [Export]
        private Label ProjectName, ProjectMade;

        public override void _Ready()
        {
            GetNode<Button>("Overlay").Pressed += OnSelection;
        }

        public ProjectMetadata? ProjectMetadata = null;

        public ProjectItem ApplyProjectMetadata(ProjectMetadata projectMetadata)
        {
            ProjectMetadata = projectMetadata;
            ProjectName.Text = projectMetadata.Name;
            ProjectMade.Text = $"Created at: {projectMetadata.CreatedAt}";
            return this;
        }

        public void RenameProject(string newName)
        {
            if (!StringVerifier.IsValid(newName))
            {
                PopupManager.Singleton.AlertPopup("Invalid new name for the project.");
                return;
            }

            if (!StringVerifier.IsOnlyEnglishLetters(newName))
            {
                PopupManager.Singleton.AlertPopup("Only english letters are allowed.");
                return;
            }

            // TODO: Renaming of the project
        }

        private void OnSelection()
        {
            if (ProjectManager.Singleton.SelectedProjectItem == this)
            {
                GD.Print("Double click");
                PopupManager.Singleton.ConfirmationPopup("Testing description this is.", [() => { }, () => { }]);
                return;
            }

            ThemeTypeVariation = "IodePrimary";
            ProjectManager.Singleton.SelectedProjectItem = this;
        }
    }
}