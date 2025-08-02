using Godot;
using System;

namespace Iode.Core
{
    public sealed class IodeSettings
    {
        public static IodeSettings Singleton { get; private set; } = null;

        IodeSettings()
        {
            if (Singleton != null)
            {
                throw new InvalidOperationException("Singleton for IodeSettings already exists.");
            }

            Singleton = this;
        }

        
    }
}