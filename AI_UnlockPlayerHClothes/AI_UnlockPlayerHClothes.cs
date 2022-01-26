using HarmonyLib;

using BepInEx;
using BepInEx.Logging;

using AIChara;

namespace AI_UnlockPlayerHClothes {
    [BepInProcess("AI-Syoujyo")]
    [BepInPlugin(nameof(AI_UnlockPlayerHClothes), nameof(AI_UnlockPlayerHClothes), VERSION)]
    public class AI_UnlockPlayerHClothes : BaseUnityPlugin
    {
        public const string VERSION = "1.4.4";
        
        public new static ManualLogSource Logger;

        public static HScene hScene;

        private void Awake()
        {
            Logger = base.Logger;

            var harmony = new Harmony(nameof(AI_UnlockPlayerHClothes));
            harmony.PatchAll(typeof(Hooks));
        }

        public static ChaControl[] newGetFemales()
        {
            var females = hScene.GetFemales();
            var males = hScene.GetMales();

            var newFemales = new ChaControl[females.Length + males.Length];

            for (var i = 0; i < females.Length; i++)
                newFemales[i] = females[i];
            
            for (var i = 0; i < males.Length; i++)
                newFemales[i + females.Length] = males[i];

            return newFemales;
        }
    }
}