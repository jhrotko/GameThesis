using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivingBeing : MonoBehaviour {

    public float LifeTotal;
    public float LifeCurrent;
    public Image lifeBar;
    public bool dead;
    
    public void InitializeLife(float total)
    {
        LifeTotal = total;
        LifeCurrent = total;
        UpdateLifeBar();
        dead = false;
    }

    public void UpdateLifeBar()
    {
        lifeBar.fillAmount = LifeCurrent / LifeTotal;
    }

    public void UpdateLife(float damage)
    {
        LifeCurrent -= damage;
        UpdateLifeBar();
    }

    public bool IsDead()
    {
        return LifeCurrent <= 0.0f;
    }

    public void Die(Animator anim)
    {
        //anim.SetTrigger("Dead");
        if(!dead)
            anim.Play("Dead", 0);
        dead = true;
    }

}
