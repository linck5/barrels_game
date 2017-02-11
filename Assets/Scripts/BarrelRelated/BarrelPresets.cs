using UnityEngine;
using System.Collections;

public class BarrelPresets : MonoBehaviour {

    public GameObject barrelPF;

    private static BarrelPresets instance = null;
    public static BarrelPresets inst {
        get { if (!instance) instance = GameObject.FindObjectOfType<BarrelPresets>(); return instance; }
    }


    public GameObject InstantiatePathSwingVertical (Vector3 position, Quaternion rotation, int difficulty, bool activate) {

        float pointsSpacingMin = 3f;
        float pointsSpacingMax = 15f;

        float pointsSpacing = Random.Range(pointsSpacingMin, pointsSpacingMax);

        GameObject newBarrel = Instantiate(barrelPF, position, rotation) as GameObject;

        Barrel barrelScript = newBarrel.GetComponent<Barrel>();

        PathMoveBehaviour pathScript = newBarrel.AddComponent<PathMoveBehaviour>();

        pathScript.initialAngle = 0f;

        pathScript.delayBetweenMoves = 0f;
        pathScript.moveDuration = DifficultyManager.MoveDurationBasedOnDifficulty();

        pathScript.pathPoints.Add(new Vector3(position.x, position.y, position.z));
        pathScript.pathPoints.Add(new Vector3(position.x, position.y + pointsSpacing * (Random.value > 0.5f? 1: -1), position.z));

        barrelScript.NextBarrelPosition = () =>
        {
            float nextBarrelRelativeXRange = Random.Range(5, 15);
            float nextBarrelY = pathScript.pathPoints[Random.Range(0, pathScript.pathPoints.Count)].y;

            return new Vector3(position.x + nextBarrelRelativeXRange, nextBarrelY, 0);
        };

        if (activate) {
            pathScript.Activate();
        }

        return newBarrel;
    }

    public GameObject InstantiatePathThreePointsVertical (Vector3 position, Quaternion rotation, int difficulty, bool activate) {

        float pointsSpacingMin = 2f;
        float pointsSpacingMax = 8f;


        float pointsSpacing = Random.Range(pointsSpacingMin, pointsSpacingMax);

        GameObject newBarrel = Instantiate(barrelPF, position, rotation) as GameObject;

        Barrel barrelScript = newBarrel.GetComponent<Barrel>();

        PathMoveBehaviour pathScript = newBarrel.AddComponent<PathMoveBehaviour>();

        pathScript.initialAngle = 0f;


        pathScript.moveDuration = DifficultyManager.MoveDurationBasedOnDifficulty();
        pathScript.delayBetweenMoves = DifficultyManager.DelayBetweenMovesBasedOnDifficulty();

        int firstMoveDirection = Random.value > 0.5f ? 1 : -1;

        pathScript.pathPoints.Add(new Vector3(position.x, position.y, position.z));

        bool firstPositionIsCentralPoint = Random.value > 0.5f;

        if (firstPositionIsCentralPoint) {
            pathScript.backAndForth = false; //it will be virtually back and forth because of the path points
            pathScript.pathPoints.Add(new Vector3(position.x, position.y + pointsSpacing * firstMoveDirection, position.z));
            pathScript.pathPoints.Add(new Vector3(position.x, position.y, position.z));
            pathScript.pathPoints.Add(new Vector3(position.x, position.y + pointsSpacing * -firstMoveDirection, position.z));
        }
        else {
            pathScript.backAndForth = true;
            pathScript.pathPoints.Add(new Vector3(position.x, position.y + pointsSpacing * firstMoveDirection, position.z));
            pathScript.pathPoints.Add(new Vector3(position.x, position.y + pointsSpacing * firstMoveDirection * 2, position.z));
        }


        barrelScript.NextBarrelPosition = () =>
        {
            float nextBarrelRelativeXRange = Random.Range(5, 15);
            float nextBarrelY = pathScript.pathPoints[Random.Range(0, pathScript.pathPoints.Count)].y;

            return new Vector3(position.x + nextBarrelRelativeXRange, nextBarrelY, 0);
        };

        if (activate) {
            pathScript.Activate();
        }

        return newBarrel;
    }

    public GameObject InstantiateStepRotating (Vector3 position, Quaternion rotation, int difficulty, bool activate) {

        GameObject newBarrel = Instantiate(barrelPF, position, rotation) as GameObject;

        Barrel barrelScript = newBarrel.GetComponent<Barrel>();

        RotateMoveBehaviour rotateScript = newBarrel.AddComponent<RotateMoveBehaviour>();

        rotateScript.backAndForth = true;

        rotateScript.rotationDuration = DifficultyManager.MoveDurationBasedOnDifficulty();
        rotateScript.delayBetweenMoves = DifficultyManager.DelayBetweenMovesBasedOnDifficulty();

        int steps = Random.Range(3, 5);
        float[] angleRange = {-45, 45};

        if(angleRange[0] > angleRange[1]){
            float aux = angleRange[0];
            angleRange[0] = angleRange[1];
            angleRange[1] = aux;
        }

        float stepAdder = (angleRange[1] - angleRange[0]) / (steps - 1);

        bool reverse = Random.value > 0.5f;
        for (int i = 0; i < steps; i++) {
            int multiplier = reverse ? steps - 1 - i : i;
            rotateScript.angles.Add(angleRange[0] + stepAdder * multiplier);
        }

        rotateScript.initialAngle = rotateScript.angles[0];

        barrelScript.NextBarrelPosition = () =>
        {
            int randomIndex = (int) Mathf.Floor(Random.Range(0, rotateScript.angles.Count));
            float directionAngleRad = rotateScript.angles[randomIndex] * Mathf.Deg2Rad;
            float distance = Random.Range(8, 12);

            return new Vector3(
                position.x + Mathf.Cos(directionAngleRad) * distance, 
                position.y + Mathf.Sin(directionAngleRad) * distance, 
                0
                );
        };

        if (activate) {
            rotateScript.Activate();
        }

        return newBarrel;
    }

    
}
