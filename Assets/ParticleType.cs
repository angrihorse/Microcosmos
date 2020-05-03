using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ParticleType : ScriptableObject{
	public float range;
	public float acceleration;
	public float infectionLikelihood;
	public float diameter;
	public Material material;
}
