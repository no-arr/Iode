using Godot;
using System;

namespace Iode.Core
{
    public sealed partial class PopupManager : Node
    {
        public static PopupManager Singleton { get; private set; } = null;

        public override void _EnterTree()
        {
            if (Singleton != null)
            {
                throw new InvalidOperationException("Singleton for PopupManager already exists.");
            }

            Singleton = this;
        }

        public override void _ExitTree()
        {
            Singleton = null;
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