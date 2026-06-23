using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

public class BlockManager : MonoBehaviour
{
    [SerializeField] private GameObject[] blocks;
    [SerializeField] private int duration;
    void Start()
    {

        foreach (var obj in blocks)
        {
            obj.SetActive(false);
        }

        StartCoroutine(IEnumerator_BlockActiveLoop());
    }

    IEnumerator IEnumerator_ScaleDown( Transform _transform,int _duration )
    {

        var tmp_scale = 0f;
        for (int i = 0; i < _duration; i++)
        {
            tmp_scale = 1f - ((float)i / (float)duration);
            _transform.localScale = Vector3.one *tmp_scale;
            yield return new WaitForFixedUpdate();
        }

        _transform.localScale = Vector3.one;
    }

    IEnumerator IEnumerator_BlockActiveLoop()
    {

        while (true)
        {
            for(int i = 0; i < blocks.Length; i++)
            {

                blocks[i].SetActive(true);

                for(int j = 0; j < duration; j++)
                {
                    yield return new WaitForFixedUpdate();
                }

                StartCoroutine(
                    IEnumerator_ScaleDown(blocks[i].transform.GetChild(0),duration)
                );

                var tmp_num = i-1 < 0 ?blocks.Length-1 : i-1;
                if (blocks[tmp_num].activeSelf)
                {
                    blocks[tmp_num].SetActive(false);
                }
            }
        }
    }
}
