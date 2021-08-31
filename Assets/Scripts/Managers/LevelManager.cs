using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MenuController;

public class LevelManager
{
    public static List<Level> listeLevels;

    // Start is called before the first frame update
    public static void InitLevelManager()
    {
        Level.ResetIndex();

        listeLevels = new List<Level>()
        {
            new Level("Level_1",  0, false, Tutoriel.Win),
            new Level("Level_2",  0, false, Tutoriel.Jump),
            new Level("Level_3",  0, false, Tutoriel.Water),
            new Level("Level_4",  0, false, Tutoriel.Kill),
            new Level("Level_5",  1,  false,  Tutoriel.Block),
            new Level("Level_6",  0, false, Tutoriel.Checkpoint),
            new Level("Level_7",  0, false, Tutoriel.Key),
            new Level("Level_8",  0, false, Tutoriel.Teleport),
            new Level("Level_9",  0, true, Tutoriel.Gravity),
            new Level("Level_10", 1, true),
            new Level("Level_11", 0, false),
            new Level("Level_12", 0, true),
            new Level("Level_13", 3, false)
        };
    }

    public static Level GetLevel(string szSceneName)
    {
        if (szSceneName == "Level_Test")
            return new Level("Level_Test", 9, false);

        List<Level> lvlSearch = listeLevels.Where(lvl => lvl.SceneName == szSceneName).ToList();

        if (lvlSearch.Count > 0)
            return lvlSearch[0];
        else
            return null;
    }

    public static Level GetNextLevel(Level lvlCurrent)
    {
        List<Level> lvlSearch = listeLevels.Where(lvl =>
        {
            if (lvlCurrent.Index >= Level.Level_Index)
                return lvl.Index == lvlCurrent.Index;
            else
                return lvl.Index == lvlCurrent.Index + 1;
        }).ToList();

        return lvlSearch[0];
    }
}

public class Level
{
    public static int Level_Index = -1;

    public bool CapacityBlock
    {
        get; set;
    }

    public int CapacityBlockCount
    {
        get;set;
    }

    public bool CapacityGravity
    {
        get; set;
    }

    public string SceneName
    {
        get;set;
    }

    private int nIndex;
    public int Index
    {
        get
        {
            return nIndex;
        }
    }

    private Tutoriel enumTutoriel;
    public Tutoriel Tutoriel
    {
        get
        {
            return enumTutoriel;
        }
    }

    public Level(string szSceneName, int nCapacityBlock, bool bGravityCapacity)
    {
        CapacityBlock = nCapacityBlock > 0;
        CapacityBlockCount = nCapacityBlock;
        CapacityGravity = bGravityCapacity;
        SceneName = szSceneName;

        Level_Index++;
        nIndex = Level_Index;

        enumTutoriel = Tutoriel.Aucun;
    }

    public Level(string szSceneName, int nCapacityBlock, bool bGravityCapacity, Tutoriel tuto)
    {
        CapacityBlock = nCapacityBlock > 0;
        CapacityBlockCount = nCapacityBlock;
        CapacityGravity = bGravityCapacity;
        SceneName = szSceneName;

        Level_Index++;
        nIndex = Level_Index;

        enumTutoriel = tuto;
    }

    public static void ResetIndex()
    {
        Level.Level_Index = -1;
    }
}