using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace InfectedQualities.Core
{
    public class InfectedQualitiesClientConfig : ModConfig
    {
        public override LocalizedText DisplayName => Language.GetText("Mods.InfectedQualities.Config.Title.Main.Client");

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("$Mods.InfectedQualities.Config.Title.Visual")]
        [LabelKey("$Mods.InfectedQualities.Config.InfectedPlantera.Label"), TooltipKey("$Mods.InfectedQualities.Config.InfectedPlantera.Tooltip")]
        [BackgroundColor(45, 148, 185)]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool InfectedPlantera { get; set; }
    }

    [BackgroundColor(29, 11, 45, 200)]
    public class InfectedQualitiesServerConfig : ModConfig
    {
        public override LocalizedText DisplayName => Language.GetText("Mods.InfectedQualities.Config.Title.Main.Server");

        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("$Mods.InfectedQualities.Config.Title.Content")]
        [LabelKey("$Mods.InfectedQualities.Config.InfectedBiomes.Label"), TooltipKey("$Mods.InfectedQualities.Config.InfectedBiomes.Tooltip")]
        [BackgroundColor(135, 54, 206)]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool InfectedBiomes { get; set; }

        [LabelKey("$Mods.InfectedQualities.Config.PylonOfNight.Label"), TooltipKey("$Mods.InfectedQualities.Config.PylonOfNight.Tooltip")]
        [BackgroundColor(135, 54, 206)]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool PylonOfNight { get; set; }

        [LabelKey("$Mods.InfectedQualities.Config.KeyOfNaught.Label"), TooltipKey("$Mods.InfectedQualities.Config.KeyOfNaught.Tooltip")]
        [BackgroundColor(135, 54, 206)]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool KeyOfNaught { get; set; }

        [LabelKey("$Mods.InfectedQualities.Config.DivinePowder.Label"), TooltipKey("$Mods.InfectedQualities.Config.DivinePowder.Tooltip")]
        [BackgroundColor(135, 54, 206)]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool DivinePowder { get; set; }

        [Header("$Mods.InfectedQualities.Config.Title.Experimental")]
        [LabelKey("$Mods.InfectedQualities.Config.HardmodeChasmPurification.Label"), TooltipKey("$Mods.InfectedQualities.Config.HardmodeChasmPurification.Tooltip")]
        [BackgroundColor(185, 133, 45)]
        [DefaultValue(false)]
        public bool HardmodeChasmPurification { get; set; }

        [LabelKey("$Mods.InfectedQualities.Config.DisableInfectionSpread.Label"), TooltipKey("$Mods.InfectedQualities.Config.DisableInfectionSpread.Tooltip")]
        [BackgroundColor(185, 133, 45)]
        [DefaultValue(false)]
        public bool DisableInfectionSpread { get; set; }

        [LabelKey("$Mods.InfectedQualities.Config.AllowNPCsInEvilBiomes.Label"), TooltipKey("$Mods.InfectedQualities.Config.AllowNPCsInEvilBiomes.Tooltip")]
        [BackgroundColor(185, 133, 45)]
        [DefaultValue(false)]
        public bool AllowNPCsInEvilBiomes { get; set; }
    }
}
