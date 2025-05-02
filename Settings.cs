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
            "Mastery Level Offset",
            1,
            5,
            RequireRestart = false, 
            HintText = "Determines the additional level(s) required to unlock mastery perks. NOTE: Trade caps at 2.",
            Order = 0
        )]
        [SettingPropertyGroup("Mastery Settings")]
        public int MasteryLevelOffset { get; set; } = 5;

        [SettingPropertyBool("Show Mastery Message(s)", RequireRestart = false, HintText = "Displays a message when a mastery perk has been unlocked and when fully mastered. This was implemented mainly so there's more feedback it's working!", Order = 2)]
        [SettingPropertyGroup("Mastery Settings")]
        public bool ShowMessages { get; set; } = true;

    }
}