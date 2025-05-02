using HarmonyLib;
using System;
using System.Linq;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SkillMastery
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            InformationManager.DisplayMessage(new InformationMessage("SkillMastery Loaded."));

            _harmony = new Harmony("com.skillmastery.perkunlocker");
            _harmony.PatchAll();

            foreach (var m in Harmony.GetAllPatchedMethods())
            {
                var owner = Harmony.GetPatchInfo(m)?.Owners?.FirstOrDefault();
                if (owner == "com.skillmastery.perkunlocker")
                    InformationManager.DisplayMessage(new InformationMessage($"SkillMastery patched: {m.DeclaringType.FullName}.{m.Name}"));
            }
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            _harmony?.UnpatchAll("com.skillmastery.perkunlocker");
        }
    }
}