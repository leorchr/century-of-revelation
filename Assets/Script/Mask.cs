using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

public enum MaskStatus
{
    Full, Using, Empty, Charging
}

public class Mask : MonoBehaviour
{
    public static Mask instance;
    private bool isInsidePlat;

    private float timeActivation;
    public float duration;

    //private float speedRecharging; // ajout de la speed plus tard
    private float rechargingActivation;

    [SerializeField] private float cooldown;
    private float cooldownRemaining;
    private float cooldownActivation;
    private bool ableToUse;

    public float timeRemaining;
    private float currentTimeRemaining;
    [HideInInspector] public MaskStatus maskStatus = MaskStatus.Full;

    public float speedReduction;

    /*public float energy;
    public bool isEnergy;
    GameObject energyOrb;
*/
    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip[] clip;


    PlatformVisibility[] allPlatforms;
    bool maskPlatforms = true;
    private void Awake()
    {
        if (instance) Destroy(this);
        else instance = this;
    }

    private void Start()
    {
        allPlatforms = FindObjectsOfType<PlatformVisibility>();
        foreach (PlatformVisibility platform in allPlatforms)
        {
            platform.ShowMaskPlatforms(maskPlatforms);
        }
        maskStatus = MaskStatus.Full;
        timeRemaining = duration;
        ableToUse = true;

    }
    

    private void Update()
    {
        //Debug.Log("TimeR :" + timeRemaining);
        switch (maskStatus)
        {
            case MaskStatus.Using:                
                timeRemaining = duration - (duration - currentTimeRemaining) - (Time.time - timeActivation);
                //AudioManager.instance.PlaySFX(clip[0], audioSource);
                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    PlateformOff();
                    maskStatus = MaskStatus.Empty;
                    cooldownActivation = Time.time;
                    ableToUse = false;
                }
                break;
            case MaskStatus.Empty:
                timeRemaining = 0;
                cooldownRemaining = cooldown - (Time.time - cooldownActivation);
                PlayerController.instance.sprintSpeed = speedReduction;
                PlayerController.instance.walkSpeed = speedReduction;
                PlayerController.instance.moveSpeed = PlayerController.instance.walkSpeed;
                if (cooldownRemaining <= 0)
                {
                    rechargingActivation = Time.time;
                    maskStatus = MaskStatus.Charging;
                }
                break;
            case MaskStatus.Charging:
                timeRemaining = Time.time - rechargingActivation;
                if (timeRemaining >= duration)
                {
                    timeRemaining = duration;
                    maskStatus = MaskStatus.Full;
                    ableToUse = true;
                    PlayerController.instance.sprintSpeed = 8f;
                    PlayerController.instance.walkSpeed = 4.5f;
                    PlayerController.instance.moveSpeed = PlayerController.instance.walkSpeed;
                }
                break;
            case MaskStatus.Full:
                break;
        }
    }
    public void PlateformOn()
    {

        timeActivation = Time.time;
        currentTimeRemaining = timeRemaining;
        maskStatus = MaskStatus.Using;
        maskPlatforms = !maskPlatforms;
        foreach (PlatformVisibility platform in allPlatforms)
        {
            platform.ShowMaskPlatforms(maskPlatforms);
        }

    }

    public void PlateformOff()
    {
        maskPlatforms = !maskPlatforms;
        foreach (PlatformVisibility platform in allPlatforms)
        {
            platform.ShowMaskPlatforms(maskPlatforms);
        }
    }

    public void ResetPlatforms()
    {
        timeRemaining = duration;
        maskStatus = MaskStatus.Full;
        foreach (PlatformVisibility platform in allPlatforms)
        {
            platform.ResetMaskPlatforms();
        }
        maskPlatforms = true;
        ableToUse = true;
    }

    public void UseMask(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (ableToUse && !isInsidePlat)
            {
                if (maskStatus == MaskStatus.Full || maskStatus == MaskStatus.Charging)
                {
                    PlateformOn();
                }
                else if (maskStatus == MaskStatus.Using)
                {
                    PlateformOff();
                    rechargingActivation = Time.time - timeRemaining;
                    maskStatus = MaskStatus.Charging;
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Platform")
        {
            isInsidePlat = true;
        }
        /*else if (other.gameObject.tag == "Energy")
        {
            isEnergy = true;
            Debug.Log(isEnergy);
            energyOrb = other.gameObject;
            //PlusEnergy();
        }*/
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Platform")
        {
            isInsidePlat = false;
        }
        /*else if(other.gameObject.tag == "Energy")
        {
            isEnergy = false;
        }*/
    }

    /*public void PlusEnergy()
    {
        if(timeRemaining < duration)
        {
            timeRemaining = timeRemaining + energy;
            
            if (timeRemaining >= duration)
            {
                timeRemaining = duration;
            }
            Debug.Log(isEnergy);
            isEnergy = false;
            Destroy(energyOrb);
        }
        else if (timeRemaining == duration)
        {
            return;
        }
    }*/
}
