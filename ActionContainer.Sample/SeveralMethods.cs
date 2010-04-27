using System;
using ActionContainer.Services;

namespace ActionContainer.Sample
{
	public class SeveralMethods:IActionProvider
	{
		public void SayHello(string name)
		{
			Console.WriteLine("Hello, {0}", name);
		}
	}

	public class RandomMan : IActionProvider
	{
		public int GenerateRandom()
		{
			var random = new Random();
			return random.Next();
		}
	}
}