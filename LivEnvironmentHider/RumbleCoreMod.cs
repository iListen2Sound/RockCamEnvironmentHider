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
			if(!isFirstLoad) return;

			InitImportedAssets();
			
			isFirstLoad = false;
		}
		private void OnMapInitialized(string sceneName)
		{
			LastScene = CurrentScene;
			CurrentScene = sceneName.Trim().ToLower();
			//BuildDebugScreen();
			
			FirstLoad();

			UpdatePrefs();


			//ModifyMaps
			CreateGreenScreens();
			MelonCoroutines.Start(HideFromLiv(true));
			
		}

		
		
		public override void OnUpdate()
		{
			if(CurrentMapProduction != null)
			{
				DiffLog($"Map Production Active: {CurrentMapProduction.activeSelf}", true);
			}
		}
	}
}