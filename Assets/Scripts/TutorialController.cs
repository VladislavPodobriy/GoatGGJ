using System.Collections;
using Pixelplacement;
using UnityEngine;

public class TutorialController : Singleton<TutorialController>
{
    public GameObject First;
    public GameObject Second;
    public GameObject Third;

    private void Start()
    {
        StartCoroutine(Show());
    }

    public void ShowThird()
    {
        Third.SetActive(true);
    }

    public IEnumerator Show()
    {
        yield return new WaitForSeconds(5);
        First.SetActive(true);
        yield return new WaitForSeconds(2);
        Second.SetActive(true);
    }
}
