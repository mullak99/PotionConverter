namespace PotionConverter
{
	public static class Utils
	{
		public static int GetAverageTint(List<Effect> effects)
		{
			if (effects.Count == 0)
				return 3694022;

			float rSum = 0.0f;
			float gSum = 0.0f;
			float bSum = 0.0f;
			int totalAmplification = 0;

			foreach (var effect in effects)
			{
				int colorValue = effect.Colour.StartsWith("0x") ? Convert.ToInt32(effect.Colour[2..], 16) : Convert.ToInt32(effect.Colour);

				int red = (colorValue >> 16) & 0xFF;
				int green = (colorValue >> 8) & 0xFF;
				int blue = (colorValue >> 0) & 0xFF;

				int amplification = (effect.Amplifier ?? 0) + 1;

				rSum += amplification * red / 255.0f;
				gSum += amplification * green / 255.0f;
				bSum += amplification * blue / 255.0f;

				totalAmplification += amplification;
			}

			if (totalAmplification == 0)
				return 0; // Fully transparent

			double rAvg = (rSum / totalAmplification) * 255.0f;
			double gAvg = (gSum / totalAmplification) * 255.0f;
			double bAvg = (bSum / totalAmplification) * 255.0f;

			return (int)rAvg << 16 | (int)gAvg << 8 | (int)bAvg;
		}

		public static void FastDeleteAll(string path, bool deleteDirectory = true)
		{
			try
			{
				Parallel.ForEach(Directory.GetFiles(path), File.Delete);
				Parallel.ForEach(Directory.GetDirectories(path), x => FastDeleteAll(x));
				if (deleteDirectory)
					Directory.Delete(path, false);
			}
			catch (Exception)
			{
				// Ignore exceptions
			}
		}
	}
}
