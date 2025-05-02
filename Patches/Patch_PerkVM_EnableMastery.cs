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
                return;

            // Only if the player has learned the sibling perk...
            if (!hero.GetPerkValue(perk.AlternativePerk))
                return;

            // Compute mastery threshold
            int idx = (int)(perk.RequiredSkillValue / 25f);
            int cap = perk.Skill == DefaultSkills.Trade ? 300
                         : perk.Skill == DefaultSkills.TwoHanded ? 250
                         : 275;
            int required = cap + (idx * Settings.SkillMasteryLevelOffset);

            // Check if the player's skill is greater than or equal to the required level
            int playerSkill = hero.GetSkillValue(perk.Skill);

            // Log only when availability is changed
            bool isAvailableNow = playerSkill >= required;

            // Log when the perk's availability changes (either becomes available or unavailable)
            if (isAvailableNow && !GetIsAvailable(__instance))
            {
                InformationManager.DisplayMessage(new InformationMessage($"[RefreshState] Perk {perk.Name} became available (Player Skill: {playerSkill}, Required: {required})"));
                SetIsAvailable(__instance, true);
            }
            else if (!isAvailableNow && GetIsAvailable(__instance))
            {
                InformationManager.DisplayMessage(new InformationMessage($"[RefreshState] Perk {perk.Name} is no longer available (Player Skill: {playerSkill}, Required: {required})"));
                SetIsAvailable(__instance, false);
            }

            // Log when player skill exceeds max, but the perk is not available
            if (playerSkill > cap && !isAvailableNow)
            {
                InformationManager.DisplayMessage(new InformationMessage($"[RefreshState] Player skill for {perk.Name} is above max (Player Skill: {playerSkill}, Cap: {cap}), but perk is not available."));
            }

            // If the player skill exceeds cap but the perk doesn't become visible, log the failure
            if (playerSkill > cap && !isAvailableNow)
            {
                InformationManager.DisplayMessage(new InformationMessage($"[RefreshState] {perk.Name} was expected to be visible, but requirements aren't met yet (Player Skill: {playerSkill}, Cap: {cap})"));
            }
        }

        // Helper methods to interact with the internal _isAvailable field (since it's not directly exposed)
        private static bool GetIsAvailable(PerkVM perkVM)
        {
            return (bool)AccessTools.Field(typeof(PerkVM), "_isAvailable").GetValue(perkVM);
        }

        private static void SetIsAvailable(PerkVM perkVM, bool value)
        {
            AccessTools.Field(typeof(PerkVM), "_isAvailable").SetValue(perkVM, value);
        }
    }
}
