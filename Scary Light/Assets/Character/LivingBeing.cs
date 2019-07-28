using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivingBeing : MonoBehaviour {

    public float LifeTotal;
    public Image lifeBar;
    public bool dead;
    public Text TextDamage;
    public Canvas canvasDamage;

    public float LifeCurrent;
    protected List<Text> TextDmgLst;
    private float fadeRate = 0.7f;
    
    public void InitializeLife()
    {
        //LifeTotal = LifeTotal;
        LifeCurrent = LifeTotal;
        UpdateLifeBar();
        dead = false;
        TextDmgLst = new List<Text>();
    }

    public void CreateTextDamage(float damage)
    {
        Text tDamage = Instantiate(TextDamage);
        tDamage.transform.SetParent(canvasDamage.transform);
        tDamage.GetComponent<RectTransform>().localScale = TextDamage.transform.localScale;
        tDamage.GetComponent<RectTransform>().rotation = TextDamage.transform.rotation;
        tDamage.GetComponent<RectTransform>().position = TextDamage.transform.position;
        tDamage.text = "-" + damage;


        StartCoroutine(FadeOut(tDamage));
    }

    public void UpdateLifeBar()
    {
        lifeBar.fillAmount = LifeCurrent / LifeTotal;
    }

    public void UpdateLife(float damage)
    {
        LifeCurrent -= damage;
        UpdateLifeBar();

        if(damage > 0.0f)
            CreateTextDamage(damage);
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

    public IEnumerator FadeOut(Text tDamage)
    {
        float startAlpha = tDamage.color.a;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tmpColor = tDamage.color;
            tDamage.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp(startAlpha, 0, progress));
            progress += fadeRate * Time.deltaTime;
            yield return null;
        }
        //TextDmgLst.Add(tDamage);
        Debug.Log("Destroying ");
        Destroy(tDamage.gameObject);
        //TextDmgLst.Add(tDamage); 
    }

    public void DestroyTexts()
    {
        foreach(Text t in TextDmgLst)
        {
            Destroy(t);
        }
        TextDmgLst.Clear();
    }
}
