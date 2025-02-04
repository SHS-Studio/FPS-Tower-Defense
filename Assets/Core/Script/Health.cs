using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enemy;

public class Health : MonoBehaviour
{
    public float Maxhealth;
    public float currenthealth;
    // Start is called before the first frame update
    void Start()
    {
         currenthealth = Maxhealth;
    }

    // Update is called once per frame
    void Update()
    {
         Maxhealth = currenthealth;
    }

    public void TakeDamage(float amount)
    {
        currenthealth -= amount;
        if (Maxhealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        this.gameObject.SetActive(false);
    }


    [Tooltip("HUD HP  to display bullet count on screen. Will be find under name 'HUD_bullets' in scene.")]
    public TextMesh HUD_HP;
    void OnGUI()
    {
        //if (!HUD_HP)
        //{
        //    try
        //    {
        //        HUD_HP = GameObject.Find("HUD_HP").GetComponent<TextMesh>();
        //    }
        //    catch (System.Exception ex)
        //    {
        //        print("Couldnt find the HUD_HP ->" + ex.StackTrace.ToString());
        //    }
        //}
        //if (HUD_HP)
            HUD_HP.text = "HitPoint".ToString() + " - " + currenthealth.ToString();


    }
}
