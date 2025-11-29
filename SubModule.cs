using HarmonyLib;
using System.Linq;
using TaleWorlds.Core;
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
            InformationManager.DisplayMessage(new InformationMessage("SkillMastery Loaded (v1.3.7.1)"));

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

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            // Only show this once per game session
            InformationManager.DisplayMessage(new InformationMessage("SkillMastery Loaded (v1.3.7.1)"));
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            // Triggers once at game startup, when the main menu appears
            InformationManager.DisplayMessage(new InformationMessage("SkillMastery Loaded (v1.3.7.1)"));
        }
    }
}