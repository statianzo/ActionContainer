using System;
using System.Collections.Generic;

namespace ActionContainer.Framework
{
	public class ActionCallInfo
	{
		public ActionCallInfo()
		{
			UnnamedArguments = new List<object>();
		}
		public string MethodName { get; set; }
		public IList<object> UnnamedArguments { get; set; }
		public IList<Tuple<string, object>> NamedArguments { get; set; }
		public Type ReturnType { get; set; }
	}
}