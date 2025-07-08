using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
	public void ChangeScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName); // ƒV[ƒ“‚ğ“Ç‚İ‚Şˆ—
	}

	// Update is called once per frame
	void Update()
	{

	}
}
