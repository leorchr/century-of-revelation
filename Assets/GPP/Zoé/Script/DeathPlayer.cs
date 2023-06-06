using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlayer : MonoBehaviour
{
    public float timeToRespawn;
    public void Death()
    {
        Invoke("Respawn", timeToRespawn);
        Mask.instance.timeRemaining = Mask.instance.duration;
        FadeInFadeOut.instance.FadeIn();
        Mask.instance.maskStatus = MaskStatus.Full;
        //SFX

    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Void" || other.gameObject.tag == "Enemy")
        {
            Debug.Log("touch�");
            Death();
        }
    }

    public void Respawn()
    {
        PlayerController.instance.GetComponent<Rigidbody>().position = PlayerController.instance.respawnPosition;
        PlayerVFX.instance.DeathParticles();
    }
}
