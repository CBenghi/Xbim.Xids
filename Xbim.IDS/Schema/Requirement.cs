using System;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections;
using System.Xml.Schema;
using System.ComponentModel;
using System.Xml;
using System.Collections.ObjectModel;

namespace Xbim.IDS
{
    public partial class Requirement
    {
		public ObservableCollection<string> Stage { get; set; }

		public Stakeholder Provider { get; set; }

		public ObservableCollection<Stakeholder> Consumer { get; set; }

		public string Name { get; set; }

		public ModelPart ModelSubset { get; set; }

		public Expectation Need { get; set; }

		public string Guid { get; set; }
	}
}
