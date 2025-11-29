using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using System.Collections.Generic;
using System.Reflection;

namespace SkillMastery.Patches
{
    [HarmonyPatch(typeof(SkillVM), "RefreshNumOfUnopenedPerks")]
    public static class Patch_SkillVM_RefreshNumOfUnopenedPerks
    {
        static void Postfix(SkillVM __instance)
        {
            int num = 0;
            var perks = __instance.Perks;

            HashSet<int> countedCosts = new HashSet<int>();
            foreach (PerkVM perk in perks)
            {
                if ((perk.CurrentState == PerkVM.PerkStates.EarnedButNotSelected
                  || perk.CurrentState == PerkVM.PerkStates.EarnedPreviousPerkNotSelected)
                  && !countedCosts.Contains((int)perk.Perk.RequiredSkillValue))
                {
                    countedCosts.Add((int)perk.Perk.RequiredSkillValue);
                    num++;
                }
            }

            // Set private NumOfUnopenedPerks field
            var numField = typeof(SkillVM).GetField("_numOfUnopenedPerks", BindingFlags.NonPublic | BindingFlags.Instance);
            numField?.SetValue(__instance, num);

            // And possibly update bound property too
            var prop = typeof(SkillVM).GetProperty("NumOfUnopenedPerks");
            if (prop != null && prop.CanWrite)
                prop.SetValue(__instance, num);
        }
    }
}
