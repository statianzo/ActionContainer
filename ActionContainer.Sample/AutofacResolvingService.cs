using System;
using System.Collections.Generic;
using System.Linq;
using ActionContainer.Services;
using Autofac;
using Autofac.Core;

namespace ActionContainer.Sample
{
	public class AutofacResolvingService:IActionContainerResolvingService
	{
		private readonly Func<IContainer> _containerFunc;

		public AutofacResolvingService(Func<IContainer> containerFunc)
		{
			_containerFunc = containerFunc;
		}

		public object Resolve(Type service)
		{
			var container = _containerFunc();
			return container.Resolve(service);
		}

		public object Resolve(Type service, string key)
		{
			var container = _containerFunc();
			return container.Resolve(new KeyedService(key,service));
		}

		public object[] ResolveAll(Type service)
		{
			var container = _containerFunc();
			return ((IEnumerable<object>) container.Resolve(typeof (IEnumerable<>).MakeGenericType(service))).ToArray();
		}
	}
}