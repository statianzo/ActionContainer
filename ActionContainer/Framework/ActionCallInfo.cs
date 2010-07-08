using System;

namespace ActionContainer.Framework
{
	public class ActionCallInfo
	{
		public ActionCallInfo()
		{
			Arguments = new object[0];
			ArgumentNames = new string[0];
		}
		public string MethodName { get; set; }
		public object[] Arguments { get; set; }
		public string[] ArgumentNames { get; set; }
		public Type ReturnType { get; set; }
	}
}