using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;

using HarmonyLib;

namespace HS2_UnlockPlayerHClothes
{
    public static class Hooks
    {
        private static readonly List<int> clothesKindList = new List<int>{0, 2, 4, 1, 3, 5, 6};
        
        [HarmonyPostfix, HarmonyPatch(typeof(HScene), "SetStartVoice")]
        public static void HScene_SetStartVoice_ApplyClothesConfig(HScene __instance)
        {
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
                    males[1].SetClothesState(kind, (byte)(hData.SecondCloth ? 0 : 2));
            
                males[1].SetAccessoryStateAll(hData.SecondAccessory);
                males[1].SetClothesState(7, (byte)(!hData.SecondShoes ? 2 : 0));
            }
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(HSceneSpriteChaChoice), "ChangeChaOptions")]
        public static bool HSceneSpriteChaChoice_ChangeChaOptions_Patch(HScene.AnimationListInfo info, HSceneSpriteChaChoice __instance, UnityEngine.UI.Dropdown ___dropdown, Manager.HSceneManager ___hSceneManager)
        {
            __instance.FeMaleActive[0] = !info.fileFemale.IsNullOrEmpty();
            __instance.FeMaleActive[1] = !info.fileFemale2.IsNullOrEmpty();
            __instance.MaleActive[0] = !info.fileMale.IsNullOrEmpty();
            __instance.MaleActive[1] = !info.fileMale2.IsNullOrEmpty();
            
            if (___hSceneManager.numFemaleClothCustom == 0 || (___hSceneManager.numFemaleClothCustom < 2) ? __instance.FeMaleActive[___hSceneManager.numFemaleClothCustom] : __instance.MaleActive[___hSceneManager.numFemaleClothCustom - 2]) 
                return false;
            
            ___hSceneManager.numFemaleClothCustom = 0;
            ___dropdown.value = 0;

            return false;
        }

        [HarmonyTranspiler, HarmonyPatch(typeof(HScene), "LateUpdate")]
        public static IEnumerable<CodeInstruction> HScene_LateUpdate_RemoveClothesLock(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();

            var startindex = il.FindIndex(instruction => instruction.opcode == OpCodes.Call && (instruction.operand as MethodInfo)?.Name == "get_HData");
            if (startindex <= 0)
            {
                HS2_UnlockPlayerHClothes.Logger.LogMessage("Failed transpiling 'HScene_LateUpdate_RemoveClothesLock' get_HData index not found!");
                HS2_UnlockPlayerHClothes.Logger.LogWarning("Failed transpiling 'HScene_LateUpdate_RemoveClothesLock' get_HData index not found!");
                return il;
            }
            
            var endindex = il.FindIndex(instruction => instruction.opcode == OpCodes.Ldfld && (instruction.operand as FieldInfo)?.Name == "ctrlFlag");
            if (endindex <= 0)
            {
                HS2_UnlockPlayerHClothes.Logger.LogMessage("Failed transpiling 'HScene_LateUpdate_RemoveClothesLock' ctrlFlag index not found!");
                HS2_UnlockPlayerHClothes.Logger.LogWarning("Failed transpiling 'HScene_LateUpdate_RemoveClothesLock' ctrlFlag index not found!");
                return il;
            }
            
            for (var i = startindex; i <= endindex - 2; i++)
                il[i].opcode = OpCodes.Nop;

            return il;
        }
        
        [HarmonyPrefix, HarmonyPatch(typeof(HSceneSpriteChaChoice), "SetMale")]
        public static bool HSceneSpriteChaChoice_SetMale_Patch() => false;
    }
}