using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Library;
using System.Linq;
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

            foreach (PerkVM perk in perks)
            {
                if (perk.CurrentState == PerkVM.PerkStates.EarnedButNotSelected
                 || perk.CurrentState == PerkVM.PerkStates.EarnedPreviousPerkNotSelected)
                {
                    // Count all unopened perks regardless of their alternative type (0/1/2)
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
