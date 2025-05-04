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
            17,
            RequireRestart = false, 
            HintText = "Determines the additional level(s) required to unlock mastery perks. (Default: 17)\nNOTE: By default, it's not actually 17. It'll either be 8/5/2 for 250/275/300 Skills respectively.\nThis is due to Safeguard Logic that'll ensure perks are always affordable and below 330!",
            Order = 0
        )]
        [SettingPropertyGroup("Mastery Settings")]
        public int MasteryLevelOffset { get; set; } = 17;

        
        [SettingPropertyInteger(
            "Mastery Starting Level Offset",
            -75,
            0,
            RequireRestart = false,
            HintText = "This will make the Mastery Perks become available sooner. (Default: 0)\nIf you lower the Starting Level then recommended that you keep the default Perk Cost Offset at 17; as this will\nensure perks scale all the way up to 330 affordably and fairly.",
            Order = 1
        )]
        [SettingPropertyGroup("Mastery Settings")]
        public int MasteryStartingLevelOffset { get; set; } = 0;

        
        [SettingPropertyBool("Show Mastery Message(s)", RequireRestart = false, HintText = "Displays a message when a mastery perk has been unlocked and when fully mastered.\nThis was implemented mainly so there's more feedback it's working! (Default: True)", Order = 2)]
        [SettingPropertyGroup("Mastery Settings")]
        public bool ShowMessages { get; set; } = true;

        
        [SettingPropertyBool("Allow Companions to Master Skills", RequireRestart = false, HintText = "If disabled, only your main character can unlock mastery perks.\nOtherwise, it allows companions to also unlock Skill Mastery perks. (Default: True)\nNOTE: Enemy Lords / Neutral Heroes currently do NOT acquire Skill Mastery Perks", Order = 3)]
        [SettingPropertyGroup("Mastery Settings")]
        public bool AllowAICompanions { get; set; } = true;


        [SettingPropertyBool("Auto-assign Mastery Perks - Companions", RequireRestart = false, HintText = "If enabled, when you go to the character screen any available \nMastery Perks for companions will automatically be acquired. (Default: True)", Order = 4)]
        [SettingPropertyGroup("Mastery Settings")]
        public bool AutoAssignCompanions { get; set; } = true;


        [SettingPropertyBool("Auto-assign Mastery Perks - Player", RequireRestart = false, HintText = "If enabled, when you go to the character screen any available \nMastery Perks for the Player will be automatically acquired. (Default: False)", Order = 5)]
        [SettingPropertyGroup("Mastery Settings")]
        public bool AutoAssignPlayer { get; set; } = false;
    }
}