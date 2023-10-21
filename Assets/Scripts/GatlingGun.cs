using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingGun : MonoBehaviour
{
   public float baseRotationSpeed = 1.0f;
    public float clickHoldMultiplier = 2.0f;
    public float decreaseRate = 0.1f;
    public GameObject[] objectsToRotate;

    private bool isClicking = false;
    public float currentSpeed;
    public float increase;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isClicking = true;
            currentSpeed = baseRotationSpeed * clickHoldMultiplier;
        }
        
      
        if (Input.GetMouseButtonUp(0))
        {
            isClicking = false;
        }

        if (!isClicking && currentSpeed > baseRotationSpeed)
        {
            increase = Mathf.Lerp(increase, 0, Time.deltaTime * decreaseRate);
            //increase -= Time.deltaTime * decreaseRate;
            //if (increase < 0) increase = 0;
        }
        else
        {
            increase += Time.deltaTime * currentSpeed;
            if (increase > 360) increase = 0;
        }

        foreach (GameObject obj in objectsToRotate)
        {
            obj.transform.localRotation = Quaternion.Euler(new Vector3(increase, -90f, -90f));
        }
    }
}
