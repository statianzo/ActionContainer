using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ActionContainer.Framework;
using ActionContainer.Framework.Extensions;
using ActionContainer.Services;

namespace ActionContainer
{
	public class ServiceAgent : DynamicObject
	{
		private readonly IActionContainerResolvingService _resolvingService;


		public ServiceAgent(IActionContainerResolvingService resolvingService)
		{
			_resolvingService = resolvingService;
		}

		public ActionCallInfo CallInfo { get; set; }


		public override bool TryConvert(ConvertBinder binder, out object result)
		{
			CallInfo.ReturnType = binder.ReturnType;
			result = ExecuteListeners(CallInfo);
			return true;
		}

		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			var callInfo = new ActionCallInfo
			{
				NamedArguments = binder.CallInfo
					.ArgumentNames
					.Reverse()
					.Zip(args.Reverse(), (x, y) => new Tuple<string, object>(x, y))
					.ToList(),
				UnnamedArguments = args.Take(binder.CallInfo.ArgumentCount - binder.CallInfo.ArgumentNames.Count).ToList(),
				MethodName = binder.Name
			};
			result = binder.ResultDiscarded()
			         	? ExecuteListeners(callInfo)
			         	: new ServiceAgent(_resolvingService) {CallInfo = callInfo};
			return true;
		}

		public object ExecuteListeners(ActionCallInfo callInfo)
		{
			object[] actionListeners = _resolvingService.ResolveAll(typeof (IActionListener));
			IEnumerable<IActionListener> handlers = actionListeners.Cast<IActionListener>().Where(h => h.CanHandle(callInfo));
			var context = new ActionListenerContext(callInfo);
			bool wasHandled = handlers.Any(actionListener => actionListener.Handle(context));
			if (!wasHandled)
				throw new InvalidOperationException("No handler for call");
			return context.Result;
		}
	}
}