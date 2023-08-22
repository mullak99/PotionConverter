namespace PotionConverter.Logic
{
	public class ReverseBedrock
	{
		private const string POTIONS_FILE = "potions_reverseengineered.txt";
		private const string INPUT_DIRECTORY = "reverse";

		private static readonly string[] REQUIRED_FILES = {
			"potion.png", "splash_potion.png", "lingering_potion.png", "potion_overlay.png"
		};

		public ReverseBedrock()
		{
			if (REQUIRED_FILES.Any(file => !File.Exists(file)))
				throw new FileNotFoundException($"Required file not found.\nPlease include all of the following:\n{string.Join("\n", REQUIRED_FILES.Select(x => $"- {x}"))}");
			if (!Directory.Exists(INPUT_DIRECTORY))
				throw new DirectoryNotFoundException($"Input directory not found: {INPUT_DIRECTORY}");

			string[] files = Directory.GetFiles(INPUT_DIRECTORY, "*.png");

			Image<Rgba32> potionBottle = Image.Load<Rgba32>("potion.png");
			Image<Rgba32> splashPotionBottle = Image.Load<Rgba32>("splash_potion.png");
			Image<Rgba32> lingeringPotionBottle = Image.Load<Rgba32>("lingering_potion.png");
			Image<Rgba32> potionOverlay = Image.Load<Rgba32>("potion_overlay.png");

			if (File.Exists(POTIONS_FILE))
				File.Delete(POTIONS_FILE);
			File.Create(POTIONS_FILE).Close();

			#if DEBUG
			string debugOutput = "overlayDebug";
			if (!Directory.Exists(debugOutput))
				Directory.CreateDirectory(debugOutput);
			else
				Utils.FastDeleteAll(debugOutput, false);
			#endif

			foreach (string file in files)
			{
				string fileName = Path.GetFileName(file);
				if (fileName.StartsWith("tipped_arrow"))
				{
					Console.WriteLine($"Skipping {Path.GetFileName(file)}...");
					continue;
				}

				Image<Rgba32> baseImage = Image.Load<Rgba32>(file);
				Image<Rgba32> isolated;

				if (fileName.StartsWith("potion_bottle_splash"))
					isolated = IsolateOverlay(baseImage, splashPotionBottle);
				else if (fileName.StartsWith("potion_bottle_lingering"))
					isolated = IsolateOverlay(baseImage, lingeringPotionBottle);
				else if (fileName.StartsWith("potion_bottle"))
					isolated = IsolateOverlay(baseImage, potionBottle);
				else
				{
					Console.WriteLine($"Skipping {fileName}...");
					continue;
				}
				Console.WriteLine($"Isolated overlay for {fileName}");

				#if DEBUG
				isolated.Save(Path.Combine(debugOutput, fileName));
				#endif

				int tint = ExtractOriginalTint(potionOverlay, isolated);
				File.AppendAllText(POTIONS_FILE, $"{Path.GetFileNameWithoutExtension(file)} = {tint}\n");
			}
		}

		public Image<Rgba32> IsolateOverlay(Image<Rgba32> finalImage, Image<Rgba32> baseImage)
		{
			if (finalImage.Width != baseImage.Width || finalImage.Height != baseImage.Height)
				throw new ArgumentException("Both images must have the same dimensions.");

			Image<Rgba32> isolatedOverlay = new Image<Rgba32>(finalImage.Width, finalImage.Height);

			for (int x = 0; x < finalImage.Width; x++)
			{
				for (int y = 0; y < finalImage.Height; y++)
				{
					Rgba32 finalPixel = finalImage[x, y];
					Rgba32 basePixel = baseImage[x, y];

					// Check if the pixels are the same
					if (finalPixel.Equals(basePixel))
					{
						// Set to transparent if they are the same
						isolatedOverlay[x, y] = new Rgba32(0, 0, 0, 0);
					}
					else
					{
						// Otherwise, use the final pixel as it is
						isolatedOverlay[x, y] = finalPixel;
					}
				}
			}
			return isolatedOverlay;
		}

		public int ExtractOriginalTint(Image<Rgba32> overlayImage, Image<Rgba32> convertedImage)
		{
			int pixelCount = 0;
			int rSum = 0;
			int gSum = 0;
			int bSum = 0;

			for (int y = 0; y < overlayImage.Height; y++)
			{
				for (int x = 0; x < overlayImage.Width; x++)
				{
					Rgba32 overlayPixel = overlayImage[x, y];
					Rgba32 convertedPixel = convertedImage[x, y];

					// Skip transparent pixels
					if (overlayPixel.A == 0)
						continue;

					int rOriginal = (int)Math.Floor(convertedPixel.R / (overlayPixel.R / 255.0f));
					int gOriginal = (int)Math.Floor(convertedPixel.G / (overlayPixel.G / 255.0f));
					int bOriginal = (int)Math.Floor(convertedPixel.B / (overlayPixel.B / 255.0f));

					rOriginal = Math.Clamp(rOriginal, 0, 255);
					gOriginal = Math.Clamp(gOriginal, 0, 255);
					bOriginal = Math.Clamp(bOriginal, 0, 255);

					rSum += rOriginal;
					gSum += gOriginal;
					bSum += bOriginal;
					pixelCount++;
				}
			}

			if (pixelCount == 0)
				return 0; // Fully transparent

			int rAvg = rSum / pixelCount;
			int gAvg = gSum / pixelCount;
			int bAvg = bSum / pixelCount;

			#if DEBUG
			Console.WriteLine($"R: {rAvg} | G: {gAvg} | B: {bAvg}");
			#endif

			return (rAvg << 16) | (gAvg << 8) | bAvg;
		}
	}
}
