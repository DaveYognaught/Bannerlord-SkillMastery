using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using System.Reflection;
using TaleWorlds.Library;
using SkillMastery.Helpers;

namespace SkillMastery.Patches
{
    [HarmonyPatch(typeof(PerkVM), "RefreshState")]
    public static class Patch_PerkVM_EnableMastery
    {
        static void Postfix(PerkVM __instance)
        {
            if (!SkillMasterySettings.Instance.AllowAICompanions)
            {
                //No AI allowed - Do a Main Hero check. Robots.txt their ass.
                var contextHero = SkillMasteryHelper.GetContextHero(__instance);
                if (contextHero != Hero.MainHero) return;
            }

            var perk = __instance.Perk;
            var hero = SkillMasteryHelper.GetContextHero(__instance);
            if (hero == null || perk == null || perk.AlternativePerk == null) return;
            if (!hero.GetPerkValue(perk) && !hero.GetPerkValue(perk.AlternativePerk)) return; // Skip only if neither perk in the pair is selected yet


            //!! CALCULATE PERK COST HERE !!//
            //!! CALCULATE PERK COST HERE !!//
            int idx = (int)(perk.RequiredSkillValue / 25f);

            int baseCap = perk.Skill == DefaultSkills.Trade ? 300
                         : perk.Skill == DefaultSkills.TwoHanded ? 250
                         : 275;

            int PerkCount = perk.Skill == DefaultSkills.Trade ? 11
                         : perk.Skill == DefaultSkills.TwoHanded ? 9
                         : 10;

            // Starting Level Offset — reduces the cap (only if negative)
            int startOffset = SkillMasterySettings.Instance.MasteryStartingLevelOffset;
            int cap = baseCap + startOffset;

            // Ensure Perks never exceeds 330
            int maxOffsetPerPerk = (330 - cap) / PerkCount;

            int safeOffset = SkillMasterySettings.Instance.MasteryLevelOffset;
            if (safeOffset > maxOffsetPerPerk)
                safeOffset = maxOffsetPerPerk;

            int required = cap + (idx * safeOffset);
            //!! END PERK COST CALCULATION !!//
            //!! END PERK COST CALCULATION !!//


            int playerSkill = hero.GetSkillValue(perk.Skill);

            if (playerSkill < required)
            {
                return;
            }

            // The Player / Companion qualifies for the Mastery Perk. Now, determine whether it's auto-assigned or clickable. 
            bool isMainHero = hero == Hero.MainHero;
            bool autoAssign = (isMainHero && SkillMasterySettings.Instance.AutoAssignPlayer) ||
                              (!isMainHero && SkillMasterySettings.Instance.AutoAssignCompanions);

            if (autoAssign)
            {
                // Auto-Assign is true. Assign the perk immediately. Won't show Master Messages. That's fine though I think. Would spam.
                // If it didn't spam per perk, I'd like to show a message of "Upgraded Companions" or "Upgraded Hero" but not sure how.

                var sf = typeof(PerkVM).GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
                var pf = typeof(PerkVM).GetField("_perkState", BindingFlags.NonPublic | BindingFlags.Instance);
                var dev = hero.HeroDeveloper;

                AccessTools.Method(typeof(HeroDeveloper), "AddPerk")
                           .Invoke(dev, new object[] { perk });

                // Optionally update the UI state immediately
                sf.SetValue(__instance, PerkVM.PerkStates.EarnedAndActive);
                pf.SetValue(__instance, (int)PerkVM.PerkStates.EarnedAndActive);

                return; // We've done our job yipee
            }
            else
            {
                // Auto-Assign is false. Show the perk as "earned-but-not-selected" in the UI instead to prompt clicking.

                var sf = typeof(PerkVM).GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
                var pf = typeof(PerkVM).GetField("_perkState", BindingFlags.NonPublic | BindingFlags.Instance);
                var current = (PerkVM.PerkStates)sf.GetValue(__instance);

                if (current == PerkVM.PerkStates.EarnedAndNotActive
                 || current == PerkVM.PerkStates.NotEarned
                 || current == PerkVM.PerkStates.EarnedPreviousPerkNotSelected)
                {
                    sf.SetValue(__instance, PerkVM.PerkStates.EarnedButNotSelected);
                    pf.SetValue(__instance, (int)PerkVM.PerkStates.EarnedButNotSelected);
                }
            }
        }
    }
}
