using HarmonyLib;
using SkillMastery;
using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace SkillMastery.Patches
{
    [HarmonyPatch(typeof(PerkVM), "ExecuteStartSelection")]
    public static class Patch_PerkVM_ExecuteStartSelection
    {
        static bool Prefix(PerkVM __instance)
        {
            var perk = __instance.Perk;
            var hero = Hero.MainHero;
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
            int idx = (int)(perk.RequiredSkillValue / 25f);
            int cap = perk.Skill == DefaultSkills.Trade ? 300
                         : perk.Skill == DefaultSkills.TwoHanded ? 250
                         : 275;
            int required = cap + (idx * Settings.SkillMasteryLevelOffset);
            if (hero.GetSkillValue(perk.Skill) < required) return true;

            // 3) Grant it directly
            var dev = hero.HeroDeveloper;
            AccessTools.Method(typeof(HeroDeveloper), "AddPerk")
                       .Invoke(dev, new object[] { perk });

            // 4) Feedback (optional)
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

            // 5) Block further clicks
            AccessTools.Field(typeof(PerkVM), "_isInSelection")
                       .SetValue(__instance, false);
            __instance.RefreshState();

            return false;
        }
    }
}