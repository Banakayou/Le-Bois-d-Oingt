using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour {

	private bool loadScene = false;
	[SerializeField] private Text loadingText;

	void Start()
	{
		if (loadScene == false)
		{
			loadScene = true;
			loadingText.text = "Chargement...";
			StartCoroutine(LoadNewScene());
		}
	}

	void Update()
	{
		if (loadScene == true)
		{
			loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));
		}
	}
		
	IEnumerator LoadNewScene()
	{
		AsyncOperation async = SceneManager.LoadSceneAsync(1);
		while (!async.isDone)
		{
			yield return null;
		}
	}
}