using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Cooldown : MonoBehaviour
{
    public TMP_Text MainText;
    public float startingCD;

    private float currentCD;
    private string startingText;
    private WaitForSeconds oneSecond = new WaitForSeconds(1f);

    public void StartCooldown()
    {
        StartCoroutine(CountDown());
    }

    private void Awake()
    {
        startingText = MainText.text;
    }

    private IEnumerator CountDown()
    {
        currentCD = startingCD;

        while(currentCD > 0)
        {
            MainText.text = currentCD.ToString();
            yield return oneSecond;
            currentCD--;
        }
        MainText.text = startingText;
    }
}
