using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	private void Start()
	{
		this.Init();
        Solarmax.Singleton<LoadResManager>.Get().Init();
		LoadResManager.LoadScene("scenes/game.ab");
		UnityEngine.SceneManagement.SceneManager.LoadScene("Game", LoadSceneMode.Single);
	}

	private void Init()
	{
	}
}
