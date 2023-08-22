using PotionConverter.Logic;

namespace PotionConverter
{
	public class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				switch (args.Length)
				{
					case 0:
					case > 0 when args[0] == "generate":
					{
						Console.WriteLine("Generating Bedrock potion & tipped arrow textures...");
						JavaConvert _ = new();
						break;
					}
					case > 0 when args[0] == "reverse":
					{
						Console.WriteLine("Reverse-engineering Bedrock potion colours...");
						ReverseBedrock _ = new();
						break;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			#if !DEBUG
			finally
			{
				Console.WriteLine("Press any key to exit...");
				Console.ReadKey();
			}
			#endif
		}
	}
}