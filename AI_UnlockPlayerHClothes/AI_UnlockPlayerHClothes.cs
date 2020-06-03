using HarmonyLib;

using BepInEx;
using BepInEx.Logging;
using BepInEx.Harmony;

using System.Collections.Generic;
using System.Linq;

using AIChara;
using Manager;

namespace AI_UnlockPlayerHClothes {
    [BepInProcess("AI-Syoujyo")]
    [BepInPlugin(nameof(AI_UnlockPlayerHClothes), nameof(AI_UnlockPlayerHClothes), VERSION)]
    public class AI_UnlockPlayerHClothes : BaseUnityPlugin
    {
        public const string VERSION = "1.4.0";
        
        public new static ManualLogSource Logger;

        private static HScene hScene;
        
        private static ChaControl player;
        private static readonly List<int> clothesKindList = new List<int>{0, 2, 1, 3, 5, 6};

        private void Awake()
        {
            Logger = base.Logger;

            HarmonyWrapper.PatchAll(typeof(Transpilers));
            HarmonyWrapper.PatchAll(typeof(AI_UnlockPlayerHClothes));
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

        [HarmonyPrefix, HarmonyPatch(typeof(HSceneSpriteClothCondition), "Init")]
        public static void HSceneSpriteClothCondition_Init_IncreaseAllState(HSceneSpriteClothCondition __instance)
        {
            // Force "all clothes off/on" to int array of 4 instead of 2
            var trav = Traverse.Create(__instance);
            trav.Field("allState").SetValue(new int[4]);
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(HScene), "SetStartVoice")]
        public static void HScene_SetStartVoice_ApplyClothesConfig(HScene __instance)
        {
            hScene = __instance;
            
            var traverse = Traverse.Create(hScene);
            var manager = traverse.Field("hSceneManager").GetValue<HSceneManager>();

            if(manager != null && manager.Player != null)
                player = manager.Player.ChaControl;

            var hData = Manager.Config.HData;
            foreach (var kind in clothesKindList.Where(kind => player.IsClothesStateKind(kind)))
                player.SetClothesState(kind, (byte)(hData.Cloth ? 0 : 2));
            
            player.SetAccessoryStateAll(hData.Accessory);
            player.SetClothesState(7, (byte)(!hData.Shoes ? 2 : 0));
        }
    }
}
