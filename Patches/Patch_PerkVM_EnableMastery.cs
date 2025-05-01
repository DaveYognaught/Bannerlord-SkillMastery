using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace SkillMastery.Patches
{
    [HarmonyPatch(typeof(PerkVM), "RefreshState")]
    public static class Patch_PerkVM_EnableMastery
    {
        static void Prefix(PerkVM __instance)
        {
            // Grab the perk and the player hero
            var perk = __instance.Perk;
            var hero = Hero.MainHero;
            if (hero == null || perk == null || perk.AlternativePerk == null)
            {
                InformationManager.DisplayMessage(new InformationMessage("Hero or Perk or AlternativePerk is null!"));
                return;
            }

            // Only if the player has learned the sibling perk...
            if (!hero.GetPerkValue(perk.AlternativePerk))
            {
                InformationManager.DisplayMessage(new InformationMessage($"Player has not learned alternative perk {perk.AlternativePerk.Name}!"));
                return;
            }

            // Compute mastery threshold
            int idx = (int)(perk.RequiredSkillValue / 25f);
            int cap = perk.Skill == DefaultSkills.Trade ? 300
                         : perk.Skill == DefaultSkills.TwoHanded ? 250
                         : 275;
            int required = cap + (idx * Settings.SkillMasteryLevelOffset);

            // Log what the threshold is
            InformationManager.DisplayMessage(new InformationMessage($"[RefreshState] Perk: {perk.Name}, Skill: {perk.Skill.Name}, Required: {required}, Player Skill: {hero.GetSkillValue(perk.Skill)}"));

            // If player meets or exceeds it, flip _isAvailable on
            if (hero.GetSkillValue(perk.Skill) >= required)
            {
                InformationManager.DisplayMessage(new InformationMessage($"[RefreshState] Player meets the threshold! Making Perk clickable for {perk.Name}."));
                AccessTools.Field(typeof(PerkVM), "_isAvailable")
                           .SetValue(__instance, true);
            }
            else
            {
                InformationManager.DisplayMessage(new InformationMessage($"[RefreshState] Player does not meet the threshold for {perk.Name}."));
            }
        }
    }
}