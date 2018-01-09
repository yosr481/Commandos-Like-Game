using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BribeButton : MonoBehaviour {

    public string bottleName = "Bottles";
    public string cigaretteName = "cigarettes";
    public Text[] buttons = new Text[2];
    public GameObject throwPointPrefab;

    public int bottlesAmount = 2;
    public int cigarettesAmount = 2;

    // Use this for initialization
    void Start()
    {
        buttons[0].text = bottlesAmount + " " + bottleName;
        buttons[1].text = cigarettesAmount + " " + cigaretteName;
    }

    // Update is called once per frame
    void Update()
    {

        if (bottlesAmount <= 0)
        {
            buttons[0].gameObject.GetComponentInParent<Button>().enabled = false;
        }

        if (cigarettesAmount <= 0)
        {
            buttons[1].gameObject.GetComponentInParent<Button>().enabled = false;
        }

    }

    public void SpawnBottle()
    {
        if (bottlesAmount > 0)
        {
            bottlesAmount--;
            buttons[0].text = bottlesAmount + " " + bottleName;
        }
    }

    public void ThrowCigarette()
    {
        if (cigarettesAmount > 0)
        {
            cigarettesAmount--;
            buttons[1].text = cigarettesAmount + " " + cigaretteName;
            Instantiate(throwPointPrefab);
        }
    }
}
