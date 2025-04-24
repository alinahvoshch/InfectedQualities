using InfectedQualities.Core;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Tiles.Plants
{
	public class HallowedThorns : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileShine[Type] = 9000;
			Main.tileCut[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileNoFail[Type] = true;

			TileID.Sets.Conversion.Thorn[Type] = true;
			TileID.Sets.TileCutIgnore.IgnoreDontHurtNature[Type] = false;
			TileID.Sets.TouchDamageDestroyTile[Type] = true;
			TileID.Sets.TouchDamageImmediate[Type] = 17;
			TileID.Sets.IgnoredByGrowingSaplings[Type] = true;

			TileID.Sets.Hallow[Type] = true;
			TileID.Sets.HallowBiome[Type] = 1;
			TileID.Sets.HallowBiomeSight[Type] = true;
			TileID.Sets.AddJungleTile(Type);

			DustType = DustID.HallowedPlants;
			HitSound = SoundID.Grass;

			AddMapEntry(new(29, 160, 247), Language.GetText("MapObject.Thorn"));

			TileLoader.RegisterConversion(TileID.JungleThorns, BiomeConversionID.Hallow, ApplyConversion);
			TileLoader.RegisterConversion(Type, BiomeConversionID.Hallow, ApplyConversion); //Placing this here prevents hallowed thorns from breaking via blue solution, the delegate returning false is very important
		}

		public bool ApplyConversion(int i, int j, int type, int conversionType)
		{
			WorldGen.ConvertTile(i, j, Type);
			return false;
		}

		public override void Convert(int i, int j, int conversionType)
		{
			switch (conversionType)
			{
				case BiomeConversionID.Chlorophyte:
					WorldGen.KillTile(i, j);
					if (Main.netMode != NetmodeID.SinglePlayer)
					{
						NetMessage.SendTileSquare(-1, i, j);
					}
					break;
				case BiomeConversionID.Purity:
					WorldGen.ConvertTile(i, j, TileID.JungleThorns);
					break;
				case BiomeConversionID.Corruption:
					WorldGen.ConvertTile(i, j, TileID.CorruptThorns);
					return;
				case BiomeConversionID.Crimson:
					WorldGen.ConvertTile(i, j, TileID.CrimsonThorns);
					return;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 3 : 10;

		public override bool IsTileDangerous(int i, int j, Player player) => true;

		public override void RandomUpdate(int i, int j)
		{
			WorldGen.GrowSpike(i, j, Type, (ushort)ModContent.TileType<HallowedJungleGrass>());
			WorldGen.SpreadInfectionToNearbyTile(i, j, BiomeConversionID.Hallow);
		}

		public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
		{
			if (down == ModContent.TileType<HallowedJungleGrass>())
			{
				down = Type;
			}
		}

		public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedBiomes;
	}
}
