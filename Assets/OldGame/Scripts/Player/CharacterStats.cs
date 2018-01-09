using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(PlayerControl))]
public class CharacterStats : MonoBehaviour {

    public float health;
    public int team;
    public bool selected;
    public bool run;
    public bool dead;
    public bool crouch;
    public bool isBeenChased;
    public bool isBeenCaught;
    public bool isRiding = false;
    public GameObject selectedCube;
    PlayerControl plControl;

	void Start () {
        plControl = GetComponent<PlayerControl>();
	}
	
	// Update is called once per frame
	void Update () {
        if(selectedCube)
            selectedCube.SetActive(selected);

        if (run) crouch = false;
	}

    public void StartRiding(Transform seatPoint)
    {
        isRiding = true;  
    }

    public void MoveToPosition(Vector3 pos)
    {
        plControl.moveToPosition = true;
        plControl.destPosition = pos;
    }
}

/*[CustomEditor(typeof(CharacterStats))]
public class EnemyAIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CharacterStats characterStats = (CharacterStats)target;

        EditorGUILayout.BeginHorizontal("Box");
        GUILayout.Label("Waypoint count: " + 10);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        characterStats.selected = EditorGUILayout.Toggle("Selected", characterStats.selected);
        if (characterStats.selected)
        {
            base.DrawDefaultInspector();
        }
        EditorGUILayout.EndVertical();
    }
}*/
