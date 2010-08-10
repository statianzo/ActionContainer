using System;
using System.Linq;
using ActionContainer.Framework;
using ActionContainer.Services;

namespace ActionContainer.Sample.Listeners
{
	public class LogListener : IActionListener
	{
		public bool CanHandle(ActionCallInfo callInfo)
		{
			return callInfo.NamedArguments.Any(x => x.Item1.Equals("log", StringComparison.OrdinalIgnoreCase));
		}

		public bool Handle(ActionListenerContext context)
		{

			var arg = context.CallInfo.NamedArguments.FirstOrDefault(x => x.Item1.Equals("log", StringComparison.OrdinalIgnoreCase));
			if (arg == null || !(arg.Item2 is bool) || !(bool)arg.Item2)
				return false;
			var unnamed = context.CallInfo.UnnamedArguments;
			Console.Write("Calling {0}({1}) with Args: ", context.CallInfo.MethodName, context.CallInfo.ReturnType);
			for (int i = 0; i < unnamed.Count; i++)
			{
				var val = unnamed[i];
				Console.Write("input{0}: {1} ", i, val);
			}
			Console.WriteLine();
			return false;
		}
	}
}