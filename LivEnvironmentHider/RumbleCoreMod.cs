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

		

		GameObject CurrentMapProduction;
		public override void OnInitializeMelon()
		{
			Calls.onAMapInitialized += OnMapInitialized;
			
			
			if (!Directory.Exists(USER_DATA))
				Directory.CreateDirectory(USER_DATA);

			modCategory = MelonPreferences.CreateCategory(BuildInfo.Name);
			modCategory.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

			GreenScreenColor = modCategory.CreateEntry<string>("Green Screen Color", "#000000", null,"Plot Twist: Doesn't actually have to be green");

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

			BasePitMask.transform.localPosition = new Vector3(-0.2f, 0f, 0);
			BasePitMask.transform.localRotation = Quaternion.Euler(270, 0, 0);
			BasePitMask.transform.localScale = new Vector3(44.4f, 44.4f, 44.4f);

			BaseCylinder.transform.localPosition = new Vector3(0, 0.39f, 0);
			BaseCylinder.transform.localRotation = Quaternion.Euler(0, 0, 0);
			BaseCylinder.transform.localScale = new Vector3(70, 70, 70);


			isFirstLoad = false;
		}
		private void OnMapInitialized(string sceneName)
		{
			CurrentScene = sceneName.Trim().ToLower();
			BuildDebugScreen();
			
			FirstLoad();

			modCategory.LoadFromFile();

			MelonCoroutines.Start(HideFromLiv());

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

			derCylinder.SetActive(CurrentScene.Contains("map"));
			derPitMask.SetActive(CurrentScene == "map1");
			

			derCylinder.GetComponent<MeshRenderer>().material.color = gsColor;
			derPitMask.GetComponent<MeshRenderer>().material.color = gsColor;

			derCylinder.transform.SetParent(CurrentMapProduction.transform, true);
			derPitMask.transform.SetParent(CurrentMapProduction.transform, true);

		}

		/// <summary>
		/// Gives Rumblehud a chance to take portraits before moving map production layers
		/// </summary>
		/// <returns></returns>
		private IEnumerator HideFromLiv()
		{
			GameObject mapProduction = new();
			List<int> objectsToHide = new();
			GameObject arenaParent;
			GameObject tournamentScorer = GameObject.Find("NewTextGameObject(Clone)");
			tournamentScorer.layer = NO_LIV_LAYER;

			if (CurrentScene == "map0")
			{
				objectsToHide = new List<int> { 0, 1, 3, 4, 6 };
				arenaParent = Calls.GameObjects.Map0.Map0production.Mainstaticgroup.GetGameObject();
				mapProduction = Calls.GameObjects.Map0.Map0production.GetGameObject();

			}
			else if (CurrentScene == "map1")
			{
				objectsToHide = new List<int> { 0, 2, 3, 4 };
				arenaParent = Calls.GameObjects.Map1.Map1production.Mainstaticgroup.GetGameObject();
				mapProduction = Calls.GameObjects.Map1.Map1production.GetGameObject();
			} 
			else
			{
				arenaParent = new();
			}
			this.CurrentMapProduction = mapProduction;

			yield return new WaitForSeconds(3);

			for (int i = 0; i < arenaParent.transform.childCount; i++)
			{
				GameObject child = arenaParent.transform.GetChild(i).gameObject;
				if (objectsToHide.Contains(i))
				{
					child.layer = NO_LIV_LAYER;
				}
			}
		}
		public override void OnUpdate()
		{
			if(CurrentMapProduction != null)
			{
				DiffLog($"{CurrentMapProduction.activeSelf}");
			}
		}




	}
}