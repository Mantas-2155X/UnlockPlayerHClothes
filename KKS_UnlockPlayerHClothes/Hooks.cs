using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KKS_UnlockPlayerHClothes
{
    public class Hooks
    {
        [HarmonyPostfix, HarmonyPatch(typeof(HSprite), nameof(HSprite.Start))]
        public static void HSprite_Start_Patch(HSprite __instance)
        {
            var original = GameObject.Find("Canvas/SubMenu/ClothCategory/ClothMale/ClothCategory/Cloth/ClothGroup/ClothCategory/Cloth");
            if (original == null)
                return;

            original.GetComponentInChildren<TextMeshProUGUI>().text = "All Clothes";
            
            foreach (var maleButton in KKS_UnlockPlayerHClothes.extraMaleButtons)
            {
                var copy = Object.Instantiate(original, original.transform.parent);
                copy.name = maleButton.Value;
                copy.transform.SetSiblingIndex(maleButton.Key - 1);
                copy.GetComponentInChildren<TextMeshProUGUI>().text = maleButton.Value;
                
                var button = copy.GetComponent<Button>();
                button.onClick = new Button.ButtonClickedEvent();
                button.onClick.AddListener(delegate { __instance.OnClickClothMale(maleButton.Key); });
            }
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(HSprite), nameof(HSprite.OnClickClothMale))]
        public static void HSprite_OnClickClothMale_Patch(ChaControl ___male, int _cloth)
        {
            if (_cloth < 2)
                return;

            Manager.Config.HData.IsMaleShoes = !Manager.Config.HData.IsMaleShoes;
            
            switch (_cloth)
            {
                case 2:
                    ___male.SetClothesStateNext((int)ChaFileDefine.ClothesKind.top);
                    break;
                case 3:
                    ___male.SetClothesStateNext((int)ChaFileDefine.ClothesKind.bot);
                    break;
                case 4:
                    ___male.SetClothesStateNext((int)ChaFileDefine.ClothesKind.shorts);
                    break;
                case 5:
                    ___male.SetClothesStateNext((int)ChaFileDefine.ClothesKind.gloves);
                    break;
                case 6:
                    ___male.SetClothesStateNext((int)ChaFileDefine.ClothesKind.panst);
                    break;
                case 7:
                    ___male.SetClothesStateNext((int)ChaFileDefine.ClothesKind.socks);
                    break;
            }
        }
    }
}