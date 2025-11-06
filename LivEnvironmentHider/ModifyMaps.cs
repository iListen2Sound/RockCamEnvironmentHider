using Il2CppInterop.Runtime;
using Il2CppRUMBLE.Managers;
using MelonLoader;
using RumbleModdingAPI;
using System.Collections;
using UnityEngine;
//using System.Drawing;
using UnityEngine.Rendering.UI;
using static Il2CppRootMotion.FinalIK.RagdollUtility;

namespace LivEnvironmentHider
{
	public partial class LivEnvironmentHider : MelonMod
	{
		GameObject DerivedCylinder;
		GameObject DerivedPitMask;
		Dictionary<GameObject, int> HideableObjectsToLayer;

		List<int> OriginalLayer = new();

		//Negate this variable in HideFromLiv to toggle hide status. Set to false when in gym or park
		bool IsEnvVisible = true;
		bool IsFloorVisible = true;
		bool IsRingVisible = true;

		private void CreateGreenScreens()
		{
			if (CurrentScene.Contains("map"))
			{


				DerivedCylinder = GameObject.Instantiate(BaseCylinder);
				DerivedCylinder.transform.SetParent(GrabMapProduction().transform, true);
				DerivedCylinder.SetActive(false);

				if (CurrentScene == "map1")
				{

					DerivedPitMask = GameObject.Instantiate(BasePitMask);
					DerivedPitMask.transform.SetParent(GrabMapProduction().transform, true);
					DerivedPitMask.SetActive(false);

				}
				//SetGreenSreenColor(PrefGreenScreenColor.Value);
			}
		}

		private GameObject GrabArenaStaticGroup()
		{
			GameObject arenaParent;
			if (CurrentScene == "map0")
			{
				arenaParent = Calls.GameObjects.Map0.Map0production.Mainstaticgroup.GetGameObject();
			}
			else if (CurrentScene == "map1")
			{
				arenaParent = Calls.GameObjects.Map1.Map1production.Mainstaticgroup.GetGameObject();
			}
			else
			{
				Log("GrabArenaStaticGroup: Current scene is not a supported map", false, 1);
				return null;
			}

			return arenaParent;
		}


		private GameObject GrabMapProduction()
		{
			GameObject mapProduction;
			if (CurrentScene == "map0")
			{
				mapProduction = Calls.GameObjects.Map0.Map0production.GetGameObject();
			}
			else if (CurrentScene == "map1")
			{
				mapProduction = Calls.GameObjects.Map1.Map1production.GetGameObject();
			}
			else
			{
				Log("GrabMapProduction: Current scene is not a supported map", false, 1);
				return null;
			}

			return mapProduction;
		}

		private void SetFloorVisibility(bool isVisible)
		{
			GameObject floor;
			if (CurrentScene == "map0")
			{
				floor = GrabArenaStaticGroup().transform.GetChild(2).gameObject;
			}
			else if (CurrentScene == "map1")
			{
				floor = GrabArenaStaticGroup().transform.GetChild(1).gameObject;
				DerivedPitMask.SetActive(isVisible);
			}
			else
			{ Log("SetFloorVisibility: unsupported map", true, 0); return; }
			floor.layer = isVisible ? 9 : 23;
			IsFloorVisible = isVisible;
		}

		private void SetRingVisibility(bool isVisible)
		{
			GameObject ring;
			if (CurrentScene == "map0")
			{
				ring = GrabArenaStaticGroup().transform.GetChild(5).gameObject;
			}
			else if (CurrentScene == "map1")
			{
				ring = GrabArenaStaticGroup().transform.GetChild(5).gameObject;
			}
			else
			{ Log("SetRingVisibility: unsupported map", true, 0); return; }
			ring.layer = isVisible ? 0 : 23;
			IsRingVisible = isVisible;
		}

		private Dictionary<GameObject, int> GrabHideableObjects()
		{
			Dictionary<GameObject, int> hideableObjectsToLayer = new();
			List<string> hideableNames = new();
			if (CurrentScene == "map0")
			{
				hideableNames = new List<string> { "Background plane", "Backgroundrocks", "Gutter", "leave", "Root" };
				

			}
			else if (CurrentScene == "map1")
			{
				hideableNames = new List<string> { "Cliff", "Deathdert", "Leaves_Map2", "Outherboundry" };

			}
			else
			{
				Log("GrabHideableObjects: Current scene is not a supported map", true, 1);
				return null;
			}

			GameObject arenaParent = GrabArenaStaticGroup();

			//Iterate through arena parent children. Add children with hideable indices to dictionary awith its original layer property
			for (int i = 0; i < arenaParent.transform.childCount; i++)
			{

				GameObject child = arenaParent.transform.GetChild(i).gameObject;
				if (hideableNames.Contains(child.name))
				{
					hideableObjectsToLayer.Add(child, child.layer);
				}
			}

			

			return hideableObjectsToLayer;

		}


