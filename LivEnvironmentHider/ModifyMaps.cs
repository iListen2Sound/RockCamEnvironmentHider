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
				Color gsColor;

				if (!ColorUtility.TryParseHtmlString(GreenScreenColor.Value, out gsColor))
				{
					Log($"Failed to parse color from: {GreenScreenColor.Value}", false, 2);
					gsColor = Color.black;
				}

				
				DerivedCylinder = GameObject.Instantiate(BaseCylinder);
				DerivedCylinder.SetActive(GreenScreenActive.Value);
				DerivedCylinder.GetComponent<MeshRenderer>().material.color = gsColor;
				


				if (CurrentScene == "map1")
				{
					
					DerivedPitMask = GameObject.Instantiate(BasePitMask);
					DerivedPitMask.GetComponent<MeshRenderer>().material.color = gsColor;
					DerivedPitMask.SetActive(GreenScreenActive.Value);

				}
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
		/// <summary>
		/// Gives Rumblehud a chance to take portraits before moving map production layers
		/// </summary>
		/// <returns></returns>
		private IEnumerator HideFromLiv(bool hide, bool skipDelay = false)
		{

			Log($"HideFromLiv: CurrentScene is {CurrentScene}", true, 0);
			if (isEnvHidden == hide)
			{
				Log($"HideFromLiv: Environment is already {(hide ? "hidden" : "visible")}. Breaking out of coroutine", false, 1);
				yield break;

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
				//Add combat floor as last element so it can be removed from the list if hide combat floor is disabled.
				objectsToHide = new List<int> { 0, 1, 3, 4, 6, };
				

			}
			else if (CurrentScene == "map1")
			{
				objectsToHide = new List<int> { 0, 2, 3, 4 };
				//Parent derived pit mask and cylinder so they get disabled along with the map production when a custom map is loaded
				DerivedPitMask.transform.SetParent(mapProduction.transform, true);
				DerivedPitMask.SetActive(hide);
			} 
			else
			{
				isEnvHidden = false;
				//Exit if not in a combat map
				yield break;
			}

			DerivedCylinder.transform.SetParent(mapProduction.transform, true);
			DerivedCylinder.SetActive(hide);

			SetFloorVisibility(!hide);
			
			CurrentMapProduction = mapProduction;

            float secondsToWait; 
			//treat being in a different scene from the last as the first time the arena is loaded.
			bool firstArenaLoad = CurrentScene != LastScene;
			// Give rumble hud a chance to take the opponent's portraite when the map first loads
            secondsToWait = (!firstArenaLoad && hide) ? 0 : ((float) DelayEnvHide.Value);
			yield return new WaitForSeconds(skipDelay ? 0 : secondsToWait);

			//Loop through arena parent's children. When a child's index is in the list of objectsToHide, set the layer accordingly.
			for (int i = 0; i < arenaParent.transform.childCount; i++)
			{
				
				GameObject child = arenaParent.transform.GetChild(i).gameObject;
				Log($"Hide from LIV pre: {child.name} layer: {child.layer}", true);
				//Store the environment elements' original layers in a list only on the first load.
				if(firstArenaLoad) //Firstload is the wrong variable to check here. Only reason it's successful is because the initial indices are already there and never cleared.
					//Skip delay would be better but the name isn't descriptive. Need to rename to something that reflects the fact that it's used when you change the hid state without first loading the arena
				{
					OriginalLayer.Add(child.layer);
				}
				if (objectsToHide.Contains(i))
				{
					// if hide is intended, set layer to hide from liv, otherwise, set it to the original layer value
					child.layer = hide ? NO_LIV_LAYER : OriginalLayer[i];
				}
				Log($"Hide from LIV post: {child.name} layer: {child.layer}", true);
			}
			isEnvHidden = hide;
			GreenScreenActive.Value = hide;
			modCategory.SaveToFile();
		}

		private void SetFloorVisibility(bool isVisible)
		{
			GameObject floor;
			if(CurrentScene == map0)
			{
				floor = GrabArenaStaticGroup().transform.GetChild(2);
			}
			else if (CurrentScene == map1)
			{
				floor = GrabArenaStaticGroup().transform.GetChild(1);
			}
			floor.layer = isVisible ? 9 : 23;
		}


		private void ToggleEnvHide()
		{
			
			MelonCoroutines.Start(HideFromLiv(!isEnvHidden, true));
		}
    }
}