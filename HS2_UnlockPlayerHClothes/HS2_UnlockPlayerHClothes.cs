using HarmonyLib;

using BepInEx;
using BepInEx.Logging;
using BepInEx.Harmony;

using System.Collections.Generic;
using System.Linq;

using AIChara;

namespace HS2_UnlockPlayerHClothes {
    [BepInProcess("HoneySelect2")]
    [BepInPlugin(nameof(HS2_UnlockPlayerHClothes), nameof(HS2_UnlockPlayerHClothes), VERSION)]
    public class HS2_UnlockPlayerHClothes : BaseUnityPlugin
    {
        public const string VERSION = "1.4.0";
        
        public new static ManualLogSource Logger;

        private static HScene hScene;
        private static readonly List<int> clothesKindList = new List<int>{0, 2, 4, 1, 3, 5, 6};

        private void Awake()
        {
            Logger = base.Logger;

            HarmonyWrapper.PatchAll(typeof(Transpilers));
            HarmonyWrapper.PatchAll(typeof(HS2_UnlockPlayerHClothes));
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

        [HarmonyPostfix, HarmonyPatch(typeof(HScene), "SetStartVoice")]
        public static void HScene_SetStartVoice_ApplyClothesConfig(HScene __instance)
        {
            hScene = __instance;
            
            var males = hScene.GetMales();
            if (males[0] == null)
                return;
            
            var hData = Manager.Config.HData;
            
            foreach (var kind in clothesKindList.Where(kind => males[0].IsClothesStateKind(kind)))
                males[0].SetClothesState(kind, (byte)(hData.Cloth ? 0 : 2));
            
            males[0].SetAccessoryStateAll(hData.Accessory);
            males[0].SetClothesState(7, (byte)(!hData.Shoes ? 2 : 0));

            if (males[1] == null)
                return;
            
            foreach (var kind in clothesKindList.Where(kind => males[1].IsClothesStateKind(kind)))
                males[1].SetClothesState(kind, (byte)(hData.SecondCloth ? 0 : 2));
            
            males[1].SetAccessoryStateAll(hData.SecondAccessory);
            males[1].SetClothesState(7, (byte)(!hData.SecondShoes ? 2 : 0));
        }
    }
}
