using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using System;
using System.Reflection;

namespace SkillMastery.Patches
{
    [HarmonyPatch(typeof(PerkVM), "RefreshState")]
    public static class Patch_PerkVM_EnableMastery
    {
        static void Postfix(PerkVM __instance)
        {
            var perk = __instance.Perk;
            var hero = Hero.MainHero;

            if (hero == null || perk == null || perk.AlternativePerk == null)
                return;

            if (!hero.GetPerkValue(perk.AlternativePerk))
                return;

            int idx = (int)(perk.RequiredSkillValue / 25f);
            int cap = perk.Skill == DefaultSkills.Trade ? 300
                     : perk.Skill == DefaultSkills.TwoHanded ? 250
                     : 275;
            int required = cap + (idx * Settings.SkillMasteryLevelOffset);
            int playerSkill = hero.GetSkillValue(perk.Skill);

            if (playerSkill < required)
            {
                InformationManager.DisplayMessage(new InformationMessage($"[RefreshState] {perk.Name} NOT available: {playerSkill}/{required}"));
                return;
            }

            // Force state: if it's been locked due to AltPerk, override it
            var stateField = typeof(PerkVM).GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            var perkStateField = typeof(PerkVM).GetField("_perkState", BindingFlags.NonPublic | BindingFlags.Instance);

            // Only override if it's not already an earned state
            var current = (PerkVM.PerkStates)stateField.GetValue(__instance);
            if (current == PerkVM.PerkStates.EarnedAndNotActive ||
                current == PerkVM.PerkStates.NotEarned ||
                current == PerkVM.PerkStates.EarnedPreviousPerkNotSelected)
            {
                InformationManager.DisplayMessage(new InformationMessage($"[RefreshState] Overriding Perk {perk.Name} to EarnedButNotSelected (Skill: {playerSkill}, Required: {required})"));

                stateField.SetValue(__instance, PerkVM.PerkStates.EarnedButNotSelected);
                perkStateField.SetValue(__instance, (int)PerkVM.PerkStates.EarnedButNotSelected);
            }
        }
    }
}
