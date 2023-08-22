namespace PotionConverter
{
	public class Potion
	{
		public string Name { get; set; } = null!;
		public string? PotionName { get; set; } = null!;
		public string? TippedArrowName { get; set; }
		public Effect? Effect { get; set; }
		public List<Effect>? Effects { get; set; }
		public bool DisablePotion { get; set; }
		public bool DisableSplashPotion { get; set; }
		public bool DisableLingeringPotion { get; set; }

		public string? GetPotionName()
		{
			if (!DisablePotion && PotionName != null)
			{
				if (PotionName.ToLower() == "water")
					return "potion_bottle_drinkable.png";
				return $"potion_bottle_{PotionName.Replace(" ", "_")}.png";
			}
			return null;
		}

		public string? GetSplashPotionName()
		{
			if (!DisableSplashPotion && PotionName != null)
			{
				if (PotionName.ToLower() == "water")
					return "potion_bottle_splash.png";
				return $"potion_bottle_splash_{PotionName.Replace(" ", "_")}.png";
			}
			return null;
		}

		public string? GetLingeringPotionName()
		{
			if (!DisableLingeringPotion && PotionName != null)
			{
				if (PotionName.ToLower() == "water")
					return "potion_bottle_lingering.png";
				return $"potion_bottle_lingering_{PotionName.Replace(" ", "_")}.png";
			}
			return null;
		}

		public string? GetTippedArrowName()
			=> TippedArrowName != null ? $"tipped_arrow_{TippedArrowName.Replace(" ", "_")}.png" : null;

		public int GetColour()
		{
			if (Effect != null)
				return Utils.GetAverageTint(new List<Effect> { Effect });
			if (Effects != null)
				return Utils.GetAverageTint(Effects);

			Console.WriteLine($"No effect for {Name}");
			return 0;
		}
	}

	public class Effect
	{
		public string Colour { get; set; } = null!;
		public int? Amplifier { get; set; }
	}
}
