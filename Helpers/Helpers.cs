using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterDeveloper;
using System;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.CampaignSystem.CharacterDevelopment;

namespace SkillMastery.Helpers
{
    public static class SkillMasteryHelper
    {
        internal static Hero GetContextHero(PerkVM vm)
        {
            var del = (Func<PerkObject, bool>)AccessTools
                .Field(typeof(PerkVM), "_getIsPerkSelected")
                .GetValue(vm);

            if (del?.Target == null)
                return null;

            // Cast to SkillVM
            var skillVM = del.Target;
            var skillVMType = skillVM.GetType();

            // Access the private _developerVM field
            var devField = skillVMType.GetField("_developerVM", BindingFlags.NonPublic | BindingFlags.Instance);
            if (devField == null)
            {
                InformationManager.DisplayMessage(new InformationMessage("Could not find _developerVM field"));
                return null;
            }

            var devVM = devField.GetValue(skillVM);
            if (devVM == null)
            {
                InformationManager.DisplayMessage(new InformationMessage("_developerVM was null"));
                return null;
            }

            // Access Hero property from CharacterDeveloperVM
            var heroProp = devVM.GetType().GetProperty("Hero", BindingFlags.Public | BindingFlags.Instance);
            var hero = heroProp?.GetValue(devVM) as Hero;

            InformationManager.DisplayMessage(new InformationMessage($"Context Hero: {hero?.Name}"));

            return hero;
        }
    }
}