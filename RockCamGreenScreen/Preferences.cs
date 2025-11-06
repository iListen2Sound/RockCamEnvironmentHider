using MelonLoader;
using RumbleModdingAPI;
using UnityEngine;
using System.Collections;
//using System.Drawing;
using UnityEngine.Rendering.UI;

namespace RockCamGreenScreen
{


	public partial class RockCamGreenScreen : MelonMod
	{
		private const string USER_DATA = $"UserData/{BuildInfo.Name}/";
		private const string CONFIG_FILE = "config.cfg";

		private MelonPreferences_Category CatMain;
		private MelonPreferences_Entry<string> PrefGreenScreenColor;
		private MelonPreferences_Entry<bool> PrefHideFloor;
		private MelonPreferences_Entry<string> PrefFloorBackgroundColor;
		private MelonPreferences_Entry<bool> PrefGreenScreenActive;
		private MelonPreferences_Entry<bool> PrefHideRingClamp;

		private MelonPreferences_Category CatInput;
		private MelonPreferences_Entry<string> PrefModifierKey;

		private MelonPreferences_Category CatCompat;
		private MelonPreferences_Entry<double> PrefDelayEnvHide;

		private MelonPreferences_Category CatDiagnostics;
		private MelonPreferences_Entry<bool> PrefDebugMode;


		private void InitPreferences()
		{
			if (!Directory.Exists(USER_DATA))
			{
				Log("Userdata folder not found. Creating...");
				Directory.CreateDirectory(USER_DATA);
			}


			CatMain = MelonPreferences.CreateCategory(BuildInfo.Name, BuildInfo.Name);	
			CatMain.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			
			PrefGreenScreenActive = CatMain.CreateEntry("Green Screen Active", true, null, "Activates or deactivates green screen function");
			PrefGreenScreenColor = CatMain.CreateEntry("Green Screen Color", "#FF00FF", null, "Plot Twist: Doesn't actually have to be green. You can use black with a luma key filter");
			PrefHideFloor = CatMain.CreateEntry("Hide Combat Floor", false, null, "Hides the combat floor from the Rock Cam.");
			PrefHideRingClamp = CatMain.CreateEntry("Hide Ring Clamp", false, null, "Hides the ring clamp from the Rock Cam.");

			CatInput = MelonPreferences.CreateCategory("Input");
			CatInput.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			PrefModifierKey = CatInput.CreateEntry("Modifier Key", "LeftAlt", null, "Key to use as a modifier to use keyboard input (specify left or right when using actual modifier keys)");


			CatCompat = MelonPreferences.CreateCategory("Compatability with other mods");
			CatCompat.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			PrefDelayEnvHide = CatCompat.CreateEntry("Hide Environment Delay", 2.0, null, "Delay in seconds for hiding map environment from Liv. Needed for Rumble HUD's portrait creation");

			CatDiagnostics = MelonPreferences.CreateCategory("Diagnostic options");
			CatDiagnostics.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

			PrefDebugMode = CatDiagnostics.CreateEntry("Enable Debug Mode", false, null, "Enables more verbose logging and other debugging helper tools");
			DebugModeActive = PrefDebugMode.Value;

			CatMain.SaveToFile();
			CatInput.SaveToFile();
			CatCompat.SaveToFile();
			CatDiagnostics.SaveToFile();
		}

		private void ReadPrefs()
		{
			CatMain.LoadFromFile();
			CatCompat.LoadFromFile();
			CatDiagnostics.LoadFromFile();
			DebugModeActive = PrefDebugMode.Value;
		}
		private void SavePrefs()
		{
			CatMain.SaveToFile();
		}
	}
}