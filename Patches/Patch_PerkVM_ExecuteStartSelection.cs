using HarmonyLib;
using SkillMastery;
using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using SkillMastery.Helpers;

namespace SkillMastery.Patches
{
    [HarmonyPatch(typeof(PerkVM), "ExecuteStartSelection")]
    public static class Patch_PerkVM_ExecuteStartSelection
    {
        static bool Prefix(PerkVM __instance)
        {
            if (!SkillMasterySettings.Instance.AllowAICompanions)
            {
                //No AI allowed - Do a Main Hero check. Robots.txt their ass.
                var contextHero = SkillMasteryHelper.GetContextHero(__instance);
                if (contextHero != Hero.MainHero) return true;
            }

            var perk = __instance.Perk;
            var hero = SkillMasteryHelper.GetContextHero(__instance);
            if (hero == null || perk == null || perk.AlternativePerk == null)
                return true;  // let vanilla handle it

            // 1) Already picked?
            var isSel = (Func<PerkObject, bool>)
                AccessTools.Field(typeof(PerkVM), "_getIsPerkSelected")
                           .GetValue(__instance);
            if (isSel(perk))
                return false; // swallow click

            // 2) Mastery conditions?
            if (!isSel(perk.AlternativePerk)) return true;


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


            if (hero.GetSkillValue(perk.Skill) < required) return true;

            // 3) Grant it directly
            var dev = hero.HeroDeveloper;
            AccessTools.Method(typeof(HeroDeveloper), "AddPerk")
                       .Invoke(dev, new object[] { perk });

            // 4) Feedback (optional)
            if(SkillMasterySettings.Instance.ShowMessages)
            {
                var maxTier = PerkObject.All
                  .Where(p => p.Skill == perk.Skill && p.AlternativePerk != null)
                  .Max(p => p.RequiredSkillValue);
                bool isFinal = Math.Abs(perk.RequiredSkillValue - maxTier) < 0.001f;
                if (isFinal)
                    InformationManager.DisplayMessage(new InformationMessage($"You are a master of {perk.Skill.Name}!"));
                else
                {
                    InformationManager.DisplayMessage(new InformationMessage($"Your skill mastery grows..."));
                }
            }

            // 5) Block further clicks
            AccessTools.Field(typeof(PerkVM), "_isInSelection")
                       .SetValue(__instance, false);
            __instance.RefreshState();

            return false;
        }
    }
}