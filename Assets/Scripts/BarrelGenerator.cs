using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarrelGenerator : MonoBehaviour {

    private static BarrelGenerator instance = null;
    public static BarrelGenerator inst {
        get { if (!instance) instance = GameObject.FindObjectOfType<BarrelGenerator>(); return instance; }
    }

    Vector3 nextBarrelPosition;

	void Start () {
        nextBarrelPosition = this.transform.position;
	}
	
	void Update () {
	
	}

    public void GenerateBarrel (int difficulty) {

        List<System.Func<GameObject>> functionsList = new List<System.Func<GameObject>>();
        functionsList.Add(() => BarrelPresets.inst.InstantiateStepRotating(nextBarrelPosition, Quaternion.identity, difficulty, false));
        functionsList.Add(() => BarrelPresets.inst.InstantiatePathThreePointsVertical(nextBarrelPosition, Quaternion.identity, difficulty, false));
        functionsList.Add(() => BarrelPresets.inst.InstantiatePathSwingVertical(nextBarrelPosition, Quaternion.identity, difficulty, false));

        GameObject newBarrel = functionsList[Random.Range(0, functionsList.Count)]();
        //GameObject newBarrel = functionsList[0]();

        newBarrel.transform.parent = this.gameObject.transform;

        Barrel newBarrelScript = newBarrel.GetComponent<Barrel>();
        newBarrelScript.difficulty = difficulty;
        GameManager.inst.barrels.Add(newBarrel);

        nextBarrelPosition = newBarrelScript.NextBarrelPosition();
    }
}
