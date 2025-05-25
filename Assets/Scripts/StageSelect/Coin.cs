using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    public Transform coin;
    public float flipDuration = 2f;

    public void FlipCoin(int p1Index, int p2Index)
    {
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
        bool isHeads = Random.value < 0.5f;
        int selectedIndex = isHeads ? p1Index : p2Index;

        StageData.selectedStageIndex = selectedIndex;

        StartCoroutine(AnimateCoinFlip(isHeads));
    }


    IEnumerator AnimateCoinFlip(bool isHeads)
    {
        float timer = 0f;
        float flips = 3f; // number of full flips

        float finalAngle = isHeads ? -90f : 90f;

        float totalRotation = flips * 360f + finalAngle; // Rotate full flips plus final angle

        while (timer < flipDuration)
        {
            float t = timer / flipDuration;
            float currentAngle = Mathf.Lerp(0, totalRotation, t);
            coin.rotation = Quaternion.Euler(currentAngle, 0f, 0f);
            timer += Time.deltaTime;
            yield return null;
        }

        // Just ensure it's exactly final rotation at the end (optional)
        coin.rotation = Quaternion.Euler(finalAngle, 0f, 0f);

        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("SampleScene");
    }

}
