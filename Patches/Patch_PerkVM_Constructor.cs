using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;
using static TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkVM;
using TaleWorlds.CampaignSystem;

namespace SkillMastery.Patches
{
    [HarmonyPatch(typeof(PerkVM))]
    // Patch the constructor overload:
    [HarmonyPatch(
        MethodType.Constructor,
        typeof(PerkObject), typeof(bool),
        typeof(PerkAlternativeType),
        typeof(Action<PerkVM>), typeof(Action<PerkVM>),
        typeof(Func<PerkObject, bool>),
        typeof(Func<PerkObject, bool>)
    )]
    public class Patch_PerkVM_Constructor
    {
        static void Postfix(PerkVM __instance)
        {
            // Grab the underlying perk and the hero
            var perk = __instance.Perk;
            var hero = Hero.MainHero;
            if (perk == null || hero == null) return;

            // Only override if the sibling perk is already learned
            if (perk.AlternativePerk != null && hero.GetPerkValue(perk.AlternativePerk))
            {
                // Compute your “mastery” requirement
                int perkIndex = (int)(perk.RequiredSkillValue / 25f);
                int maxSkill = GetSkillCap(perk.Skill);
                int newReq = maxSkill + (perkIndex + 1) * Settings.SkillMasteryLevelOffset;

                // Overwrite the VM fields
                __instance.Level = newReq;
                __instance.LevelText = newReq.ToString();
            }
        }

        private static int GetSkillCap(SkillObject skill)
        {
            if (skill == DefaultSkills.Trade) return 300;
            if (skill == DefaultSkills.TwoHanded) return 250;
            return 275;
        }
    }
}