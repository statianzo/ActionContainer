namespace ActionContainer.Framework
{
	public class ActionListenerContext
	{
		public ActionListenerContext(ActionCallInfo callInfo)
		{
			CallInfo = callInfo;
		}

		public ActionCallInfo CallInfo { get; private set; }
		public object Result { get; set; }
	}
}