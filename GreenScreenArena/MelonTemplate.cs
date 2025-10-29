using MelonLoader;
using RumbleModdingAPI;
using UnityEngine;
using UnityEngine.Rendering.UI;

[assembly: MelonInfo(typeof(GreenScreenArena.GreenScreenArena), GreenScreenArena.BuildInfo.Name, GreenScreenArena.BuildInfo.Version, GreenScreenArena.BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonAuthorColor(255, 87, 166, 80)]
[assembly: MelonColor(255, 87, 166, 80)]


namespace GreenScreenArena
{
	public static class BuildInfo
	{
		public const string Name = "GreenScreenArena";
		public const string Author = "iListen2Sound";
		public const string Version = "1.0.0";
	}

	public partial class GreenScreenArena : MelonMod
	{
		private string CurrentScene = "";
		private string lastDiffLogMessage = string.Empty;

		private const int NO_LIV_LAYER = 23;
		public override void OnInitializeMelon()
		{
			Calls.onAMapInitialized += OnMapInitialized;
			LoggerInstance.Msg("Initialized.");
		}

		private void OnMapInitialized(string sceneName)
		{
			BuildDebugScreen();
			CurrentScene = sceneName;

			if (sceneName.Trim().ToLower() == "map1")
			{
				List<int> objectsToHide = new List<int> { 0, 2, 3, 4 };
				GameObject arenaParent = Calls.GameObjects.Map1.Map1production.Mainstaticgroup.GetGameObject();
				for (int i = 0; i < arenaParent.transform.childCount; i++)
				{
					GameObject child = arenaParent.transform.GetChild(i).gameObject;
					if (objectsToHide.Contains(i))
					{
						child.layer = NO_LIV_LAYER;
					}
				}
			}
			if(sceneName.Trim().ToLower() == "map0")
			{
				List<int> objectsToHide = new List<int> { 0, 1, 3, 4, 6 };
				GameObject arenaParent = Calls.GameObjects.Map0.Map0production.Mainstaticgroup.GetGameObject();
				for (int i = 0; i < arenaParent.transform.childCount; i++)
				{
					GameObject child = arenaParent.transform.GetChild(i).gameObject;
					if (objectsToHide.Contains(i))
					{
						child.layer = NO_LIV_LAYER;
					}
				}
			}
		}
		public override void OnUpdate()
		{
			DiffLog($"");

		}




	}
}