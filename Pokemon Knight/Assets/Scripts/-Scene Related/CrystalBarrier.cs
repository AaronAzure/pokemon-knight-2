using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CrystalBarrier : MonoBehaviour
{
    // [SerializeField] private Enemy boss;
    [SerializeField] private string roomName;
    [SerializeField] private GameObject[] barriers;
    [SerializeField] private SpriteRenderer crystalImg;
    [SerializeField] private Sprite brokenCrystal;
    [Space] [SerializeField] private Collider2D col;
    [Space][SerializeField] private GameObject glowObj;
    [SerializeField] private GameObject shatterObj;

    [Space] public bool canBreak;
    public CrystalBarrierSupport csSupport;
    public bool broken; // Ensurance


    void Start() 
    {
        roomName = SceneManager.GetActiveScene().name + " " + this.name;
            
        if (PlayerPrefsElite.VerifyArray("crystalsBroken" + PlayerPrefsElite.GetInt("gameNumber")))
        {
            List<string> crystalsBroken = new List<string>( 
                PlayerPrefsElite.GetStringArray("crystalsBroken" + PlayerPrefsElite.GetInt("gameNumber"))
            );
            
            if (crystalsBroken.Contains(roomName))
                BrokenCrystal(true);
            else
            {
                if (glowObj != null)
                    glowObj.SetActive(true);
                
                if (shatterObj != null)
                    shatterObj.SetActive(false);
            }
        }
        else
        {
            if (glowObj != null)
                glowObj.SetActive(true);
            
            if (shatterObj != null)
                shatterObj.SetActive(false);
        }
    }

    private void BrokenCrystal(bool alreadyBroken=false)
    {
        broken = true;
        if (csSupport != null)
            csSupport.transform.gameObject.SetActive(false);
        if (col != null)
            col.enabled = false;
        foreach (GameObject barrier in barriers)
            barrier.SetActive(false);
        crystalImg.sprite = brokenCrystal;

        if (glowObj != null)
            glowObj.SetActive(false);
        
        if (!alreadyBroken && shatterObj != null)
            shatterObj.SetActive(true);
        else if (alreadyBroken && shatterObj != null)
            shatterObj.SetActive(false);

        this.enabled = false;
    }

    public void BreakCrystal()
    {
        if (canBreak && !broken)
        {
            broken = true;
            List<string> temp = new List<string>(
                PlayerPrefsElite.GetStringArray("crystalsBroken" + PlayerPrefsElite.GetInt("gameNumber")) 
            );
            temp.Add(roomName);
            PlayerPrefsElite.SetStringArray("crystalsBroken" + PlayerPrefsElite.GetInt("gameNumber"), temp.ToArray());

            BrokenCrystal();
        }
    }

}
