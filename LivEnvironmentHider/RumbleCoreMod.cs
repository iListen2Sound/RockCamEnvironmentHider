using MelonLoader;
using RumbleModdingAPI;
using UnityEngine;
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
		private const string USER_DATA = $"UserData/{BuildInfo.Name}/";
		private const string CONFIG_FILE = "config.cfg";

		private MelonPreferences_Category modCategory;
		private MelonPreferences_Entry<string> GreenScreenColor;

		private string CurrentScene = "";
		private bool isFirstLoad = true;
		private string lastDiffLogMessage = string.Empty;

		private const int NO_LIV_LAYER = 23;
		private const int LIV_ONLY_LAYER = 19;

	

		private GameObject ScreenPack;
		private GameObject BasePitMask;
		private GameObject BaseCylinder;

		

		GameObject mapProduction;
		public override void OnInitializeMelon()
		{
			Calls.onAMapInitialized += OnMapInitialized;
			Calls.onMatchStarted += OnMatchStarted;
			
			if (!Directory.Exists(USER_DATA))
				Directory.CreateDirectory(USER_DATA);

			modCategory = MelonPreferences.CreateCategory(BuildInfo.Name);
			modCategory.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

			GreenScreenColor = modCategory.CreateEntry<string>("Green Screen Color", "#000000");

			LoggerInstance.Msg("Initialized.");
		}

		private void FirstLoad()
		{
			if(!isFirstLoad) return;

			ScreenPack = GameObject.Instantiate(Calls.LoadAssetFromStream<GameObject>(this, "LivEnvironmentHider.Assets.livgreenscreen", "LivEnvironmentHider"));
			GameObject.DontDestroyOnLoad(ScreenPack);
			BasePitMask = ScreenPack.transform.GetChild(0).gameObject;
			BaseCylinder = ScreenPack.transform.GetChild(2).gameObject;
			ScreenPack.SetActive(false);

			for(int i = 0; i < ScreenPack.transform.childCount; i++)
			{
				GameObject child = ScreenPack.transform.GetChild(i).gameObject;
				child.layer = LIV_ONLY_LAYER;
				child.GetComponent<MeshRenderer>().material.color = Color.black;
				child.SetActive(false);
			}

			BasePitMask.transform.localPosition = new Vector3(0, 0.39f, 0);
			BasePitMask.transform.localRotation = Quaternion.Euler(270, 0, 0);
			BasePitMask.transform.localScale = new Vector3(44, 44, 44);

			BaseCylinder.transform.localPosition = new Vector3(0, 0.39f, 0);
			BaseCylinder.transform.localRotation = Quaternion.Euler(0, 0, 0);
			BaseCylinder.transform.localScale = new Vector3(70, 70, 70);


			isFirstLoad = false;
		}
		private void OnMapInitialized(string sceneName)
		{
			CurrentScene = sceneName;
			BuildDebugScreen();
			
			FirstLoad();

			modCategory.LoadFromFile();
			GameObject mapProduction = new();


			if (sceneName.Trim().ToLower() == "map1")
			{
				List<int> objectsToHide = new List<int> { 0, 2, 3, 4 };
				GameObject arenaParent = Calls.GameObjects.Map1.Map1production.Mainstaticgroup.GetGameObject();
				mapProduction = Calls.GameObjects.Map1.Map1production.GetGameObject();
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
				mapProduction = Calls.GameObjects.Map0.Map0production.GetGameObject();
				for (int i = 0; i < arenaParent.transform.childCount; i++)
				{
					GameObject child = arenaParent.transform.GetChild(i).gameObject;
					if (objectsToHide.Contains(i))
					{
						child.layer = NO_LIV_LAYER;
					}
				}
			}

			this.mapProduction = mapProduction;
			Color gsColor;

			if (!ColorUtility.TryParseHtmlString(GreenScreenColor.Value, out gsColor))
			{
				Log($"Failed to parse color from: {GreenScreenColor.Value}", false, 2);
				gsColor = Color.black;
			}
			GameObject derCylinder;
			GameObject derPitMask;

			derCylinder = GameObject.Instantiate(BaseCylinder);
			derPitMask = GameObject.Instantiate(BasePitMask);

			derCylinder.SetActive(sceneName.Trim().ToLower().Contains("map"));
			derPitMask.SetActive(sceneName.Trim().ToLower() == "map1");
			

			derCylinder.GetComponent<MeshRenderer>().material.color = gsColor;
			derPitMask.GetComponent<MeshRenderer>().material.color = gsColor;

			derCylinder.transform.SetParent(mapProduction.transform);
			derPitMask.transform.SetParent(mapProduction.transform);

		}

		private void OnMatchStarted()
		{



		}
		public override void OnUpdate()
		{
			if(mapProduction != null)
			{
				DiffLog($"{mapProduction.activeSelf}");
			}
		}




	}
}