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
		private bool isFirstLoad = true;
		private string lastDiffLogMessage = string.Empty;

		private const int NO_LIV_LAYER = 23;
		private const int LIV_ONLY_LAYER = 19;

	

		private GameObject ScreenPack;
		private GameObject PitMask;
		private GameObject Cylinder;
		public override void OnInitializeMelon()
		{
			Calls.onAMapInitialized += OnMapInitialized;
			LoggerInstance.Msg("Initialized.");
		}

		private void FirstLoad()
		{
			if(!isFirstLoad) return;

			//ArenaAlpha loc, rot, scale: 
			//pos = -0.1692 0.0159 0.01
			//localpos = 0 0.39 0
			//Rot = 270 0 0
			//scale = 44 44 44

			//Cylinder loc, rot, scale:
			//pos = -0.1692 0.0159 0.01
			//localpos = 0 0.39 0
			//rot = 0 0 0
			//scale = 50 50 50
			ScreenPack = GameObject.Instantiate(Calls.LoadAssetFromStream<GameObject>(this, "GreenScreenArena.Assets.livgreenscreen", "LivEnvironmentHider"));
			GameObject.DontDestroyOnLoad(ScreenPack);
			PitMask = ScreenPack.transform.GetChild(0).gameObject;
			Cylinder = ScreenPack.transform.GetChild(2).gameObject;
			ScreenPack.SetActive(true);

			for(int i = 0; i < ScreenPack.transform.childCount; i++)
			{
				GameObject child = ScreenPack.transform.GetChild(i).gameObject;
				child.layer = LIV_ONLY_LAYER;
				child.GetComponent<MeshRenderer>().material.color = Color.black;
				child.SetActive(false);
			}

			PitMask.transform.localPosition = new Vector3(0, 0.39f, 0);
			PitMask.transform.localRotation = Quaternion.Euler(270, 0, 0);
			PitMask.transform.localScale = new Vector3(44, 44, 44);

			Cylinder.transform.localPosition = new Vector3(0, 0.39f, 0);
			Cylinder.transform.localRotation = Quaternion.Euler(0, 0, 0);
			Cylinder.transform.localScale = new Vector3(50, 50, 50);


			isFirstLoad = false;
		}
		private void OnMapInitialized(string sceneName)
		{
			CurrentScene = sceneName;
			BuildDebugScreen();
			
			FirstLoad();

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

			Cylinder.SetActive(sceneName.Trim().ToLower().Contains("map"));
			PitMask.SetActive(sceneName.Trim().ToLower() == "map1");
		}
		public override void OnUpdate()
		{
			DiffLog($"");

		}




	}
}