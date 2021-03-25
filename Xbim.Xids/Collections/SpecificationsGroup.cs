using System;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections;
using System.Xml.Schema;
using System.ComponentModel;
using System.Xml;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Xbim.Xids
{
	public partial class SpecificationsGroup
	{
		public List<string> Stage { get; set; }

		public Stakeholder Provider { get; set; }

		public List<Stakeholder> Consumer { get; set; }

		public List<Specification> Specifications { get; set; } = new List<Specification>();
	}
}