using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;


//[System.Serializable]
//public class SplinePathData
//{
//    public SliceData[] slices;
//}
//
//[System.Serializable]
//public class SliceData
//{
//    public int splineIndex;
//    public SplineRange range;
//
//    // can store more useful info
//    public bool isEnabled = true;
//    public float sliceLength;
//    public float distanceFromStart;
//
//
//}
public class ShortcutPathing : MonoBehaviour
{
    [SerializeField] SplineContainer container;
    [SerializeField] float speed = 25f;
    //[SerializeField] SplinePathData pathData;

    SplinePath path;

    float progressRatio;
    float progress;
    float totalLength;



    // Start is called before the first frame update
    void Start()
    {
        Matrix4x4 localToWorldMatrix = container.transform.localToWorldMatrix;

        //List<SliceData> enabledSlices = pathData.slices.Where(slice => slice.isEnabled).ToList();
        //List<SplineSlice<Spline>> slices = new List<SplineSlice<Spline>>();
        //
        //totalLength = 0f;
        //foreach (SliceData sliceData in enabledSlices)
        //{
        //    Spline spline = container.Splines[sliceData.splineIndex];
        //    SplineSlice<Spline> slice = new SplineSlice<Spline>(spline, sliceData.range, localToWorldMatrix);
        //    slices.Add(slice);
        //
        //    // Calculate the slice details
        //    sliceData.distanceFromStart = totalLength;
        //    sliceData.sliceLength = slice.GetLength();
        //    totalLength += sliceData.sliceLength;
        //}
        //
        //path = new SplinePath(slices);


        // Create a SplinePath from slices
        path = new SplinePath(new[]
        {
            new SplineSlice<Spline>(container.Splines[0], new SplineRange(0, 8), localToWorldMatrix),
            new SplineSlice<Spline>(container.Splines[1], new SplineRange(0, 8), localToWorldMatrix),
            new SplineSlice<Spline>(container.Splines[0], new SplineRange(10, 11), localToWorldMatrix),
        });



        StartCoroutine(FollowCoroutine());

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator FollowCoroutine()
    {
        for (int i = 0; ; ++i)
        {
            progressRatio = 0f;

            while (progress < 1.0f)
            {
                //get the position on the path
                Vector3 pos = path.EvaluatePosition(progressRatio);
                Vector3 direction = path.EvaluateTangent(progressRatio);

                transform.position = pos;
                transform.LookAt(pos + direction);


                // Increment the progress ratio
                progressRatio += speed * Time.deltaTime;

                //Calculate the current distance travelled
                progress = progressRatio * totalLength;
                yield return null;
            }
        }
    }
}
