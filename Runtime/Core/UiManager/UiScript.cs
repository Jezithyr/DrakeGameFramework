using System.Collections;
using System.Collections.Generic;
using DrakeFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DrakeFramework
{
    public abstract class UIMonoBehavior : MonoBehaviour
    {
        private const string MainMenuScene = "MainMenu";
		public void ShowScreen(string screenName)
		{
			Game.Ui.ShowScreen(screenName);
		}
		public void HideScreen(string screenName)
		{
			Game.Ui.HideScreen(screenName);
		}
		public void LoadScreen(string screenName)
		{
			Game.Ui.LoadScreen(screenName);
		}
		public void UnloadScreen(string screenName)
		{
			Game.Ui.UnloadScreen(screenName);
		}
		public void LoadScene(string SceneName)
		{
			SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
		}

		public void ReturnToMainMenu()
		{
			Game.Ui.UnloadAllScreens();
			SceneManager.LoadScene(MainMenuScene, LoadSceneMode.Single);
		}
    }
}
