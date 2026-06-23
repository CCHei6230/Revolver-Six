using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MeshTrail : MonoBehaviour
{

    public static IEnumerator IEnumerator_Trail(GameObject _ToMakeTrail,int _activeFrame , int _meshRefreshTime , float _destroyTime ,Material _material = null)
    {
        GameObject _trailObj = new GameObject("TrailObj  " + _ToMakeTrail.name);
        MeshFilter[] meshFilters = _ToMakeTrail.GetComponentsInChildren<MeshFilter>();
        Vector3[] scales = new Vector3[meshFilters.Length];
        Vector3[] positions = new Vector3[meshFilters.Length];
        Quaternion[] rotations = new Quaternion[meshFilters.Length];

        for (int i = _activeFrame; i >0; i--)
        {
            if (meshFilters[0] == null)
            {
                Destroy(_trailObj,5f);
                yield break;
            }
            if (i % _meshRefreshTime == 0)
            {
                for (int j = 0; j < meshFilters.Length; j++)
                {
                    scales[j] = meshFilters[j].transform.lossyScale;
                    positions[j] = meshFilters[j].transform.position;
                    rotations[j] = meshFilters[j].transform.rotation;
                }
                for (int j = 0; j < meshFilters.Length; j++)
                {
                    GameObject meshObj = new GameObject();
                    meshObj.transform.SetParent(_trailObj.transform);
                    meshObj.transform.position = positions[j];
                    meshObj.transform.rotation = rotations[j];
                    meshObj.transform.localScale = scales[j] ;
                    MeshRenderer mR =  meshObj.AddComponent<MeshRenderer>();
                    MeshFilter mF = meshObj.AddComponent<MeshFilter>();
                    Mesh mesh = meshFilters[j].mesh;

                    mF.mesh = mesh;

                    mR.material = _material;

                    Destroy(meshObj,_destroyTime);
                }

            }

            yield return new WaitForFixedUpdate();
        }

        Destroy(_trailObj,5f);
    }

}
