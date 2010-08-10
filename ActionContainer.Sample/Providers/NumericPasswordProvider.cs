using ActionContainer.Services;

namespace ActionContainer.Sample.Providers
{
	public class NumericPasswordProvider: IActionProvider
	{
		public int GeneratePassword()
		{
			return 42;
		}
	}
}