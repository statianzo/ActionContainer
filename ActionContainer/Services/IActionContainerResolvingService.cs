using System;

namespace ActionContainer.Services
{
	public interface IActionContainerResolvingService
	{
		object Resolve(Type service);
		object Resolve(Type service, string key);
	}
}