﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArchiCop.Core
{
    public class VisualStudioProjectLoadEngine : LoadEngine
    {
        private readonly List<string> _projectsFiles;
        private readonly IEnumerable<VisualStudioProject> _projects;

        public VisualStudioProjectLoadEngine(string path)
        {
            _projectsFiles =
                new List<string>(Directory.GetFiles(path, "*csproj", SearchOption.AllDirectories));
            _projectsFiles.AddRange(
                new List<string>(Directory.GetFiles(path, "*fsproj", SearchOption.AllDirectories)));
            _projectsFiles.AddRange(
                new List<string>(Directory.GetFiles(path, "*vbproj", SearchOption.AllDirectories)));

            _projectsFiles = _projectsFiles.Select(Path.GetFullPath).ToList();

            IVisualStudioProjectRepository repository = new VisualStudioProjectRepository();

            _projects = repository.GetProjects(_projectsFiles);
        }

        protected override IEnumerable<ArchiCopEdge> GetEdges()
        {
            var edges = new List<ArchiCopEdge>();

            foreach (string project in _projectsFiles)
            {                
                foreach (string reference in GetProjectDependencies(project))
                {
                    edges.Add(new ArchiCopEdge(new ArchiCopVertex(Path.GetFileNameWithoutExtension(project)),
                                               new ArchiCopVertex(reference)));
                }
            }

            return edges;
        }

        private IEnumerable<string> GetProjectDependencies(string path)
        {
            VisualStudioProject projectRoot = _projects.First(item => item.ProjectPath.ToLower() == path.ToLower());

            var list = projectRoot.ProjectReferences.Select(project => project.Name).ToList();
            list.AddRange(projectRoot.LibraryReferences.Select(library => library.Name));

            return list;
        }
    }
}