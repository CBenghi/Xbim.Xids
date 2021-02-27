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
    public partial class RequirementsCollection
    {

		public ObservableCollection<string> Stage { get; set; }

		public Stakeholder Provider { get; set; }

		public ObservableCollection<Stakeholder> Consumer { get; set; }

		public ObservableCollection<Requirement> Requirements { get; set; } = new ObservableCollection<Requirement>();
	}
}