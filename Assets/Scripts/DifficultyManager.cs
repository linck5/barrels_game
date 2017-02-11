using System;
using System.Collections.Generic;

class DifficultyManager
{
    public static float delayBetweenMovesMin = 0.2f;
    public static float delayBetweenMovesMax = 0.7f;

    public static float moveDurationMin = 0.1f;
    public static float moveDurationMax = 0.6f;


    public const int MIN_DIFFICULTY = 1;
    public const int MAX_DIFFICULTY = 1000;
    public static int currentDifficulty = MIN_DIFFICULTY;
    public static int barrelsBeatRecord = 1;
    public const int TOP_DIFFICULTY_BARRELS_BEAT = 1000; //7200
    
    public static float DelayBetweenMovesBasedOnDifficulty () {
        return ValueBasedOnDifficulty(DifficultyManager.currentDifficulty, delayBetweenMovesMax, moveDurationMin);
    }
    
    public static float MoveDurationBasedOnDifficulty () {
        return ValueBasedOnDifficulty(DifficultyManager.currentDifficulty, moveDurationMax, moveDurationMin);
    }

    static public float ValueBasedOnDifficulty (int difficulty, float easiestValue, float hardestValue) {
        float difficultyProgress = Utils.ValueProgress(DifficultyManager.MIN_DIFFICULTY, DifficultyManager.MAX_DIFFICULTY, difficulty);
        return easiestValue + (hardestValue - easiestValue) * difficultyProgress;
    }

    public static void UpdateDifficulty () {
        if (GameManager.inst.barrelsBeat + GameManager.inst.initialBarrelsSpawned >= DifficultyManager.TOP_DIFFICULTY_BARRELS_BEAT) {
            DifficultyManager.currentDifficulty = DifficultyManager.MAX_DIFFICULTY;
            return;
        }

        float difficultyProgress = 
            Utils.ValueProgress(1, TOP_DIFFICULTY_BARRELS_BEAT, GameManager.inst.barrelsBeat + GameManager.inst.initialBarrelsSpawned);

        currentDifficulty = (int)Utils.EaseValue(
            DifficultyManager.MIN_DIFFICULTY,
            DifficultyManager.MAX_DIFFICULTY,
            (difficultyProgress * (DifficultyManager.MAX_DIFFICULTY - DifficultyManager.MIN_DIFFICULTY)) + DifficultyManager.MIN_DIFFICULTY,
            Ease.QuadOut
            );

        if (DifficultyManager.currentDifficulty < DifficultyManager.MIN_DIFFICULTY) {
            DifficultyManager.currentDifficulty = DifficultyManager.MIN_DIFFICULTY;
        }
        if (DifficultyManager.currentDifficulty > DifficultyManager.MAX_DIFFICULTY) {
            DifficultyManager.currentDifficulty = DifficultyManager.MAX_DIFFICULTY;
        }
            
    }
}
