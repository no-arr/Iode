using Godot;
using System;

namespace Iode.Core
{
    public sealed partial class IodeApp : Node
    {
        public static IodeApp Singleton { get; private set; } = null;

        public override void _EnterTree()
        {
            if (Singleton != null)
            {
                throw new InvalidOperationException("Singleton for IodeApp already exists.");
            }

            Singleton = this;
        }

        
    }
}