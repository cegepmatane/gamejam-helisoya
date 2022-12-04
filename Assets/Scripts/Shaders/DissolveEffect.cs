using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private BigfootController bigfoot;

    public float dissolveAmount;
    public float dissolveSpeed;
    public bool isDissolving;

    public void Update()
    {
        if (isDissolving)
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount + dissolveSpeed * Time.deltaTime);
            material.SetFloat("_DissolveAmount", dissolveAmount);
        }
        else
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount - dissolveSpeed * Time.deltaTime);
            material.SetFloat("_DissolveAmount", dissolveAmount);
        }

        if(bigfoot.isDead == true)
        {
            isDissolving = true;
        }



    }

    public void StartDissolve(float dissolveSpeed)
    {
        isDissolving=true;  
        this.dissolveSpeed = dissolveSpeed;
    }

    public void StopDissolve(float dissolveSpeed)
    {
        isDissolving = false;
        this.dissolveSpeed = dissolveSpeed;
    }
}
