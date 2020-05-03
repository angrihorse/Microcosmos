using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OkayButton : MonoBehaviour
{
	public UIManager UIManager;
	public GameObject mainUniverse;
	public GameObject tutorialUniverse;
	public GameObject tutorialText;
	public GameObject madeByText;

    // Start is called before the first frame update
    public void ActivateUniverse()
    {
		tutorialUniverse.GetComponent<Universe>().ClearBoard();
		UIManager.Hide();
		tutorialUniverse.SetActive(false);
		mainUniverse.SetActive(true);
		tutorialText.SetActive(false);
		madeByText.SetActive(false);
		gameObject.SetActive(false);
    }
}
