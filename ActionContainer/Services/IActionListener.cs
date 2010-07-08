using ActionContainer.Framework;

namespace ActionContainer.Services
{
	public interface IActionListener
	{
		bool CanHandle(ActionCallInfo callInfo);
		bool Handle(ActionListenerContext context);
	}
}