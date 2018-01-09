using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetUpPoint : MonoBehaviour {

    public Transform seat;

    Horse horse;
    Cart cart;

    void Start()
    {
        horse = FindObjectOfType<Horse>();
        cart = FindObjectOfType<Cart>();
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.GetComponent<CharacterStats>())
        {
            /*CharacterStats currentChar = coll.GetComponent<CharacterStats>();
            currentChar.StartRiding(seat);*/
            MouseManager.RemoveUnitFromSelectedUnit(coll.GetComponent<CharacterStats>());
            MouseManager.RemoveFromOnScreenUnit(coll.GetComponent<CharacterStats>());
            coll.GetComponent<CharacterStats>().isRiding = true;
            Destroy(coll.gameObject, 0.5f);
            cart.rider.SetActive(true);
            horse.playerAtGetUpPoint = true;
        }
    }
}
