using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;

namespace SkillMastery
{
    public class SkillMasterySettings : AttributeGlobalSettings<SkillMasterySettings>
    {
        public override string Id => "SkillMasterySettings_v1";
        public override string DisplayName => "Skill Mastery";
        public override string FolderName => "SkillMastery";
        public override string FormatType => "json";


        [SettingPropertyInteger(
            "Mastery Perk Cost Offset",
            1,
            13,
            RequireRestart = false, 
            HintText = "Determines the additional level(s) required to unlock mastery perks. (Default: 5)\nDo not increase this unless you decrease the starting level. You may cause Perks to become become unavailable\nif they exceed 330. If Starting Level Offset is default (0), then I cap Trade's Perk Cost Offset to a max of 2.",
            Order = 0
        )]
        [SettingPropertyGroup("Mastery Settings")]
        public int MasteryLevelOffset { get; set; } = 5;

        
        [SettingPropertyInteger(
            "Mastery Starting Level Offset",
            -75,
            0,
            RequireRestart = false,
            HintText = "This will make the Mastery Perks become available sooner. Recommended that if\nyou do use this then you also increase the Perk Cost offset too. (Default: 0)",
            Order = 0
        )]
        [SettingPropertyGroup("Mastery Settings")]
        public int MasteryStartingLevelOffset { get; set; } = 0;

        
        [SettingPropertyBool("Show Mastery Message(s)", RequireRestart = false, HintText = "Displays a message when a mastery perk has been unlocked and when fully mastered.\nThis was implemented mainly so there's more feedback it's working! (Default: True)", Order = 2)]
        [SettingPropertyGroup("Mastery Settings")]
        public bool ShowMessages { get; set; } = true;

        
        [SettingPropertyBool("Allow Companions to Master Skills", RequireRestart = false, HintText = "If disabled, only your main character can unlock mastery perks. Otherwise,\nit allows companions and other characters to use Skill Mastery. (Default: False)", Order = 3)]
        [SettingPropertyGroup("Mastery Settings")]
        public bool AllowAICompanions { get; set; } = false;


        [SettingPropertyBool("Auto-assign Mastery Perks - Companions", RequireRestart = false, HintText = "If enabled, when you go to the character screen any available Mastery Perks \nfor companions will automatically be acquired. (Default: True)", Order = 4)]
        [SettingPropertyGroup("Mastery Settings")]
        public bool AutoAssignCompanions { get; set; } = true;


        [SettingPropertyBool("Auto-assign Mastery Perks - Player", RequireRestart = false, HintText = "If enabled, when you go to the character screen any available Mastery Perks \nfor the Player will be automatically acquired. (Default: False)", Order = 5)]
        [SettingPropertyGroup("Mastery Settings")]
        public bool AutoAssignPlayer { get; set; } = false;
    }
}