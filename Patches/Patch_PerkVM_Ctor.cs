using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using TaleWorlds.Core;
using TaleWorlds.Library;
using System.Collections.Generic;
using SkillMastery.Helpers;
using System.Reflection;
using System.Linq;
using static TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper.PerkVM;
using static TaleWorlds.Core.ViewModelCollection.Information.TooltipProperty;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core.ViewModelCollection.Information;
using System;
using SkillMastery;
using TaleWorlds.CampaignSystem.ViewModelCollection;

namespace SkillMastery.Patches
{
    [HarmonyPatch(typeof(PerkVM))]
    [HarmonyPatch(MethodType.Constructor)]
    [HarmonyPatch(new[] {
        typeof(PerkObject),
        typeof(bool),
        typeof(PerkAlternativeType),
        typeof(System.Action<PerkVM>),
        typeof(System.Action<PerkVM>),
        typeof(System.Func<PerkObject, bool>),
        typeof(System.Func<PerkObject, bool>)
    })]
    public static class Patch_PerkVM_Ctor
    {
        static void Postfix(PerkVM __instance)
        {
            var perk = __instance.Perk;

            if (perk == null || perk.AlternativePerk == null) return;

            var hero = SkillMasteryHelper.GetContextHero(__instance);
            if (hero == null) return;

            bool altSelected = hero.GetPerkValue(perk.AlternativePerk);
            bool thisSelected = hero.GetPerkValue(perk);

            // Case 1: Neither selected — do nothing
            if (!altSelected && !thisSelected)
                return;

            // Case 3: This selected but not the alt — do nothing
            if (!altSelected && thisSelected)
                return;

            // Case 4: Both selected — do nothing
            if (altSelected && thisSelected)
                return;

            // Case 2: Alt selected, this not selected — continue
            // No return here, this is the only state we allow to proceed

            // Access the private _tooltip list inside the Hint
            var hintVM = __instance.Hint as BasicTooltipViewModel;
            if (hintVM == null) return;

            // Now, replace the lambda that generates the tooltip list
            var tooltipFuncField = typeof(BasicTooltipViewModel)
                .GetField("_tooltipProperties", BindingFlags.NonPublic | BindingFlags.Instance);
            if (tooltipFuncField == null) return;

            // Replace the lambda that creates the tooltip list with our own function
            tooltipFuncField.SetValue(hintVM, new Func<List<TooltipProperty>>(() =>
            {
                // If it got this far, it's because we've already qualified it as NOT being active. So hardcode as false.
                var originalList = CampaignUIHelper.GetPerkEffectText(perk, false);

                // Find and replace the 'Required Skill Value' tooltip entry
                for (int i = 0; i < originalList.Count; i++)
                {
                    var prop = originalList[i];
                    bool isValMatch = prop.ValueLabel == ((int)perk.RequiredSkillValue).ToString();
                    // Just matching Val now. Broader compatability for different languages.

                    if (i == originalList.Count - 1 && isValMatch)
                    {
                        //InformationManager.DisplayMessage(new InformationMessage($"Patch_PerkVM_Ctor perk: {perk.Name}"));
                        //InformationManager.DisplayMessage(new InformationMessage("Found it. Try replacing tooltip value !!!"));

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

                        // Replace the property in the list
                        prop.ValueLabel = required.ToString();
                        break;
                    }
                }

                // Return the modified list
                return originalList;
            }));
        }
    }
}
