using System;

namespace ActionContainer.Services
{
	public interface IActionContainerRegistrationService
	{
		void Register(Type implementation, Type service);
		void Register(Type implementation, Type service, string key);
		void RegisterInstance(object instance, Type service, string key);
	}
}