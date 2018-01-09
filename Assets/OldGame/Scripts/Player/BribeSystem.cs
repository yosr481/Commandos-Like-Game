using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BribeSystem : MonoBehaviour {
    /*
     * Instances of object to bribe based on quantity
     * Every bribe object contains costumized POI script
     * Throw function based on mouse click, with indicating rainbow for Cigarettes
     * putting function for drink bottle.
     * Connection to enemy system & make them react to bribe
     * stopwatch for clear area before enemies comeback

     */
    public int amount = 2;
    public GameObject throwPoint;
    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ThrowCigarettes()
    {
        /*
         * Range of throwing
         * Calculating rainbow presentation
         * animations
         */
        if (amount > 1)
        {
            amount--;
            throwPoint.SetActive(true);
        }
    }

    public void PuttingDrinks()
    {
        
    }
}
