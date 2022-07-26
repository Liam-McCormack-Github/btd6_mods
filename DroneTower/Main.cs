using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using MelonLoader;
using Main = DroneTower.Main;

[assembly: MelonInfo(typeof(Main), "Attack Drone Tower", "1.0.0", "TheWaWPro")]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]
namespace DroneTower
{
    public class Main : BloonsTD6Mod
    {
        public override void OnApplicationStart()
        {
            LoggerInstance.Msg("Drone Tower mod loaded");
        }
    }
}