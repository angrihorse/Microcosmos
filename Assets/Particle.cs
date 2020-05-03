using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
	public Vector3 velocity;

	ParticleType type;
	public ParticleType Type {
		get { return type; }
		set {
			if (type != null) {
				StartCoroutine(TransitionIntoType(type, value));
			} else {
				renderer.material = value.material;
				transform.localScale = Vector3.one * value.diameter;
			}

			type = value;
		}
	}

	public Renderer renderer;
	public AnimationCurve typeTransition;

	IEnumerator TransitionIntoType(ParticleType startType, ParticleType endType) {
		yield return new WaitForSeconds(0.2f);
		float progress = 0;
		while (progress < 1f) {
			progress += Time.deltaTime;
			transform.localScale = Vector3.one * Mathf.Lerp(startType.diameter, endType.diameter, typeTransition.Evaluate(progress));
			renderer.material.color = Color.Lerp(startType.material.color, endType.material.color, typeTransition.Evaluate(progress));
			yield return null;
		}
	}

    // Update is called once per frame
    void Update()
    {
		transform.position += velocity * Time.deltaTime;
    }
}
