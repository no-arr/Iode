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

        public ProjectItem ApplyProjectMetadata(ProjectMetadata projectMetadata)
        {
            ProjectName.Text = projectMetadata.Name;
            ProjectMade.Text = projectMetadata.CreatedAt.ToLongDateString();
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
    }
}