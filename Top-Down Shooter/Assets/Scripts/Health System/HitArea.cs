using UnityEngine;

//Attached to different parts of the body to register bullet hits
public class HitArea : MonoBehaviour
{
    public Health healthReference;

    public float chancesOfHitting = 0.8f;

    [Space]

    [Range(0, 1)]
    public float[] chancesOfHittingForLimbs;

    public void CallHit(float threat)
    {
        float[] scaledWeights = GetProbabilityGradient(chancesOfHittingForLimbs);
        float random = Random.Range(0f, 1f);

        int len = chancesOfHittingForLimbs.Length;
        for (int i = 0; i < len; i++)
        {
            if (random <= scaledWeights[i])
            {
                healthReference.CallHit(i, threat);
                break;
            }
        }
    }

    //Scales an arry of probabilities into an array from which the points occupy as much space as they have been assigned probability
    float[] GetProbabilityGradient(float[] arr)
    {
        float sum = 0f;
        foreach (float f in arr)
        {
            sum += f;
        }

        float m = 1 / sum;
        int len = arr.Length;
        float[] newArr = new float[len];
        for (int i = 0; i < len; i++)
        {
            newArr[i] = m * arr[i];
        }

        float[] prob = new float[len];
        prob[0] = newArr[0];
        for (int i = 1; i < len - 1; i++)
        {
            prob[i] = prob[i - 1] + newArr[i];
        }
        prob[len - 1] = 1f;

        return prob;
    }
}