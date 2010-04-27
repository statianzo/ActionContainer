using System;
using ActionContainer.Services;
using Autofac;

namespace ActionContainer.Sample
{
	public class AutofacRegistrationService : IActionContainerRegistrationService
	{
		private readonly ContainerBuilder _builder;

		public AutofacRegistrationService(ContainerBuilder builder)
		{
			_builder = builder;
		}

		public void Register(Type implementation, Type service)
		{
			_builder.RegisterType(implementation).As(service);
		}

		public void Register(Type implementation, Type service, string key)
		{
			_builder.RegisterType(implementation).Keyed(key, service);
		}

		public void RegisterInstance(object instance, Type service, string key)
		{
			_builder.RegisterInstance(instance).Keyed(key, service);
		}
	}
}