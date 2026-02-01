using Pixelplacement;
using UnityEngine;

public class TalkTextController : Singleton<TalkTextController>
{
    public TalkText TalkTextPrefab;

    public static void SpawnTalkText(Vector3 position, string text)
    {
        var instance = Instantiate(Instance.TalkTextPrefab, position, Quaternion.identity);
        instance.SetText(text);
    }
}
