using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Unity.Mono;
using UnityEngine;
using HarmonyLib;
using Combat;
using Utilities;
using World;
using Card;
using BepInEx.Configuration;
using System.Dynamic;


namespace DICEOMANCERCheatMenu
{
    [BepInPlugin("me.mengtl.plugin.cheatmenu", "DICEOMANCERCheatMenu", "1.0.0")]
    public class DICEOMANCERCheatMenu : BaseUnityPlugin
    {
        ConfigEntry<KeyCode> hotkey;
        private bool windowShow = false;
        void Start()
        {
            hotkey = Config.Bind<KeyCode>("config", "hotkey", KeyCode.F9, "快捷键");
            Harmony.CreateAndPatchAll(typeof(DICEOMANCERCheatMenu));
            Logger.LogInfo("插件启动成功!");
        }
        void Update()
        {
            if (Input.GetKeyDown(hotkey.Value))
            {
                windowShow = !windowShow;
            }
        }

        private Rect windowRect = new Rect(50, 50, 150, 200);

        private static bool bool_1 = false;
        private static bool bool_2 = false;
        private static bool bool_3 = false;
        private static bool bool_4 = false;
        private static bool bool_5 = false;
        private static int multiple_int = 1;
        private static bool bool_6 = false;
        private static bool bool_7 = false;
        private static bool bool_8 = false;
        private static string card_copy_id = "";
        private static bool bool_9 = false;
        private static string card_remove_id = "";
        void OnGUI()
        {
            if (windowShow)
            {
                windowRect = GUILayout.Window(123, windowRect, WindowFunc, "骰子浪游者内置修改器");
            }
        }
        public void WindowFunc(int id)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", GUILayout.Width(22))) {
                windowShow = false;
            }
            GUILayout.EndHorizontal();
            bool_1 = GUILayout.Toggle(bool_1, "魔力不减");
            bool_2 = GUILayout.Toggle(bool_2, "金币不减");
            bool_3 = GUILayout.Toggle(bool_3, "金币不减反增");
            bool_4 = GUILayout.Toggle(bool_4, "生命不减");
            bool_5 = GUILayout.Toggle(bool_5, "倍功");
            if (bool_5)
            {
                multiple_int = (int)GUILayout.HorizontalSlider(multiple_int, 1, 5);
                GUILayout.Label("倍数:" + multiple_int.ToString());
            }
            bool_6 = GUILayout.Toggle(bool_6, "卡牌能量种类无要求");
            bool_7 = GUILayout.Toggle(bool_7, "卡牌自动强化");
            bool_8 = GUILayout.Toggle(bool_8, "卡牌复制");
            if (bool_8)
            {
                GUILayout.Label("需要复制的卡牌编号:");
                card_copy_id = GUILayout.TextField(card_copy_id);
            }
            bool_9 = GUILayout.Toggle(bool_9, "卡牌删除");
            if (bool_9)
            {
                GUILayout.Label("需要删除的卡牌编号:");
                card_remove_id = GUILayout.TextField(card_remove_id);
            }
            GUI.DragWindow();//窗口可拖动
        }
        //魔力不减
        [HarmonyPrefix, HarmonyPatch(typeof(ManaManager), "PayManaToManaCosts")]
        public static bool PayManaToManaCosts()
        {
            if (bool_1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        //金币不减
        [HarmonyPrefix, HarmonyPatch(typeof(MoneyManager), "LoseMoney")]
        public static void LoseMoney(ref int moneyLost)
        {
            if (bool_2)
            {
                moneyLost = 0;
            }
        }

        //金币不减反增
        [HarmonyPrefix, HarmonyPatch(typeof(MoneyManager), "LoseMoney")]
        public static void LoseMoney2(ref int moneyLost, MoneyManager __instance)
        {
            if (bool_3)
            {
                moneyLost = -moneyLost;
            }
        }

        //生命不减与倍功
        [HarmonyPrefix, HarmonyPatch(typeof(Character), "LoseLife")]
        public static void LoseLife(ref int amount, Damage damageSource, Character __instance)
        {
            if (bool_4)
            {
                if (damageSource.TargetCharacter is CharacterPlayer)
                {
                    amount = 0;
                }
            }
            if (bool_5)
            {
                amount = amount * multiple_int;
            }
        }

        //卡牌能量种类无要求
        [HarmonyPrefix, HarmonyPatch(typeof(CardManager), "GenerateCardObjFromCardInfo", new Type[] { typeof(CardInfo), typeof(OwnerShip) })]
        public static bool GenerateCardObjFromCardInfo(ref CardInfo cardInfo)
        {
            if (bool_6)
            {
                for (int i = 0; i < cardInfo.cardManaCostColorEArrays.Length; i++)
                {
                    cardInfo.cardManaCostColorEArrays[i].colorEs = new ColorE[] { ColorE.Void };
                }
            }
            return true;
        }

        //卡牌自动强化
        [HarmonyPostfix, HarmonyPatch(typeof(CardManager), "GetAllPlayerCards")]
        public static void GetAllPlayerCards(CardBase[] __result, CardManager __instance)
        {
            if (bool_7)
            {
                foreach (CardBase card in __result)
                {
                    __instance.UpgradeCard(card);
                }
            }
        }

        //卡牌一键复制
        [HarmonyPostfix, HarmonyPatch(typeof(CardManager), "GetAllPlayerCards")]
        public static void GetAllPlayerCards2(CardBase[] __result, CardManager __instance)
        {
            if (bool_8)
            {
                __instance.AddCardToPlayerCards(__result[int.Parse(card_copy_id)+1]);
            }
        }

        //卡牌一键删除
        [HarmonyPostfix, HarmonyPatch(typeof(CardManager), "GetAllPlayerCards")]
        public static async void GetAllPlayerCards3(CardBase[] __result, CardManager __instance)
        {
            if (bool_9)
            {
                await __instance.RemoveCardFromGame(__result[int.Parse(card_remove_id)+1]);
            }
        }
    }
}
