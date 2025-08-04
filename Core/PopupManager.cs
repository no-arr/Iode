using Godot;
using System;

namespace Iode.Core
{
    public sealed partial class PopupManager : Node
    {
        [Export]
        private ConfirmationDialog ConfirmationDialog = null;

        public static PopupManager Singleton { get; private set; } = null;

        private Action OnConfirm = null;
        private Action OnCancel = null;

        public override void _EnterTree()
        {
            if (Singleton != null)
            {
                throw new InvalidOperationException("Singleton for PopupManager already exists.");
            }

            Singleton = this;
        }

        public void ConfirmationPopup(string description, Action[] actions)
        {
            if (OnConfirm != null)
                ConfirmationDialog.Confirmed -= OnConfirm;
            if (OnCancel != null)
                ConfirmationDialog.Canceled -= OnCancel;
            
            ConfirmationDialog.DialogText = description;

            OnConfirm = actions[0];
            OnCancel = actions[1];

            ConfirmationDialog.Confirmed += OnConfirm;
            ConfirmationDialog.Canceled += OnCancel;

            ConfirmationDialog.Popup();
        }

        public void AlertPopup(string description, string title = "Uh oh")
        {
            AcceptDialog acceptDialog = new()
            {
                Visible = true,
                DialogText = description,
                Title = title,
                Unfocusable = true,
                AlwaysOnTop = true,
                Unresizable = true,
                Transient = false,
                InitialPosition = Window.WindowInitialPosition.CenterMainWindowScreen,
            };

            acceptDialog.Confirmed += acceptDialog.QueueFree;
            AddChild(acceptDialog);
        }
    }
}