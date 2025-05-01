using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

namespace SkillMastery.Patches
{
    [HarmonyPatch(typeof(HeroDeveloper), "GetPerkValue")]
    public static class Patch_GetPerkValue
    {
        static void Postfix(HeroDeveloper __instance, PerkObject perk, ref bool __result)
        {
            if (__result) return;

            var hero = __instance.Hero;
            if (hero == null || perk.AlternativePerk == null)
                return;

            // Check if the alternative perk is already learned
            if (hero.GetPerkValue(perk.AlternativePerk))
            {
                var skill = perk.Skill;
                int perkIndex = (int)(perk.RequiredSkillValue / 25f);
                int maxSkill = GetSkillCap(skill);
                int required = maxSkill + (perkIndex + 1) * Settings.SkillMasteryLevelOffset;

                if (hero.GetSkillValue(skill) >= required)
                {
                    __result = true;
                }
            }

            int GetSkillCap(SkillObject skill)
            {
                if (skill == DefaultSkills.Trade)
                    return 300;
                else if (skill == DefaultSkills.TwoHanded)
                    return 250;
                else
                    return 275;
            }
        }
    }
}