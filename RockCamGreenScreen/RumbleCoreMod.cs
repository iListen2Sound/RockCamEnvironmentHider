using MelonLoader;
using RumbleModdingAPI;
using UnityEngine;


[assembly: MelonInfo(typeof(RockCamGreenScreen.RockCamGreenScreen), RockCamGreenScreen.BuildInfo.Name, RockCamGreenScreen.BuildInfo.Version, RockCamGreenScreen.BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonAuthorColor(255, 87, 166, 80)]
[assembly: MelonColor(255, 87, 166, 80)]


namespace RockCamGreenScreen
{
	public static class BuildInfo
	{
		public const string Name = "RockCamGreenScreen";
		public const string Author = "iListen2Sound";
		public const string Version = "1.0.0";
	}

	public partial class RockCamGreenScreen : MelonMod
	{

		private string LastScene = "";
		private string CurrentScene = "";
		private bool isFirstLoad = true;
		private string lastDiffLogMessage = string.Empty;

		private const int NO_LIV_LAYER = 23;
		private const int LIV_ONLY_LAYER = 19;

		private KeyCode HoldKey = KeyCode.LeftAlt;


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
			IsEnvVisible = true;
			//BuildDebugScreen();
			

			if (!isFirstLoad)
				ReadPrefs();
			FirstLoad();

			if (!Enum.TryParse<KeyCode>(PrefModifierKey.Value, out HoldKey))
			{
				Log($"Unable to parse Modifier Key Config {PrefModifierKey.Value}", false, 2);
			}
			//ModifyMaps
			//Grab hideable objects for current map and store their original layer values
			HideableObjectsToLayer = GrabHideableObjects();
			if(CurrentScene.Contains("map"))
				MelonCoroutines.Start(HideOtherMods());



			CreateGreenScreens();
			if (PrefGreenScreenActive.Value)
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
			
			if (Input.GetKey(HoldKey))
			{
				string colorHex = null;
				if (Input.GetKeyDown(KeyCode.Z))
				{
					ToggleEnvHide();
				}
				if(Input.GetKeyDown(KeyCode.F))
				{
					if (!IsEnvVisible)
						ToggleFloorVis();
					else
						Log("Can't toggle floor visibility when greater environment isn't hidden");
				}
				if (Input.GetKeyDown(KeyCode.Q)) 
				{
					if (!IsEnvVisible)
						ToggleRingVis();
					else
						Log("Can't toggle ring visibility when greater environment isn't hidden");
				}

				else if (Input.GetKeyDown(KeyCode.G)) colorHex = "#00FF00";
				else if (Input.GetKeyDown(KeyCode.B)) colorHex = "#00F";
				else if (Input.GetKeyDown(KeyCode.R)) colorHex = "#F00";
				else if (Input.GetKeyDown(KeyCode.K)) colorHex = "#000";
				else if (Input.GetKeyDown(KeyCode.M)) colorHex = "#F0F";
				else if (Input.GetKeyDown(KeyCode.W)) colorHex = "#FFF";
				else if (Input.GetKeyDown(KeyCode.Y)) colorHex = "#FF0";
				else if (Input.GetKeyDown(KeyCode.C)) colorHex = "#0FF";
				else if (Input.GetKeyDown(KeyCode.O)) colorHex = "#FFA500";
				else if (Input.GetKeyDown(KeyCode.P)) colorHex = "#FFC0CB";
				else if (Input.GetKeyDown(KeyCode.L)) colorHex = "#FAE";
				else if (Input.GetKeyDown(KeyCode.T)) colorHex = "#F30";
				else if (Input.GetKeyDown(KeyCode.I)) colorHex = "#57A650";

				

				if (colorHex != null)
				{
					DiffLog($"SetGreenSreenColor called with {colorHex}", true);
					SetGreenSreenColor(colorHex);
				}

			}
			
		}
	}
}