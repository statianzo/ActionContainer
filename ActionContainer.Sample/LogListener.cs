using System;
using System.Collections.Generic;
using System.Linq;
using ActionContainer.Framework;
using ActionContainer.Services;

namespace ActionContainer.Sample
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
			if (arg == null)
				return false;
			Console.WriteLine("LOG: {0}",arg.Item2);
			return false;
		}
	}
}