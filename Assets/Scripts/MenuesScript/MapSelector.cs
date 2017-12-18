using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour {

	public GameObject LoadingImage;
	public Slider loadingBar;
	private AsyncOperation async;


	//set the loading screen and call loadlevelwith bar
	public void SetLoadingScreen(int levelID){
		LoadingImage.SetActive (true);
		StartCoroutine (LoadLevelWithBar (levelID));
	}


	//asynchronously call the new scene and show the progress on the bar
	IEnumerator LoadLevelWithBar (int levelID){

		async=	SceneManager.LoadSceneAsync(levelID);
		while (!async.isDone) {
			loadingBar.value = async.progress;
			yield return null;
		}
	}
}

