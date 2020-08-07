using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;

using HarmonyLib;

namespace HS2_UnlockPlayerHClothes
{
    public static class Transpilers
    {
        [HarmonyTranspiler, HarmonyPatch(typeof(HSceneSpriteChaChoice), "ChangeChaOptions")]
        public static IEnumerable<CodeInstruction> HSceneSpriteChaChoice_ChangeChaOptions_AllowMalesClothesCategory(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
            
            // Ignore turning off males selections
            var index = il.FindLastIndex(instruction => instruction.opcode == OpCodes.Call && (instruction.operand as MethodInfo)?.Name == "IsNullOrEmpty");
            if (index <= 0)
            {
                HS2_UnlockPlayerHClothes.Logger.LogMessage("Failed transpiling 'HSceneSpriteChaChoice_ChangeChaOptions_AllowMalesClothesCategory' IsNullOrEmpty index not found!");
                HS2_UnlockPlayerHClothes.Logger.LogWarning("Failed transpiling 'HSceneSpriteChaChoice_ChangeChaOptions_AllowMalesClothesCategory' IsNullOrEmpty index not found!");
                return il;
            }

            il[index - 14].opcode = OpCodes.Nop;
            il[index - 13].opcode = OpCodes.Nop;
            il[index - 12].opcode = OpCodes.Nop;
            
            for (var i = 8; i <= 17; i++)
                il[index + i].opcode = OpCodes.Nop;

            return il;
        }
        /*
        [HarmonyTranspiler, HarmonyPatch(typeof(HSceneSpriteChaChoice), "Init")]
        public static IEnumerable<CodeInstruction> HSceneSpriteChaChoice_Init_AllowMalesClothesCategory(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
            
            // Ignore futanari check and show males anyway
            var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt && (instruction.operand as MethodInfo)?.Name == "get_futanari");
            if (index <= 0)
            {
                HS2_UnlockPlayerHClothes.Logger.LogMessage("Failed transpiling 'HSceneSpriteChaChoice_Init_AllowMalesClothesCategory' get_futanari index not found!");
                HS2_UnlockPlayerHClothes.Logger.LogWarning("Failed transpiling 'HSceneSpriteChaChoice_Init_AllowMalesClothesCategory' get_futanari index not found!");
                return il;
            }

            for (var i = 0; i <= 7; i++)
                il[index - i].opcode = OpCodes.Nop;

            return il;
        }
        */
        [HarmonyTranspiler, HarmonyPatch(typeof(HDropdownCharChoiceTemplate), "CheckCha")]
        public static IEnumerable<CodeInstruction> HDropdownCharChoiceTemplate_CheckCha_AllowMalesClothesCategory(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
            
            // Ignore futanari check and show males anyway
            var items = il.FindAll(instruction => instruction.opcode == OpCodes.Callvirt && (instruction.operand as MethodInfo)?.Name == "get_futanari");
            
            foreach (var index in items.Select(item => il.IndexOf(item)).ToList())
            {
                if (index <= 0)
                {
                    HS2_UnlockPlayerHClothes.Logger.LogMessage("Failed transpiling 'HDropdownCharChoiceTemplate_CheckCha_AllowMalesClothesCategory' get_futanari index not found!");
                    HS2_UnlockPlayerHClothes.Logger.LogWarning("Failed transpiling 'HDropdownCharChoiceTemplate_CheckCha_AllowMalesClothesCategory' get_futanari index not found!");
                    return il;
                }

                for (var i = 0; i <= 15; i++)
                    il[1 + index - i].opcode = OpCodes.Nop;
            }

            return il;
        }
        
        [HarmonyTranspiler, HarmonyPatch(typeof(HSceneSprite), "OnClickCloth")]
        public static IEnumerable<CodeInstruction> HSceneSprite_OnClickCloth_AllowMalesClothesCategory(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
            
            // Force don't disable male buttons
            var index = il.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt && (instruction.operand as MethodInfo)?.Name == "SetMale");
            if (index <= 0)
            {
                HS2_UnlockPlayerHClothes.Logger.LogMessage("Failed transpiling 'HSceneSprite_OnClickCloth_AllowMalesClothesCategory' SetMale index not found!");
                HS2_UnlockPlayerHClothes.Logger.LogWarning("Failed transpiling 'HSceneSprite_OnClickCloth_AllowMalesClothesCategory' SetMale index not found!");
                return il;
            }

            il[index - 1].opcode = OpCodes.Ldc_I4_1;

            return il;
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
    }
}