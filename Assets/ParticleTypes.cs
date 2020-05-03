using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ParticleTypes : ScriptableObject
{
	public ParticleType amber;
	public ParticleType teal;
	public ParticleType pink;
	public ParticleType grey;

	public AnimationCurve accelerationByDistanceAA;
	public AnimationCurve accelerationByDistanceAT;
	public AnimationCurve accelerationByDistanceAP;

	public AnimationCurve accelerationByDistanceTA;
	public AnimationCurve accelerationByDistanceTT;
	public AnimationCurve accelerationByDistanceTP;

	public AnimationCurve accelerationByDistancePA;
	public AnimationCurve accelerationByDistancePP;
	public AnimationCurve accelerationByDistancePT;

	public float AccelerationFactorByDistance(float normDist, ParticleType iType, ParticleType jType) {
		if (iType == amber) {
			if (jType == amber) {
				return amber.acceleration * accelerationByDistanceAA.Evaluate(normDist);
			}

			if (jType == teal) {
				return amber.acceleration * accelerationByDistanceAT.Evaluate(normDist);
			}

			if (jType == pink) {
				return amber.acceleration * accelerationByDistanceAP.Evaluate(normDist);
			}
		} else if (iType == teal) {
			if (jType == amber) {
				return amber.acceleration * accelerationByDistanceTA.Evaluate(normDist);
			}

			if (jType == teal) {
				return amber.acceleration * accelerationByDistanceTT.Evaluate(normDist);
			}

			if (jType == pink) {
				return amber.acceleration * accelerationByDistanceTP.Evaluate(normDist);
			}
		} else if (iType == pink) {
			if (jType == amber) {
				return amber.acceleration * accelerationByDistancePA.Evaluate(normDist);
			}

			if (jType == teal) {
				return amber.acceleration * accelerationByDistancePT.Evaluate(normDist);
			}

			if (jType == pink) {
				return amber.acceleration * accelerationByDistancePP.Evaluate(normDist);
			}
		}

		return 0;
	}
}
