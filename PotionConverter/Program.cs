using SixLabors.ImageSharp.Advanced;
using Newtonsoft.Json;

namespace PotionConverter
{
	public class Program
	{
		private const string POTIONS_FILE = "potions.json";
		private const string OUTPUT_DIRECTORY = "output";

		private static readonly string[] REQUIRED_FILES = {
			"potion.png", "splash_potion.png", "lingering_potion.png", "potion_overlay.png", "tipped_arrow_base.png", "tipped_arrow_head.png"
		};

		private static readonly JsonSerializerSettings _serializerSettings = new()
		{
			Formatting = Formatting.Indented,
			NullValueHandling = NullValueHandling.Ignore,
			DefaultValueHandling = DefaultValueHandling.Ignore
		};

		private static void Main()
		{
			try
			{
				if (REQUIRED_FILES.Any(file => !File.Exists(file)))
					throw new FileNotFoundException($"Required file not found.\nPlease include all of the following:\n{string.Join("\n", REQUIRED_FILES.Select(x => $"- {x}"))}");
				if (!File.Exists(POTIONS_FILE))
					throw new FileNotFoundException("potions.json not found.");

				List<Potion>? potions = JsonConvert.DeserializeObject<List<Potion>>(File.ReadAllText(POTIONS_FILE), _serializerSettings);
				if (potions == null || potions.Count == 0)
					throw new FileNotFoundException("No potions found in potions.json");

				// Clear output directory
				if (Directory.Exists(OUTPUT_DIRECTORY))
					Utils.FastDeleteAll(OUTPUT_DIRECTORY, false);

				// Potion textures
				Image<Rgba32> potionBottle = Image.Load<Rgba32>("potion.png");
				Image<Rgba32> splashPotionBottle = Image.Load<Rgba32>("splash_potion.png");
				Image<Rgba32> lingeringPotionBottle = Image.Load<Rgba32>("lingering_potion.png");
				Image<Rgba32> potionOverlay = Image.Load<Rgba32>("potion_overlay.png");

				// Tipped arrow textures
				Image<Rgba32> tippedArrowBase = Image.Load<Rgba32>("tipped_arrow_base.png");
				Image<Rgba32> tippedArrowOverlay = Image.Load<Rgba32>("tipped_arrow_head.png");

				// Create potion textures
				List<Task> tasks = potions.Select(potion => Task.Run(() => CreatePotions(potion, potionBottle, splashPotionBottle, lingeringPotionBottle, tippedArrowBase, potionOverlay, tippedArrowOverlay))).ToList();

				// Copy base textures
				tasks.Add(Task.Run(() =>
				{
					Console.WriteLine("Copying base textures");

					File.Copy("potion.png", Path.Combine(OUTPUT_DIRECTORY, "potion_bottle_empty.png"));
					File.Copy("lingering_potion.png", Path.Combine(OUTPUT_DIRECTORY, "potion_bottle_lingering_empty.png"));
					File.Copy("tipped_arrow_base.png", Path.Combine(OUTPUT_DIRECTORY, "tipped_arrow_base.png"));
					File.Copy("tipped_arrow_head.png", Path.Combine(OUTPUT_DIRECTORY, "tipped_arrow_head.png"));
				}));

				// Create (merged) tipped arrow texture
				tasks.Add(Task.Run(() =>
				{
					Console.WriteLine("Creating tipped_arrow.png");
					CreateOverlayTexture(tippedArrowBase, tippedArrowOverlay, "tipped_arrow.png", null);
				}));

				Task.WaitAll(tasks.ToArray());
				Console.WriteLine("Done!");
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

		private static void CreatePotions(Potion potion, Image potionImg, Image splashImg, Image lingeringImg, Image tippedArrowBaseImg, Image<Rgba32> potionOverlayImage, Image<Rgba32> tippedArrowOverlayImage)
		{
			List<Task> tasks = new();

			if (potion.PotionName != null)
			{
				Console.WriteLine($"Creating potion: {potion.Name}");
				tasks.AddRange(new List<Task>
				{
					Task.Run(() => CreateOverlayTexture(potionImg, potionOverlayImage, potion.GetPotionName()!, potion.GetColour())),
					Task.Run(() => CreateOverlayTexture(splashImg, potionOverlayImage, potion.GetSplashPotionName()!, potion.GetColour())),
					Task.Run(() => CreateOverlayTexture(lingeringImg, potionOverlayImage, potion.GetLingeringPotionName()!, potion.GetColour()))
				});
			}

			if (potion.TippedArrowName != null)
			{
				Console.WriteLine($"Creating tipped arrow: {potion.Name}");
				tasks.Add(Task.Run(() => CreateOverlayTexture(tippedArrowBaseImg, tippedArrowOverlayImage, potion.GetTippedArrowName()!, potion.GetColour())));
			}

			Task.WaitAll(tasks.ToArray());
		}

		private static void CreateOverlayTexture(Image baseImage, Image<Rgba32> overlayImage, string finalFileName, int? tintColor)
		{
			Image<Rgba32> overlay = overlayImage.Clone();
			if (tintColor != null)
			{
				int red = (tintColor.Value >> 16) & 0xFF;
				int green = (tintColor.Value >> 8) & 0xFF;
				int blue = (tintColor.Value >> 0) & 0xFF;

				overlay.Mutate(_ =>
				{
					Memory<Rgba32> pixelBuffer = overlay.GetPixelMemoryGroup().Single();
					Span<Rgba32> pixelSpan = pixelBuffer.Span;
					for (int i = 0; i < pixelSpan.Length; i++)
					{
						Rgba32 pixel = pixelSpan[i];
						pixel.R = (byte)Math.Floor(pixel.R * red / 255.0f);
						pixel.G = (byte)Math.Floor(pixel.G * green / 255.0f);
						pixel.B = (byte)Math.Floor(pixel.B * blue / 255.0f);
						pixelSpan[i] = pixel;
					}
				});
			}

			using Image<Rgba32> result = new(baseImage.Width, baseImage.Height);
			result.Mutate(context =>
			{
				context.DrawImage(baseImage, new Point(0, 0), 1.0f); // Draw the base image
				context.DrawImage(overlay, new Point(0, 0), 1.0f);   // Draw the overlay
			});

			if (!Directory.Exists(OUTPUT_DIRECTORY))
				Directory.CreateDirectory(OUTPUT_DIRECTORY);

			result.Save(Path.Combine(OUTPUT_DIRECTORY, finalFileName));
		}
	}
}