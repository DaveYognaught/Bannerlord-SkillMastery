using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu;
using System;

namespace SkillMastery
{
    public class SubModule : MBSubModuleBase
    {
        private Harmony _harmony;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            // Log to console/logs for debugging
            Console.WriteLine("SkillMastery: OnSubModuleLoad called");

            // Patch with Harmony
            _harmony = new Harmony("com.skillmastery.perkunlocker");
            _harmony.PatchAll();
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
            InformationManager.DisplayMessage(new InformationMessage("SkillMastery: Game session started!"));
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();

            // Triggers once at game startup, when the main menu appears
            InformationManager.DisplayMessage(new InformationMessage("SkillMastery: Main menu loaded!"));
        }
    }
}