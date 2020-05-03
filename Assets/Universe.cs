using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Universe : MonoBehaviour
{
	public Transform board;
	Vector3 boardHalfSize;
	public Particle particlePrefab;
	public ParticleTypes types;
	public ParticleType mainControllableType, altControllableType;
	ParticleType controllableType;
	public ParticleType infectedType;
	public float playerStrength;
	public int particleAmount;
	public Vector3 typesPercentage;
	public float thresholdAccelerationFactor;
	public float friction;
	public float infectionPeriod;
	public float flashTime;
	int infectedTotal, infectedCount;
	float percentInfected;
	public Score score;

	public Transform vacuum;
	public float vacuumDiameter;
	public float vaсuumStrength;
	public int percentToLearnVacuum;
	bool applyVacuum;
	bool vacuumLearned;

	HashSet<Particle> particles = new HashSet<Particle>();

	Camera cam;
	RaycastHit hit;
	bool paused;

    // Start is called before the first frame update
    void Start()
    {
		cam = Camera.main;

		controllableType = mainControllableType;

		vacuum.localScale = new Vector3(vacuumDiameter, 1, vacuumDiameter);

		// Scale the plane to fit the screen size.
		float height = cam.orthographicSize * 2.0f;
		float width = height * Screen.width / Screen.height;
		board.localScale = new Vector3(width / 10f, 1f, height / 10f);

		boardHalfSize = board.localScale * 10 / 2;
		SpawnParticles();
    }

	// void OnDrawGizmos() {
	// 	foreach(Particle particle in particles) {
	// 		Handles.DrawWireDisc(particle.transform.position, Vector3.up, particle.Type.range);
	// 	}
	//
	// 	Handles.color = Color.red;
	// 	foreach(Particle particle in particles) {
	// 		Handles.DrawWireDisc(particle.transform.position, Vector3.up, Type.thresholdRange);
	// 	}
	// }

	public delegate void ResetDelegate();
	public static event ResetDelegate OnReset;

	public void ClearBoard() {
		foreach (Particle particle in particles) {
			Destroy(particle.gameObject);
		}

		particles.Clear();
	}

	void Interface() {
		// Reset.
		if (Input.GetKeyDown(KeyCode.R)) {
			ClearBoard();
			infectedTotal = 0;
			infectedCount = 0;
			controllableType = mainControllableType;
			if (OnReset != null) {
				OnReset();
			}

			SpawnParticles();
		}

		// Freeze.
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (!paused) {
				Time.timeScale = 0;
			} else {
				Time.timeScale = 1;
			}

			paused = !paused;
		}

		// Quit.
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

    // Update is called once per frame
    void Update()
    {
		Interface();

		bool rightButtonHeld = Input.GetMouseButton(1);
		if (rightButtonHeld) {
			Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit);
		}

		if (Input.GetKey(KeyCode.V)) {
			vacuum.gameObject.SetActive(true);
			RaycastHit vacuumHit;
			Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out vacuumHit);
			vacuum.transform.position = vacuumHit.point;

			if (Input.GetMouseButton(0)) {
				applyVacuum = true;
			}
		} else {
			vacuum.gameObject.SetActive(false);
		}

		foreach(Particle aParticle in particles) {
			// Bounce off world borders.
			float particleRadius = aParticle.Type.diameter / 2;
			float signX = Mathf.Sign(aParticle.transform.position.x);
			if (signX * aParticle.transform.position.x > boardHalfSize.x - particleRadius) {
				aParticle.velocity.x *= -1;
				aParticle.transform.position = new Vector3(signX * (boardHalfSize.x - particleRadius),
				aParticle.transform.position.y, aParticle.transform.position.z);
				continue;
			}

			float signZ = Mathf.Sign(aParticle.transform.position.z);
			if (signZ * aParticle.transform.position.z > boardHalfSize.z - particleRadius) {
				aParticle.velocity.z *= -1;
				aParticle.transform.position = new Vector3(aParticle.transform.position.x,
				aParticle.transform.position.y, signZ * (boardHalfSize.z - particleRadius));
				continue;
			}

			// Player Controls.
			if (rightButtonHeld && aParticle.Type == controllableType) {
				Vector3 offset = hit.point - aParticle.transform.position;
				aParticle.velocity += Vector3.Scale(playerStrength * offset, Vector3.one - Vector3.up);
			}

			// Slowdown due friction.
			aParticle.velocity -= friction * aParticle.velocity;

			// Apply vacuum ability.
			if (applyVacuum) {
				Vector3 offset = vacuum.transform.position - aParticle.transform.position;
				if (offset.sqrMagnitude < vacuumDiameter * vacuumDiameter / 4) {
					aParticle.velocity += Vector3.Scale(vaсuumStrength * offset, Vector3.one - Vector3.up);
				}
			}

			// Interact with other particles.
			foreach(Particle bParticle in particles) {
				if (bParticle == aParticle) {
					continue;
				}

				Vector3 offset = bParticle.transform.position - aParticle.transform.position;
				float distance = offset.magnitude;
				Vector3 normalizedOffset = offset / (distance + 0.01f);

				if (distance < (aParticle.Type.diameter + bParticle.Type.diameter) / 2) {
					aParticle.velocity -= normalizedOffset * thresholdAccelerationFactor * aParticle.Type.acceleration * Time.deltaTime;
					bParticle.velocity += normalizedOffset * thresholdAccelerationFactor * bParticle.Type.acceleration * Time.deltaTime;
					continue;
				}

				if (distance < aParticle.Type.range) {
					if (aParticle.Type == infectedType && bParticle.Type != types.grey && bParticle.Type != types.pink) {
						if (Random.Range(0f, 1f) < bParticle.Type.infectionLikelihood * Time.deltaTime) {
							bParticle.Type = infectedType;
							StartCoroutine(InfectedRoutine(bParticle));
						}
					}

					aParticle.velocity += normalizedOffset * aParticle.Type.acceleration * Time.deltaTime *
					types.AccelerationFactorByDistance(distance / aParticle.Type.range, aParticle.Type, bParticle.Type);
				}

				if (distance < bParticle.Type.range) {
					bParticle.velocity -= normalizedOffset * bParticle.Type.acceleration * Time.deltaTime *
					types.AccelerationFactorByDistance(distance / bParticle.Type.range, bParticle.Type, aParticle.Type);
				}
			}
		}

		applyVacuum = false;
    }

	IEnumerator InfectedRoutine(Particle particle) {
		infectedTotal++;
		percentInfected = Mathf.Round(100 * (float)infectedTotal / (float)particleAmount);
		score.score = (int)percentInfected;

		if (percentInfected >= percentToLearnVacuum && !vacuumLearned) {
			vacuumLearned = true;
			if (OnVacuumLearned != null) {
				OnVacuumLearned();
			}
		}

		infectedCount++;
		// Debug.Log(infectedCount);

		yield return new WaitForSeconds(infectionPeriod - flashTime);
		if (particle == null) { yield break; }
		particle.renderer.material.color = Color.white;

		yield return new WaitForSeconds(flashTime);
		if (particle == null) { yield break; }
		particle.Type = types.grey;

		infectedCount--;
		// Debug.Log(infectedCount);

		if (infectedCount <= 0) {
			controllableType = altControllableType;
			if (OnLastInfectedDeath != null) {
				OnLastInfectedDeath();
			}
		}
	}

	public delegate void DeathDelegate();
	public static event DeathDelegate OnLastInfectedDeath;

	public delegate void VacuumGrantedDelegate();
	public static event VacuumGrantedDelegate OnVacuumLearned;

	void SpawnParticles() {
		Vector3 eachTypeAmount = typesPercentage * particleAmount;
		for (int i = 0; i < 3; i++) {
			ParticleType currType = types.amber;
			switch(i) {
				case 0:
					currType = types.amber;
					break;
				case 1:
					currType = types.teal;
					break;
				case 2:
					currType = types.pink;
					break;
			}

			for (int j = 0; j < eachTypeAmount[i]; j++) {
				Particle particleInstance = Instantiate(particlePrefab, Vector3.zero, Quaternion.identity);
				particleInstance.Type = currType;
				particles.Add(particleInstance);

				float xPos = Random.Range(-boardHalfSize.x, boardHalfSize.x);
				float zPos = Random.Range(-boardHalfSize.z, boardHalfSize.z);
				particleInstance.transform.position = new Vector3(xPos, 0, zPos) * 0.9f;

				if (currType == types.pink) {
					particleInstance.transform.position *= 0.2f;
					StartCoroutine(InfectedRoutine(particleInstance));
				}
			}
		}
	}
}
