using HarmonyLib;
using SkillMastery;
using System;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;

[HarmonyPatch(typeof(PerkVM), "ExecuteStartSelection")]
public static class Patch_PerkVM_ExecuteStartSelection
{
    static bool Prefix(PerkVM __instance)
    {
        var perk = __instance.Perk;
        var hero = Hero.MainHero;
        if (hero == null || perk == null || perk.AlternativePerk == null)
            return true; // run original

        int idx = (int)(perk.RequiredSkillValue / 25f);
        int cap = perk.Skill == DefaultSkills.Trade ? 300
                 : perk.Skill == DefaultSkills.TwoHanded ? 250
                 : 275;
        int required = cap + (idx * Settings.SkillMasteryLevelOffset);

        bool hasMastery = hero.GetPerkValue(perk.AlternativePerk) && hero.GetSkillValue(perk.Skill) >= required;

        if (hasMastery)
        {
            var onStart = AccessTools.Field(typeof(PerkVM), "_onStartSelection").GetValue(__instance) as Action<PerkVM>;
            if (onStart != null)
            {
                onStart(__instance);
                InformationManager.DisplayMessage(new InformationMessage($"[ExecuteStartSelection] {perk.Name} trying to click..."));
                return false; // skip original logic
            }
        }

        return true; // fallback to original
    }
}
