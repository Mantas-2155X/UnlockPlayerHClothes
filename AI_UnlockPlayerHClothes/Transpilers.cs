using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace AI_UnlockPlayerHClothes
{
    public class Transpilers
    {
        [HarmonyTranspiler, HarmonyPatch(typeof(HSceneSprite), "OnClickMainCategories")]
        public static IEnumerable<CodeInstruction> HSceneSprite_OnClickMainCategories_AllowMalesClothesCategory(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
            
            // Force show males selection in clothes category
            var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Call && (instruction.operand as MethodInfo)?.Name == "SetAnimationMenu");
            if (index <= 0)
            {
                AI_UnlockPlayerHClothes.Logger.LogMessage("Failed transpiling 'HSceneSprite_OnClickMainCategories_AllowMalesClothesCategory' SetAnimationMenu index not found!");
                AI_UnlockPlayerHClothes.Logger.LogWarning("Failed transpiling 'HSceneSprite_OnClickMainCategories_AllowMalesClothesCategory' SetAnimationMenu index not found!");
                return il;
            }

            il[index + 5].opcode = OpCodes.Ldc_I4_2;
            il[index + 6].opcode = OpCodes.Clt;

           return il;
        }
        
        [HarmonyTranspiler, HarmonyPatch(typeof(HSceneSpriteClothCondition), "Init")]
        public static IEnumerable<CodeInstruction> HSceneSpriteClothCondition_Init_RedirectGetFemales(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
            
            // Force clothes category to put males in "females" array
            var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt && (instruction.operand as MethodInfo)?.Name == "GetFemales");
            if (index <= 0)
            {
                AI_UnlockPlayerHClothes.Logger.LogMessage("Failed transpiling 'HSceneSpriteClothCondition_Init_RedirectGetFemales' GetFemales index not found!");
                AI_UnlockPlayerHClothes.Logger.LogWarning("Failed transpiling 'HSceneSpriteClothCondition_Init_RedirectGetFemales' GetFemales index not found!");
                return il;
            }

            il[index - 2].opcode = OpCodes.Nop;
            il[index - 1].opcode = OpCodes.Nop;
            il[index] = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AI_UnlockPlayerHClothes), nameof(AI_UnlockPlayerHClothes.newGetFemales)));
            
            return il;
        }
        
        [HarmonyTranspiler, HarmonyPatch(typeof(HScene), "LateUpdate")]
        public static IEnumerable<CodeInstruction> HScene_LateUpdate_RemoveClothesLock(IEnumerable<CodeInstruction> instructions)
        {
            var il = instructions.ToList();
            
            // Force 0 to the if statement, clothes state doesn't get forced anymore //
            var index = il.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt && (instruction.operand as MethodInfo)?.Name == "IsClothesStateKind");
            if (index <= 0)
            {
                AI_UnlockPlayerHClothes.Logger.LogMessage("Failed transpiling 'HScene_LateUpdate_RemoveClothesLock' IsClothesStateKind index not found!");
                AI_UnlockPlayerHClothes.Logger.LogWarning("Failed transpiling 'HScene_LateUpdate_RemoveClothesLock' IsClothesStateKind index not found!");
                return il;
            }

            il[index - 5].opcode = OpCodes.Nop;
            il[index - 4].opcode = OpCodes.Nop;
            il[index - 3].opcode = OpCodes.Nop;
            il[index - 2].opcode = OpCodes.Nop;
            il[index - 1].opcode = OpCodes.Nop;
            il[index].opcode = OpCodes.Ldc_I4_0;
            
            index = il.FindIndex(instruction => instruction.opcode == OpCodes.Callvirt && (instruction.operand as MethodInfo)?.Name == "SetAccessoryStateAll");
            if (index <= 0)
            {
                AI_UnlockPlayerHClothes.Logger.LogMessage("Failed transpiling 'HScene_LateUpdate_RemoveClothesLock' SetAccessoryStateAll index not found!");
                AI_UnlockPlayerHClothes.Logger.LogWarning("Failed transpiling 'HScene_LateUpdate_RemoveClothesLock' SetAccessoryStateAll index not found!");
                return il;
            }
            
            // Disable forcing accessory state //
            for (var i = 0; i < 7; i++)
                il[index - i].opcode = OpCodes.Nop;
            
            // Disable forcing shoe state //
            for (var i = 1; i < 15; i++)
                il[index + i].opcode = OpCodes.Nop;

            return il;
        }
    }
}