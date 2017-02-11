using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI : MonoBehaviour {

    public GameObject pauseWindow;
    public GameObject gameplayGUI;
    public GameObject MainMenuScreen;

    [Space(10)]
    public GameObject gameplayPauseButton;

    [HideInInspector]
    public List<GameObject> UIObjects = new List<GameObject>();

    void Awake () {
        UIObjects.Add(pauseWindow);
        UIObjects.Add(gameplayGUI);
        UIObjects.Add(MainMenuScreen);
    }

	void Start () {

        ResetUIObjectsPositions();
        UnactivateAll();

        MainMenuScreen.SetActive(true);

	}

    public void UnactivateAll () {
        foreach (GameObject uio in UIObjects) {
            uio.SetActive(false);
        }
    }

    void ResetUIObjectsPositions () {
        foreach (GameObject uio in UIObjects) {
            RectTransform rect = uio.GetComponent<RectTransform>();
            rect.offsetMax = rect.offsetMin = Vector2.zero;
        }
    }

    public void ClosePauseWindow () {
        gameplayPauseButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
        pauseWindow.SetActive(false);
        GameManager.inst.UnpouseGameplay();
    }
	
	void Update () {
	
	}
}
