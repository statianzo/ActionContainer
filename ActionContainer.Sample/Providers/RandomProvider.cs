using System;
using ActionContainer.Services;

namespace ActionContainer.Sample.Providers
{
	public class RandomProvider : IActionProvider
	{
		public int GenerateRandom()
		{
			var random = new Random();
			return random.Next();
		}
	}
}