﻿using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace ArchiCop.Core
{
    public class VisualStudioProjectGraph : BidirectionalGraph<VisualStudioProject, Edge<VisualStudioProject>>
    {
        public VisualStudioProjectGraph(IEnumerable<Edge<VisualStudioProject>> edges)
        {
            foreach (VisualStudioProject project in edges.Select(item=>item.Source))
            {
                if (!ContainsVertex(project))
                {
                    AddVertex(project);
                }
            }

            foreach (VisualStudioProject project in edges.Select(item => item.Target))
            {
                if (!ContainsVertex(project))
                {
                    AddVertex(project);
                }
            }

            foreach (var edge in edges)
            {
                AddEdge(new Edge<VisualStudioProject>(edge.Source, edge.Target));
            }
        }

        public VisualStudioProjectGraph(IEnumerable<VisualStudioProject> projects )
        {
            foreach (VisualStudioProject project in projects)
            {
                if (!ContainsVertex(project))
                {
                    AddVertex(project);
                }
            }

            foreach (VisualStudioProject projectFrom in projects)
            {
                foreach (VisualStudioProjectProjectReference projectReference in projectFrom.ProjectReferences)
                {
                    var projectTo = projects.First(item => item.ProjectGuid == projectReference.Project);
                    AddEdge(new Edge<VisualStudioProject>(projectFrom, projectTo));
                }
            }
        }
    }
}