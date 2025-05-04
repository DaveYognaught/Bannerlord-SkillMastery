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

        // !! Skill Mastery Global Settings !! //
        // !! Skill Mastery Global Settings !! //
        [SettingPropertyBool("Show Mastery Message(s)", RequireRestart = false, HintText = "Displays a message when a mastery perk is unlocked manually and if fully mastered.\nThis was implemented mainly so there's more on-click feedback! (Default: True)", 
            Order = 0)]
        [SettingPropertyGroup("Global Settings", GroupOrder = 0)]
        public bool ShowMessages { get; set; } = true;



        // !! Player Specific Settings !! //
        // !! Player Specific Settings !! //
        [SettingPropertyInteger(
            "Mastery Perk Cost Offset",
            1,
            20,
            RequireRestart = false, 
            HintText = "Determines the additional level(s) required to unlock Player Mastery Perks.\nNOTE: By default, it's not actually 20. It'll either be 8/5/2 for 250/275/300 Skills respectively.\nThis is due to Safeguard Logic that'll ensure perks are always affordable and below 330! (Default: 20)",
            Order = 1
        )]
        [SettingPropertyGroup("Player Settings", GroupOrder = 1)]
        public int PlayerMasteryLevelOffset { get; set; } = 20;

        
        [SettingPropertyInteger(
            "Mastery Starting Level Offset",
            -100,
            0,
            RequireRestart = false,
            HintText = "This will make the Player Mastery Perks become available sooner.\nIf you lower the Starting Level then recommended that you keep the default Perk Cost Offset at 20; as this will\nensure perks scale all the way up to 330 affordably and fairly. (Default: 0)",
            Order = 2
        )]
        [SettingPropertyGroup("Player Settings")]
        public int PlayerMasteryStartingLevelOffset { get; set; } = 0;


        [SettingPropertyBool("Auto-assign Mastery Perks", RequireRestart = false, HintText = "If enabled, when you go to the character screen any available \nMastery Perks for the Player will be automatically acquired. (Default: False)",
            Order = 3)]
        [SettingPropertyGroup("Player Settings")]
        public bool AutoAssignPlayer { get; set; } = false;



        // !! Companion Specific Settings !! //
        // !! Companion Specific Settings !! //
        [SettingPropertyInteger(
            "Mastery Perk Cost Offset",
            1,
            20,
            RequireRestart = false, 
            HintText = "Determines the additional level(s) required to unlock Companion Mastery Perks.\nNOTE: By default, it's not actually 20. It'll either be 8/5/2 for 250/275/300 Skills respectively.\nThis is due to Safeguard Logic that'll ensure perks are always affordable and below 330! (Default: 20)",
            Order = 1
        )]
        [SettingPropertyGroup("Companion Settings", GroupOrder = 2)]
        public int CompanionMasteryLevelOffset { get; set; } = 20;

        
        [SettingPropertyInteger(
            "Mastery Starting Level Offset",
            -100,
            0,
            RequireRestart = false,
            HintText = "This will make the Companion Mastery Perks become available sooner.\nIf you lower the Starting Level then recommended that you keep the default Perk Cost Offset at 20; as this will\nensure perks scale all the way up to 330 affordably and fairly. (Default: 0)",
            Order = 2
        )]
        [SettingPropertyGroup("Companion Settings")]
        public int CompanionMasteryStartingLevelOffset { get; set; } = 0;


        [SettingPropertyBool("Allow Companions to Master Skills", RequireRestart = false, HintText = "If disabled, only your main character can unlock mastery perks.\nOtherwise, it allows companions to also unlock Mastery Perks. (Default: True)\nNOTE: Enemy Lords / Neutral Heroes currently do NOT acquire Skill Mastery Perks",
            Order = 4)]
        [SettingPropertyGroup("Companion Settings")]
        public bool AllowAICompanions { get; set; } = true;


        [SettingPropertyBool("Auto-assign Mastery Perks", RequireRestart = false, HintText = "If enabled, when you go to the character screen any available \nMastery Perks for companions will automatically be acquired. (Default: True)",
            Order = 5)]
        [SettingPropertyGroup("Companion Settings")]
        public bool AutoAssignCompanions { get; set; } = true;
    }
}