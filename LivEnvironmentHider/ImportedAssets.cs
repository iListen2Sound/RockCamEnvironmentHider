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
        private GameObject ScreenPack;
		private GameObject BasePitMask;
		private GameObject BaseCylinder;

        private void InitImportedAssets()
        {
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

			BasePitMask.transform.localPosition = new Vector3(-0.2f, 0.01f, 0);
			BasePitMask.transform.localRotation = Quaternion.Euler(270, 0, 0);
			BasePitMask.transform.localScale = new Vector3(44.4f, 44.4f, 44.4f); 

			BaseCylinder.transform.localPosition = new Vector3(0, 0.39f, 0);
			BaseCylinder.transform.localRotation = Quaternion.Euler(0, 0, 0);
			BaseCylinder.transform.localScale = new Vector3(70, 70, 70);

        }
    }
}