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
            var contextHero = SkillMasteryHelper.GetContextHero(__instance);
            if (contextHero != Hero.MainHero) return;

            var perk = __instance.Perk;
            var hero = Hero.MainHero;
            if (hero == null || perk == null || perk.AlternativePerk == null) return;
            if (!hero.GetPerkValue(perk.AlternativePerk)) return;

            int idx = (int)(perk.RequiredSkillValue / 25f);
            int cap = perk.Skill == DefaultSkills.Trade ? 300
                         : perk.Skill == DefaultSkills.TwoHanded ? 250
                         : 275;
            int required = 0;
            if (cap == 300)
            {
                if (SkillMasterySettings.Instance.MasteryLevelOffset > 2)
                {
                    required = cap + (idx * 2);
                }
                else
                {
                    required = cap + (idx * SkillMasterySettings.Instance.MasteryLevelOffset);
                }
            }
            else
            {
                required = cap + (idx * SkillMasterySettings.Instance.MasteryLevelOffset);
            }
            int playerSkill = hero.GetSkillValue(perk.Skill);

            if (playerSkill < required)
            {
                return;
            }

            // override vanilla “locked due to sibling” states to “earned-but-not-selected”
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