		private void SetEnvironmentVisibility(bool isVisible, bool manualCall = false)
		{
			if (!CurrentScene.Contains("map"))
			{
				Log("SetEnvVis: Current Scene is not a supported map");
				return;
			}
			Log($"SetEnvVis: CurrentScene is {CurrentScene}", true, 0);
			if (IsEnvVisible == isVisible)
			{
				Log($"SetEnvVis: Environment is already {(isVisible ? "visible" : "hidden")}.", false, 1);
				return;
			}

			GameObject tournamentScorer = GameObject.Find("NewTextGameObject(Clone)");
			if (tournamentScorer != null)
				tournamentScorer.layer = NO_LIV_LAYER;


			



			DerivedCylinder.SetActive(!isVisible);
			if (CurrentScene == "map1")
				DerivedPitMask.SetActive(!isVisible);

			Dictionary<GameObject, int> hideablesToLayer = manualCall ? HideableObjectsToLayer : GrabHideableObjects();
			foreach (KeyValuePair<GameObject, int> entry in hideablesToLayer)
			{
				entry.Key.layer = isVisible ? entry.Value : NO_LIV_LAYER;
				Log($"SetEnvVis: Setting {entry.Key.name} to layer {(isVisible ? entry.Value.ToString() : NO_LIV_LAYER.ToString())}", true);
			}

			IsEnvVisible = isVisible;
			if (isVisible)
			{
				SetFloorVisibility(isVisible);
				SetRingVisibility(isVisible);
			}
			else
			{
				if (PrefHideFloor.Value)
					SetFloorVisibility(isVisible);
				if (PrefHideRingClamp.Value)
					SetRingVisibility(isVisible);
			}



			
			PrefGreenScreenActive.Value = !isVisible;

			SetGreenSreenColor(PrefGreenScreenColor.Value);
			SavePrefs();
		}

		private void ToggleEnvHide()
		{
			SetEnvironmentVisibility(!IsEnvVisible, true);
		}

		private void HideTimers()
		{
			GameObject timers = GameObject.Find("Timers");
			HideAllChildren(timers);
		}
		private void HideMabels()
		{
			GameObject mabels = GameObject.Find("Mabels");
			HideAllChildren(mabels);
		}
		private void HideNameTags()
		{
			try
			{
				foreach (Il2CppRUMBLE.Players.Player player in PlayerManager.Instance.AllPlayers)
				{
					GameObject nameTag = player.Controller.gameObject.transform.GetChild(9).gameObject;
					Log($"HideNameTag: Name tag found for player {player.Data.GeneralData.PublicUsername}", true);
					HideAllChildren(nameTag);
				}
			}
			catch (System.Exception e)
			{
				Log("HideNameTags: " + e.Message, false, 2);
			}
		}
		private void HideAllChildren(GameObject parent)
		{
			if(parent is null)
			{
				Log("HideAllChildren: Parent is null", true, 1);
				return;
			}
			if(parent.transform.childCount == 0)
			{
				Log("HideAllChildren: Parent has no children to hide\n", true, 0);
				return;
			}
			Log($"HideAllChildren: Hiding {parent.name} from LIV", true);
			for (int i = 0; i < parent.transform.childCount; i++)
			{
				GameObject child = parent.transform.GetChild(i).gameObject;
				child.layer = NO_LIV_LAYER;
				
				HideAllChildren(child);
			}
		}

		private void ToggleFloorVis()
		{
			SetFloorVisibility(!IsFloorVisible);
			PrefHideFloor.Value = !IsFloorVisible;
			SavePrefs();
		}
		private void ToggleRingVis()
		{
			SetRingVisibility(!IsRingVisible);
			PrefHideRingClamp.Value = !IsRingVisible;
			SavePrefs();
		}
		private void SetGreenSreenColor(string hexCode)
		{
			if (IsEnvVisible)
			{
				Log("SetGreenScreenColor: Can't change green screen colors when inactive", true, 1);
				return;
			}
			Color gsColor;

			if (!ColorUtility.TryParseHtmlString(hexCode, out gsColor))
			{
				Log($"Failed to parse color from: {hexCode}", false, 2);
				return;
			}

			if (DerivedPitMask != null)
				DerivedPitMask.GetComponent<MeshRenderer>().material.color = gsColor;
			if (DerivedCylinder != null)
				DerivedCylinder.GetComponent<MeshRenderer>().material.color = gsColor;

			PrefGreenScreenColor.Value = hexCode;
			if (hexCode != PrefGreenScreenColor.Value)
				CatMain.SaveToFile();
			SavePrefs();
			Log("SetGreenScreenColor: Green screen color set to " + hexCode, true);
		}
		private IEnumerator DelayEnvironmentHiding()
		{
			yield return new WaitForSeconds((float)PrefDelayEnvHide.Value);
			SetEnvironmentVisibility(false);
			
		}
		private IEnumerator HideOtherMods()
		{
			
			yield return new WaitForSeconds((float)PrefDelayEnvHide.Value);
			HideTimers();
			HideMabels();
			HideNameTags();

		}

	}
}