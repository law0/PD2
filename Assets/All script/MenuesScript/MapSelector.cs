using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelector : MonoBehaviour {

	public void LoadScene(int levelID){
	
		SceneManager.LoadSceneAsync(levelID);

	}
}
