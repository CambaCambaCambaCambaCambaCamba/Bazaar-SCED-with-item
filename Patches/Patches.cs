using System;
using System.Text.RegularExpressions;
using HarmonyLib;
using TheBazaar.Tooltips;
using TheBazaar.UI.Tooltips;

namespace ShowCombatEncounterDetail.Patches
{
	internal class Patches
	{
		[HarmonyPatch(typeof(CardTooltipController), "StartTooltipFadeIn")]
		[HarmonyPostfix]
		private static void CardTooltipController_StartTooltipFadeIn(CardTooltipController __instance)
		{
			bool flag = ShowCombatEncounterDetail.IsPveEncounter && ShowCombatEncounterDetail.ToolTipCardName != "";
			if (flag)
			{
				ShowCombatEncounterDetail.CreateImageDisplayFromCardName();
			}
		}

		[HarmonyPatch(typeof(CardTooltipController), "LockTooltip")]
		[HarmonyPostfix]
		private static void CardTooltipController_LockTooltip(CardTooltipController __instance)
		{
			bool flag = ShowCombatEncounterDetail.IsItem && ShowCombatEncounterDetail.ToolTipCardName != "";
			if (flag)
			{
				ShowCombatEncounterDetail.CreateImageDisplayFromCardName();
			}
		}

		[HarmonyPatch(typeof(CardTooltipController), "StartTooltipFadeOut")]
		[HarmonyPostfix]
		private static void CardTooltipController_StartTooltipFadeOut(CardTooltipController __instance)
		{
			ShowCombatEncounterDetail.IsPveEncounter = false;
			ShowCombatEncounterDetail.IsItem = false;
			ShowCombatEncounterDetail.ToolTipCardName = "";
			ShowCombatEncounterDetail.CleanDestroy();
		}

		[HarmonyPatch(typeof(CardTooltipData), "GetPVEEncounterLevel")]
		[HarmonyPostfix]
		private static void CardTooltipData_GetPVEEncounterLevel(CardTooltipData __instance)
		{
			ShowCombatEncounterDetail.ToolTipCardName = __instance.GetTitle();
			ShowCombatEncounterDetail.IsPveEncounter = true;
		}

		[HarmonyPatch(typeof(CardTooltipData), "GetTooltipHero")]
		[HarmonyPostfix]
		private static void CardTooltipData_GetTooltipHero(CardTooltipData __instance)
		{
			ShowCombatEncounterDetail.ToolTipCardName = __instance.GetTitle();
			ShowCombatEncounterDetail.IsItem = true;

			// List of prefixes to remove (make this configurable if needed)
			string[] prefixes = { "Deadly ", "Heavy ", "Icy ", "Turbo ", "Shielded ", "Restorative ", "Toxic ", "Fiery ", "Shiny ", "Deadly ", "Golden ", "Obsidian ", "Radiant "/* Add more prefixes */ };

			// Use regular expressions for more robust prefix removal (handles variations in spacing)
			foreach (string prefix in prefixes)
			{
				// Use Regex.Replace with a word boundary (\b) to avoid partial matches
				ShowCombatEncounterDetail.ToolTipCardName = Regex.Replace(ShowCombatEncounterDetail.ToolTipCardName, @"\b" + Regex.Escape(prefix), "", RegexOptions.IgnoreCase);
			}

			//Trim any extra whitespace that might be left after removing the prefix.
			ShowCombatEncounterDetail.ToolTipCardName = ShowCombatEncounterDetail.ToolTipCardName.Trim();
		}
	}
}
