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
		GameObject DerivedCylinder;
		GameObject DerivedPitMask;

		List<int> OriginalLayer = new();

		//Negate this variable in HideFromLiv to toggle hide status. Set to false when in gym or park
		bool isEnvHidden = false;

		private void CreateGreenScreens()
		{
			if(CurrentScene.Contains("map"))
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
				SetGreenSreenColor(GreenScreenColor.Value);
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
				return null;
			
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
				return null;
			
			return mapProduction;
		}

		private void SetFloorVisibility(bool isVisible)
		{
			GameObject floor;
			if(CurrentScene == "map0")
			{
				floor = GrabArenaStaticGroup().transform.GetChild(2).gameObject;
			}
			else if (CurrentScene == "map1")
			{
				floor = GrabArenaStaticGroup().transform.GetChild(1).gameObject;
			}
			else return;
			floor.layer = isVisible ? 9 : 23;
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
			else return;
				ring.layer = isVisible ? 0 : 23;
		}


		private void SetEnvironmentVisibility(bool isVisible, bool manualCall = false)
		{
			Log($"SetEnvVis: CurrentScene is {CurrentScene}", true, 0);
			if (isEnvHidden == !isVisible)
			{
				Log($"SetEnvVis: Environment is already {(isVisible ? "visible" : "hidden")}. Breaking out of coroutine", false, 1);

			}
			GameObject mapProduction = GrabMapProduction();
			List<int> objectsToHide;
			GameObject arenaParent = GrabArenaStaticGroup();
			GameObject tournamentScorer = GameObject.Find("NewTextGameObject(Clone)");
			int combatFloorIndex;
			if(tournamentScorer != null)
				tournamentScorer.layer = NO_LIV_LAYER;
			

			//Select game objects to hide according to the arena
			
			if (CurrentScene == "map0")
			{
				//Add combat ring as last element so it can be removed from the list if hide combat ring is disabled.
				objectsToHide = new List<int> { 0, 1, 3, 4, 6, };
				

			}
			else if (CurrentScene == "map1")
			{
				objectsToHide = new List<int> { 0, 2, 3, 4 };
				//Parent derived pit mask and cylinder so they get disabled along with the map production when a custom map is loaded
				DerivedPitMask.SetActive(!isVisible);
			} 
			else
			{
				isEnvHidden = false;
				return;
			}


			DerivedCylinder.SetActive(!isVisible);

			
			
			CurrentMapProduction = mapProduction;


			//Loop through arena parent's children. When a child's index is in the list of objectsToHide, set the layer accordingly.
			for (int i = 0; i < arenaParent.transform.childCount; i++)
			{
				
				GameObject child = arenaParent.transform.GetChild(i).gameObject;
				Log($"SetEnvVis pre: {child.name} layer: {child.layer}", true);
				//Store the environment elements' original layers in a list only on the first load.
				if(!manualCall) //Firstload is the wrong variable to check here. Only reason it's successful is because the initial indices are already there and never cleared.
					//Skip delay would be better but the name isn't descriptive. Need to rename to something that reflects the fact that it's used when you change the hid state without first loading the arena
				{
					OriginalLayer.Add(child.layer);
					if(child.layer == LIV_ONLY_LAYER)
						Log($"SetEnvVis: Found arena element set to no liv layer about to be added to OriginalLayer<int>. Possible logic error", true, 1);
				}
				if (objectsToHide.Contains(i))
				{
					// if hide is intended, set layer to hide from liv, otherwise, set it to the original layer value
					child.layer = isVisible ? OriginalLayer[i] : NO_LIV_LAYER;
				}
				Log($"SetEnvVis: {child.name} layer: {child.layer}", true);
			}
			SetFloorVisibility(isVisible);

			isEnvHidden = !isVisible;
			
			modCategory.SaveToFile();
		}

		private void ToggleEnvHide()
		{
			GreenScreenActive.Value = !isEnvHidden;
			SetEnvironmentVisibility(isEnvHidden);
			
		}

		private void SetGreenSreenColor(string hexCode)
		{
			Color gsColor;

			if (!ColorUtility.TryParseHtmlString(hexCode, out gsColor))
			{
				Log($"Failed to parse color from: {hexCode}", false, 2);
				return;
			}

			if(DerivedPitMask != null)
				DerivedPitMask.GetComponent<MeshRenderer>().material.color = gsColor;
			if(DerivedCylinder != null)
				DerivedCylinder.GetComponent<MeshRenderer>().material.color = gsColor;

			GreenScreenColor.Value = hexCode;
			if(hexCode != GreenScreenColor.Value)
				modCategory.SaveToFile();
		}
		private IEnumerator DelayEnvironmentHiding()
		{
			yield return new WaitForSeconds((float) DelayEnvHide.Value);
			SetEnvironmentVisibility(false);
		}

    }
}