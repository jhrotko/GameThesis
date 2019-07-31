using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivingBeing : MonoBehaviour {

    [SerializeField] private float LifeTotal;
    [SerializeField] private Image lifeBar;
    [SerializeField] private Text TextDamage;
    [SerializeField] private Canvas canvasDamage;

    public bool dead;

    protected List<Text> TextDmgLst;

    private float LifeCurrent;
    private float fadeRate = 0.7f;
    
    public void InitializeLife()
    {
        LifeCurrent = LifeTotal;
        UpdateLifeBar();
        dead = false;
        TextDmgLst = new List<Text>();
    }

    public void CreateTextDamage(float damage, string sign = "-")
    {
        Text tDamage = Instantiate(TextDamage);
        tDamage.transform.SetParent(canvasDamage.transform);
        tDamage.GetComponent<RectTransform>().localScale = TextDamage.transform.localScale;
        tDamage.GetComponent<RectTransform>().rotation = TextDamage.transform.rotation;
        tDamage.GetComponent<RectTransform>().position = TextDamage.transform.position;
        tDamage.text = sign + damage;


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

        if (damage > 0.0f)
            CreateTextDamage(damage);
        else if (damage < 0.0f)
        {
            damage = -damage;
            CreateTextDamage(damage, "+");
        }
           
    }

    public bool IsDead()
    {
        return LifeCurrent <= 0.0f;
    }

    public void Die(Animator anim)
    {
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
        Destroy(tDamage.gameObject);
    }

    public void DestroyTexts()
    {
        foreach(Text t in TextDmgLst)
        {
            Destroy(t);
        }
        TextDmgLst.Clear();
    }

    public bool IsOtherClose(float angle, float distance, GameObject other)
    {
        Vector3 playerRelativePos = Vector3.Normalize(other.transform.position - transform.position);
        float playerDistance = Vector3.Distance(other.transform.position, transform.position);
        //Debug.Log("distance "+playerDistance);
        return Vector3.Dot(transform.forward, playerRelativePos) >= angle && playerDistance <= distance;
    }
}
