using HarmonyLib;
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
        }


        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            _harmony?.UnpatchAll("com.skillmastery.perkunlocker");
        }
    }
}