using MelonLoader;
using RumbleModdingAPI;
using UnityEngine;
using System.Collections;
//using System.Drawing;
using UnityEngine.Rendering.UI;

[assembly: MelonInfo(typeof(LivEnvironmentHider.LivEnvironmentHider), LivEnvironmentHider.BuildInfo.Name, LivEnvironmentHider.BuildInfo.Version, LivEnvironmentHider.BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonAuthorColor(255, 87, 166, 80)]
[assembly: MelonColor(255, 87, 166, 80)]


namespace LivEnvironmentHider
{
	public static class BuildInfo
	{
		public const string Name = "LivEnvironmentHider";
		public const string Author = "iListen2Sound";
		public const string Version = "1.0.0";
	}

	public partial class LivEnvironmentHider : MelonMod
	{

		private string LastScene = "";
		private string CurrentScene = "";
		private bool isFirstLoad = true;
		private string lastDiffLogMessage = string.Empty;

		private const int NO_LIV_LAYER = 23;
		private const int LIV_ONLY_LAYER = 19;


		GameObject CurrentMapProduction;
		public override void OnInitializeMelon()
		{
			Calls.onAMapInitialized += OnMapInitialized;

			InitPreferences(); //preferences.cs

			LoggerInstance.Msg("Initialized.");
		}

		private void FirstLoad()
		{
			if (!isFirstLoad) return;

			InitImportedAssets();

			isFirstLoad = false;
		}
		private void OnMapInitialized(string sceneName)
		{
			LastScene = CurrentScene;
			CurrentScene = sceneName.Trim().ToLower();
			isEnvHidden = false;
			//BuildDebugScreen();
			FirstLoad();

			if (!isFirstLoad)
				UpdatePrefs();

			//ModifyMaps
			CreateGreenScreens();
			if (GreenScreenActive.Value)
			{


				if (LastScene == CurrentScene)
				{
					SetEnvironmentVisibility(false, false);
				}
				else
				{
					MelonCoroutines.Start(DelayEnvironmentHiding());
				}
			}

		}



		public override void OnUpdate()
		{
			if (CurrentMapProduction != null)
			{
				DiffLog($"Map Production Active: {CurrentMapProduction.activeSelf}", true);
			}
			if (Input.GetKeyDown(KeyCode.Z) && DebugModeActive)
			{
				ToggleEnvHide();
			}
			if (Input.GetKey(KeyCode.S) && DebugModeActive)
			{
				string colorHex = null;
				if (Input.GetKeyDown(KeyCode.G)) colorHex = "#00FF00";
				else if (Input.GetKeyDown(KeyCode.B)) colorHex = "#0000FF";
				else if (Input.GetKeyDown(KeyCode.R)) colorHex = "#FF0000";
				else if (Input.GetKeyDown(KeyCode.K)) colorHex = "#000000";
				else if (Input.GetKeyDown(KeyCode.M)) colorHex = "#FF00FF";
				else if (Input.GetKeyDown(KeyCode.W)) colorHex = "#FFFFFF";
				else if (Input.GetKeyDown(KeyCode.Y)) colorHex = "#FFFF00";
				else if (Input.GetKeyDown(KeyCode.C)) colorHex = "#00FFFF";
				else if (Input.GetKeyDown(KeyCode.O)) colorHex = "#FFA500";
				else if (Input.GetKeyDown(KeyCode.P)) colorHex = "#FFC0CB";
				else if (Input.GetKeyDown(KeyCode.L)) colorHex = "#800080";
				else if (Input.GetKeyDown(KeyCode.T)) colorHex = "#808080";
				else if (Input.GetKeyDown(KeyCode.D)) colorHex = "#A52A2A";

				if (colorHex != null)
				{
					DiffLog($"SetGreenSreenColor called with {colorHex}", true);
					SetGreenSreenColor(colorHex);
				}

			}
			
		}
	}
}