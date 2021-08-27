using System.Collections.Generic;
using HarmonyLib;
using BepInEx;

namespace KKS_UnlockPlayerHClothes 
{
    [BepInProcess("KoikatsuSunshine")]
    [BepInPlugin(nameof(KKS_UnlockPlayerHClothes), nameof(KKS_UnlockPlayerHClothes), VERSION)]
    public class KKS_UnlockPlayerHClothes : BaseUnityPlugin
    {
        public const string VERSION = "1.4.3";
        
        public static readonly Dictionary<int, string> extraMaleButtons = new Dictionary<int, string>()
        {
            {2, "Top"},
            {3, "Bottom"},
            {4, "Underwear"},
            {5, "Gloves"},
            {6, "Pantyhose"},
            {7, "Socks"},
        };

        private void Awake() => Harmony.CreateAndPatchAll(typeof(Hooks), nameof(KKS_UnlockPlayerHClothes));
    }
}