using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    IEnumerator FireBallTimer()
    {
        yield return new WaitForSeconds(5.0f);
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        StartCoroutine(FireBallTimer());
    }
}
