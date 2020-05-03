using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public Score score;
	public Text scoreText;
	public Text restartText;
	public Text vacuumText;

    // Start is called before the first frame update
    void OnEnable()
    {
		score.score = 0;
		Universe.OnLastInfectedDeath += DisplayRestartPrompt;
		Universe.OnReset += Hide;
		Universe.OnVacuumLearned += DisplayVacuumText;
    }

	void OnDisable()
    {
		Universe.OnLastInfectedDeath -= DisplayRestartPrompt;
		Universe.OnReset -= Hide;
		Universe.OnVacuumLearned -= DisplayVacuumText;
    }

	void Update() {
		scoreText.text = score.score.ToString() + "%";
	}

    void DisplayRestartPrompt()
    {
		restartText.gameObject.SetActive(true);
    }

	void DisplayVacuumText() {
		vacuumText.gameObject.SetActive(true);
	}

	public void Hide() {
		restartText.gameObject.SetActive(false);
		vacuumText.gameObject.SetActive(false);
	}
}
