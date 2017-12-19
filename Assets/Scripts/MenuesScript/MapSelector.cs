using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapSelector : MonoBehaviour {

    public static bool isHost = true;
    public static string addr = "";
	public GameObject LoadingImage;
	public Slider loadingBar;
	private AsyncOperation async;
    private Camera cam;


    void Start()
    {
        cam = Camera.main;
    }

    public void nextMenu()
    {
        Vector3 x = cam.transform.position;
        x.x += 907;
        cam.transform.position = x;
    }

    public static void setIsHost(bool b)
    {
        isHost = b;
    }

    public static void setServerAddr(string str)
    {
        addr = str;
    }

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

