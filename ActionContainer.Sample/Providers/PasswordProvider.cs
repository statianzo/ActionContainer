using ActionContainer.Services;

namespace ActionContainer.Sample.Providers
{
	public class PasswordProvider: IActionProvider
	{
		public string GeneratePassword()
		{
			return "RANDOMPASSWORD";
		}
	}
}