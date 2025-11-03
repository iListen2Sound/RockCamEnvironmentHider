using MelonLoader;
using RumbleModdingAPI;
using UnityEngine;
using System.Collections;
//using System.Drawing;
using UnityEngine.Rendering.UI;

namespace LivEnvironmentHider
{


	public partial class LivEnvironmentHider : MelonMod
	{
		private const string USER_DATA = $"UserData/{BuildInfo.Name}/";
		private const string CONFIG_FILE = "config.cfg";

		private MelonPreferences_Category modCategory;
		private MelonPreferences_Entry<string> GreenScreenColor;
		private MelonPreferences_Entry<bool> HideFloor;
		private MelonPreferences_Entry<string> FloorBackgroundColor;
		private MelonPreferences_Entry<bool> GreenScreenActive;

		private MelonPreferences_Category Compat;
		private MelonPreferences_Entry<double> DelayEnvHide;

		private MelonPreferences_Category Diagnostics;
		private MelonPreferences_Entry<bool> DebugModePref;


		private void InitPreferences()
		{
			if (!Directory.Exists(USER_DATA))
			{
				Log("Userdata folder not found. Creating...");
				Directory.CreateDirectory(USER_DATA);
			}


			modCategory = MelonPreferences.CreateCategory(BuildInfo.Name);
			modCategory.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			
			GreenScreenActive = modCategory.CreateEntry("Green Screen Active", true, null, "Activates or deactivates green screen function");
			GreenScreenColor = modCategory.CreateEntry("Green Screen Color", "#000000", null, "Plot Twist: Doesn't actually have to be green. If using black (default) add a luma key filter to the LIV source in OBS.");
			HideFloor = modCategory.CreateEntry("Hide Combat Floor", false, null, "Hides the combat floor from Liv.");


			Compat = MelonPreferences.CreateCategory("Compatability with other mods");
			Compat.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));
			DelayEnvHide = Compat.CreateEntry("Hide Environment Delay", 2.0, null, "Delay in seconds for hiding map environment from Liv. Needed for Rumble HUD's portrait creation");

			Diagnostics = MelonPreferences.CreateCategory("Diagnostic options");
			Diagnostics.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

			DebugModePref = Diagnostics.CreateEntry("Enable Debug Mode", false, null, "Enables more verbose logging and other debugging helper tools");
			DebugModeActive = DebugModePref.Value;
		}

		private void UpdatePrefs()
		{
			modCategory.LoadFromFile();
			Compat.LoadFromFile();
			Diagnostics.LoadFromFile();
		}
	}
}