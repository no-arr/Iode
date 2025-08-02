using Godot;
using Iode.Models;
using System;
using System.Collections.Generic;

namespace Iode.Core
{
    public sealed partial class ProjectManager : Control
    {
        public static ProjectManager Singleton { get; private set; } = null;

        public override void _Ready()
        {
            if (Singleton != null)
            {
                throw new InvalidOperationException("Singleton for ProjectManager already exists.");
            }

            Singleton = this;

            // As `ProjectManager` is the first scene, we check for editor settings and etc.
        }

        private static List<ProjectMetadata> GetProjects()
        {
            List<ProjectMetadata> projects = [];

            

            return projects;
        }
    }
}