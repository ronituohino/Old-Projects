using TMPro;
using UnityEngine;

class FPS : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI fpsText;
    [SerializeField] int updateFrameInterval;

    int frame = 0;

    void Update()
    {
        frame++;
        if(frame > updateFrameInterval)
        {
            fpsText.text = (Mathf.RoundToInt(1 / Time.deltaTime)).ToString();
            frame = 0;
        }
    }
}