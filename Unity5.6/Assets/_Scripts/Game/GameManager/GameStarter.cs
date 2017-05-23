using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //create battleinstance
        BaseBattleInstance inst = new GameBattleInstance();
        inst.Init();
	}
    private float passtime = 0f;
	// Update is called once per frame
	void Update () {
        passtime += Time.deltaTime;
        if (passtime >= 0.03f)
        {
            BaseBattleInstance.Instance.OnStep();
            BaseBattleInstance.Instance.OnPostStep();
        }
	}

    private void OnGUI()
    {
        if (GUILayout.Button("Start"))
        {
            BaseBattleInstance.Instance.Start();
        }
        if(GUILayout.Button("Goblin"))
        {
            FSGameObject gob = new FSGameObject("goblin");
            gob.AddComponent<FSCharacterController>();
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
