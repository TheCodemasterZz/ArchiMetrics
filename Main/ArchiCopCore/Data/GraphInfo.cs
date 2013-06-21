﻿using System.Collections.Generic;

namespace ArchiCop.Data
{
    public class GraphInfo : InfoObject
    {
        public GraphInfo()
        {
            Rules = new List<GraphRuleInfo>();
        }

        public List<GraphRuleInfo> Rules { get; private set; }

        public LoadEngineInfo LoadEngine { get; set; }
    }
}