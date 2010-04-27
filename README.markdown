ActionContainer
---------------

Action Container is a dynamic Service Agent implementation.

It provides a `dynamic` object that will resolve a service and call a method based upon the call's name, parameters, and return type.

For example,

	public class Needy : IDependOnSomething
	{
		public dynamic ServiceAgent { get; set; }

		public void DoYourThing()
		{
			//Void call
			ServiceAgent.SayHello("Jason");

			//Receive an int
			int rand = ServiceAgent.GenerateRandom();
			Console.WriteLine("Random of {0}", rand);
		}
	}

The first call to `ServiceAgent.SayHello("Jason");` will locate an `IActionProvider` of a void SayHello method that takes one string argument.

The call `int rand = ServiceAgent.GenerateRandom();' will locate an `IActionProvider` with a method GenerateRandom with no arguments that returns an integer.  
