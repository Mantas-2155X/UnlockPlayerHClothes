using HarmonyLib;

using BepInEx;
using BepInEx.Logging;

namespace HS2_UnlockPlayerHClothes 
{
    [BepInProcess("HoneySelect2")]
    [BepInPlugin(nameof(HS2_UnlockPlayerHClothes), nameof(HS2_UnlockPlayerHClothes), VERSION)]
    public class HS2_UnlockPlayerHClothes : BaseUnityPlugin
    {
        public const string VERSION = "1.4.3";
        
        public new static ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            
            Harmony.CreateAndPatchAll(typeof(Hooks), nameof(HS2_UnlockPlayerHClothes));
        }
    }
}
