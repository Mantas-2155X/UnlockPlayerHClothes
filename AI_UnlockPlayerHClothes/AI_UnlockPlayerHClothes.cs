using HarmonyLib;

using BepInEx;
using BepInEx.Logging;

using System.Collections.Generic;
using System.Linq;

using AIChara;

namespace AI_UnlockPlayerHClothes {
    [BepInProcess("AI-Syoujyo")]
    [BepInPlugin(nameof(AI_UnlockPlayerHClothes), nameof(AI_UnlockPlayerHClothes), VERSION)]
    public class AI_UnlockPlayerHClothes : BaseUnityPlugin
    {
        public const string VERSION = "1.4.1";
        
        public new static ManualLogSource Logger;

        private static HScene hScene;
        private static readonly List<int> clothesKindList = new List<int>{0, 2, 1, 3, 5, 6};

        private void Awake()
        {
            Logger = base.Logger;

            var harmony = new Harmony(nameof(AI_UnlockPlayerHClothes));
            harmony.PatchAll(typeof(Transpilers));
            harmony.PatchAll(typeof(AI_UnlockPlayerHClothes));
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

        // Read game config and apply clothes state for both males //
        [HarmonyPostfix, HarmonyPatch(typeof(HScene), "SetStartVoice")]
        public static void HScene_SetStartVoice_ApplyClothesConfig(HScene __instance)
        {
            hScene = __instance;
            
            var hData = Manager.Config.HData;
            var males = __instance.GetMales();

            if (males[0] != null)
            {
                foreach (var kind in clothesKindList.Where(kind => males[0].IsClothesStateKind(kind)))
                    males[0].SetClothesState(kind, (byte)(hData.Cloth ? 0 : 2));
            
                males[0].SetAccessoryStateAll(hData.Accessory);
                males[0].SetClothesState(7, (byte)(!hData.Shoes ? 2 : 0));
            }
            
            if (males[1] != null)
            {
                foreach (var kind in clothesKindList.Where(kind => males[1].IsClothesStateKind(kind)))
                    males[1].SetClothesState(kind, (byte)(hData.Cloth ? 0 : 2));
            
                males[1].SetAccessoryStateAll(hData.Accessory);
                males[1].SetClothesState(7, (byte)(!hData.Shoes ? 2 : 0));
            }
        }
        
        // Allow 4 character choices instead of 2 //
        [HarmonyPrefix, HarmonyPatch(typeof(HSceneSpriteClothCondition), "Init")]
        public static void HSceneSpriteClothCondition_Init_IncreaseAllState(ref int[] ___allState) => ___allState = new int[4];
    }
}