using InfectedQualities.Content.Extras;
using InfectedQualities.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace InfectedQualities.Content.Items.Placables
{
    public class SapphireEbonstoneBlock() : InfectedGemstoneBlock(InfectionType.Corrupt, GemType.Sapphire);
    public class RubyEbonstoneBlock() : InfectedGemstoneBlock(InfectionType.Corrupt, GemType.Ruby);
    public class EmeraldEbonstoneBlock() : InfectedGemstoneBlock(InfectionType.Corrupt, GemType.Emerald);
    public class TopazEbonstoneBlock() : InfectedGemstoneBlock(InfectionType.Corrupt, GemType.Topaz);
    public class AmethystEbonstoneBlock() : InfectedGemstoneBlock(InfectionType.Corrupt, GemType.Amethyst);
    public class DiamondEbonstoneBlock() : InfectedGemstoneBlock(InfectionType.Corrupt, GemType.Diamond);
    public class AmberEbonstoneBlock() : InfectedGemstoneBlock(InfectionType.Corrupt, GemType.Amber);

    public class SapphireCrimstoneBlock() : InfectedGemstoneBlock(InfectionType.Crimson, GemType.Sapphire);
    public class RubyCrimstoneBlock() : InfectedGemstoneBlock(InfectionType.Crimson, GemType.Ruby);
    public class EmeraldCrimstoneBlock() : InfectedGemstoneBlock(InfectionType.Crimson, GemType.Emerald);
    public class TopazCrimstoneBlock() : InfectedGemstoneBlock(InfectionType.Crimson, GemType.Topaz);
    public class AmethystCrimstoneBlock() : InfectedGemstoneBlock(InfectionType.Crimson, GemType.Amethyst);
    public class DiamondCrimstoneBlock() : InfectedGemstoneBlock(InfectionType.Crimson, GemType.Diamond);
    public class AmberCrimstoneBlock() : InfectedGemstoneBlock(InfectionType.Crimson, GemType.Amber);

    public class SapphirePearlstoneBlock() : InfectedGemstoneBlock(InfectionType.Hallowed, GemType.Sapphire);
    public class RubyPearlstoneBlock() : InfectedGemstoneBlock(InfectionType.Hallowed, GemType.Ruby);
    public class EmeraldPearlstoneBlock() : InfectedGemstoneBlock(InfectionType.Hallowed, GemType.Emerald);
    public class TopazPearlstoneBlock() : InfectedGemstoneBlock(InfectionType.Hallowed, GemType.Topaz);
    public class AmethystPearlstoneBlock() : InfectedGemstoneBlock(InfectionType.Hallowed, GemType.Amethyst);
    public class DiamondPearlstoneBlock() : InfectedGemstoneBlock(InfectionType.Hallowed, GemType.Diamond);
    public class AmberPearlstoneBlock() : InfectedGemstoneBlock(InfectionType.Hallowed, GemType.Amber);

    public abstract class InfectedGemstoneBlock(InfectionType infectionType, GemType gemType) : ModItem
    {
        public override LocalizedText Tooltip => LocalizedText.Empty;

        public override string LocalizationCategory => "Items.Placables";

        private short InfectedStoneType => infectionType switch
        {
            InfectionType.Corrupt => ItemID.EbonstoneBlock,
            InfectionType.Crimson => ItemID.CrimstoneBlock,
            InfectionType.Hallowed => ItemID.PearlstoneBlock,
            _ => ItemID.StoneBlock
        };

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(InfectedQualitiesUtilities.GetGemstoneType(infectionType, gemType));
            ItemID.Sets.ExtractinatorMode[Type] = Type;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Main.GetItemDrawFrame(InfectedStoneType, out var itemTexture, out var itemFrame);
            spriteBatch.Draw(itemTexture, position, itemFrame, drawColor, 0, itemFrame.Size() / 2f, scale, SpriteEffects.None, 0);
            return true;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Main.GetItemDrawFrame(InfectedStoneType, out var itemTexture, out var itemFrame);
            Vector2 origin = itemFrame.Size() / 2f;
            Vector2 drawPosition = Item.Bottom - Main.screenPosition - new Vector2(0, origin.Y);

            spriteBatch.Draw(itemTexture, drawPosition, itemFrame, lightColor, rotation, origin, scale, SpriteEffects.None, 0);
            return true;
        }

        public override void ExtractinatorUse(int extractinatorBlockType, ref int resultType, ref int resultStack)
        {
            if (extractinatorBlockType == TileID.ChlorophyteExtractinator)
            {
                resultType = ItemID.Search.GetId(gemType.ToString() + "StoneBlock");
                resultStack = 1;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(InfectedStoneType)
                .AddIngredient(ItemID.Search.GetId(gemType.ToString()))
                .AddCondition(Condition.InGraveyard)
                .AddTile(TileID.HeavyWorkBench)
                .Register();
        }

        public override string Name => infectionType.ToString() + gemType.ToString() + "GemstoneBlock";

        public override string Texture => "InfectedQualities/Content/Extras/Items/" + gemType.ToString() + "_GemstoneBlock";

        public override bool IsLoadingEnabled(Mod mod) => ModContent.GetInstance<InfectedQualitiesServerConfig>().InfectedGemstones;
    }
}
