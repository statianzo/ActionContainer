using System.Linq;
using ActionContainer.Framework;
using ActionContainer.Services;

namespace ActionContainer.Sample.Listeners
{
	public class StringTrimListener : IActionListener
	{
		public bool CanHandle(ActionCallInfo callInfo)
		{
			return callInfo.UnnamedArguments.Any(x => x is string);
		}

		public bool Handle(ActionListenerContext context)
		{
			var unnamed = context.CallInfo.UnnamedArguments;
			for (int i =0; i < unnamed.Count; i++)
			{
				var val = unnamed[i] as string;
				if (val != null)
					unnamed[i] = val.Trim();
			}
			return false;
		}
	}
}