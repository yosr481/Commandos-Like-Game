using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour {

    public bool show = true;
    public GameObject enmUIPrefab;
    GameObject enUI;
    Text textUI;
    EnemyAI enAI;

	// Use this for initialization
	void Start () {
        enAI = GetComponent<EnemyAI>();
        enUI = Instantiate(enmUIPrefab, transform.position, Quaternion.identity) as GameObject;
        enUI.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
        textUI = enUI.GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (show)
        {
            enUI.gameObject.SetActive(true);

            string info = enAI.aiStates.ToString();

            textUI.text = info;

            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
            enUI.transform.position = screenPoint;
        }
        else
        {
            enUI.gameObject.SetActive(false);
        }
	}

    public void EnableDisableUI()
    {
        show = !show;
    }
}
