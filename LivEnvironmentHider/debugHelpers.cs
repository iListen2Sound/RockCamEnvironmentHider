using Il2CppRUMBLE.Managers;
using Il2CppTMPro;
using RumbleModdingAPI;
using UnityEngine;

namespace LivEnvironmentHider
{
	public partial class LivEnvironmentHider
	{

		private GameObject DebugUi;
		private TextMeshPro DebugUiText;
		private GameObject PlayerUi;
		//Set to PrefDebugMode.Value once preferences have been initialized
		private bool DebugModeActive = true;
		/// <summary>
		/// Creates a debug screen in front of the player 
		/// </summary>
		private void BuildDebugScreen()
		{
			PlayerUi = PlayerManager.Instance.LocalPlayer.Controller.gameObject.transform.GetChild(6).GetChild(0).gameObject;
			DebugUi = Calls.Create.NewText("Placeholder text. You shouldn't be seeing this without some UE Shenanigans\n or decompiled code. Unless I (probably) told you about this.", 1f, Color.white, new Vector3(0f, 0.1f, 1f), Quaternion.Euler(0, 0, 0));
			DebugUi.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			DebugUi.transform.localPosition = new Vector3(0f, 0.1f, 0.96f);
			DebugUi.transform.SetParent(PlayerUi.transform, false);
			DebugUiText = DebugUi.GetComponent<TextMeshPro>();
			DebugUi.SetActive(false);
		}


		/// <summary>
		/// updates the debug screen text
		/// </summary>
		/// <param name="message"></param>
		private void UpdateDebugScreen(string message)
		{
			if (Calls.IsMapInitialized()) { DebugUiText.text = message; }
		}
		/// <summary>
		/// Call in OnUpdate to monitor variables per frame but only logs if they change
		/// </summary>
		/// <param name="message"></param>
		/// <param name="debugOnly"></param>
		/// <param name="logLevel"></param>
		private void DiffLog(string message, bool debugOnly = true, int logLevel = 0)
		{
			if (message != lastDiffLogMessage)
			{
				lastDiffLogMessage = message;
				Log("DIFFLOG: " + message, debugOnly, logLevel);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="debugOnly"></param>
		/// <param name="logLevel"></param>
		private void Log(string message, bool debugOnly = false, int logLevel = 0)
		{
			if (!DebugModeActive && debugOnly)
				return;
			switch (logLevel)
			{
				case 0:
					LoggerInstance.Msg(message);
					break;
				case 1:
					LoggerInstance.Warning("Warn: " + message);
					break;
				case 2:
					LoggerInstance.Error("Error: " + message);
					break;
				default:
					LoggerInstance.Msg("Unrecognized logLevel: " + message);
					break;
			}
		}

	}
}
