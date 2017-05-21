using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //create battleinstance
        BaseBattleInstance inst = new GameBattleInstance();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        if (GUILayout.Button("Start"))
        {
            BaseBattleInstance.Instance.Start();
        }
        if(GUILayout.Button("Goblin"))
        {
            //spawn goblin
        }
        if (GUILayout.Button("boar knight"))
        {
            //spawn boar knight
        }
        if (GUILayout.Button("giant"))
        {
            //spawn giant
        }
        if (GUILayout.Button("fly dragon"))
        {
            //spawn fly dragon
        }
    }
}
