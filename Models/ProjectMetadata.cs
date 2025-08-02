using System;
using System.Collections.Generic;

namespace Iode.Models
{
    public struct ProjectMetadata
    {
        public string Name;
        public string Path;
        public DateTime CreatedAt;
        public List<string> Tags;
    }
}