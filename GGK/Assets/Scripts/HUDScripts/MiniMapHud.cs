using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapHud : MonoBehaviour
{
    //use these if you want to use points
    [Header("Map Bounds (Points)")]
    // top left, top right, bottom left, bottom right of game world
    [SerializeField] private Vector3 posTLeft;
    [SerializeField] private Vector3 posTRight, posBLeft, posBRight;

    //Should you use the four points(above) as opposed to a center then box size(below)?
    [SerializeField] private bool usePoints;

    //Use these if you want to use a box
    [Header("Map Bounds(Box)")]
    // width, height, depth of the game space that the UI should cover
    [SerializeField] private Vector3 center;
    //x, z, y indexes
    [SerializeField] private int width, height, depth;

    [Header("Objects")]
    // a list of objects to track on the map
    [SerializeField] private List<GameObject> objects;
    //..along with their respective icons on the map
    [SerializeField] private List<Image> mapIcons;



    [Header("Depth")]
    // should icons on the map
    // change size to indicate depth?
    [SerializeField] private bool ignoreDepth;
    //Size multiplier of the average icon. 1x size is at 5% of the Minimap's size.
    [SerializeField] private float averageSize;
    //maximum, minimum y values the hud will change size to reflect
    [SerializeField] private float maxDepthTracking;
    [SerializeField] private float minDepthTracking;
    //how much larger/smaller should icons get at its maximum? 
    [SerializeField] private float sizeOffset;


    [Header("Misc.")]
    //should debug things (line renderers, for example) show?
    [SerializeField] private bool showDebug;

    //canvas
    private Canvas canvas;
    //the minimap that bounds should follow
    private Image miniMap;
    //reference icon
    private GameObject iconRef;
    //Line renderers for calculated bounds and/or points
    private LineRenderer boundsMaker;
    private LineRenderer pointsMaker;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();

        if (showDebug)
        {
            boundsMaker = GameObject.Find("MapBounds").GetComponent<LineRenderer>();
            pointsMaker = GameObject.Find("Points").GetComponent<LineRenderer>();
        }

        if (usePoints)
        {
            //establish the bounds using the 4 points
            EstablishBounds(posTLeft, posTRight, posBLeft, posBRight);
        }

        //find the minimap and icon reference
        miniMap = GameObject.Find(gameObject.name + "/MiniMap").GetComponent<Image>();
        iconRef = GameObject.Find(gameObject.name + "/MiniMap/MapIcon");

        //If there are objects to track..
        if (objects.Count > 0)
        {
            //add objects to the icon list
            iconRef.SetActive(true);
            mapIcons.Add(iconRef.GetComponent<Image>());

            //create more icons as more are needed
            for (int i = 0; i < objects.Count - 1; i++)
            {
                GameObject newIcon = Instantiate(iconRef, miniMap.gameObject.transform);
                mapIcons.Add(newIcon.GetComponent<Image>());
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //update icon position
        for (int i = 0; i < objects.Count; i++)
        {
            //calulate position
            Vector3 objPos = objects[i].transform.position;
            Vector2 miniMapSize = miniMap.rectTransform.sizeDelta;
            float xPos = (objPos.x / width) * miniMapSize.x;
            float yPos = (objPos.z / height) * miniMapSize.y;

            //set new icon position
            mapIcons[i].transform.localPosition = new Vector3(xPos, yPos, 0);

            if (!ignoreDepth)
            {
                //calculate icon size
                //default size-- 5% of minimap size times average size multiplier
                float defSize = (Mathf.Sqrt(miniMapSize.x * miniMapSize.y) / 20) * averageSize;

                //map icon size
                Vector2 size = mapIcons[i].rectTransform.sizeDelta;

                //position of the depth as a value between 0 and 1
                float lerp = Mathf.InverseLerp(minDepthTracking, maxDepthTracking, objPos.y);

                //use lerp value to lerp between min size and max size
                float iconSize = Mathf.Lerp((defSize / sizeOffset), (defSize * sizeOffset), Mathf.Clamp(lerp, 0, 1));

                //set new icon size
                mapIcons[i].rectTransform.sizeDelta = new Vector2(iconSize, iconSize);
            }
        }
    }

    //---------------------------------
    //-------------FUNCTIONS-----------
    //---------------------------------

    /// <summary>
    /// Establishes the play area's bounds using four points.  
    /// The play area's bounds will be set to the smallest rectangle that encompasses
    /// the polygon formed by the points.
    /// </summary>
    /// <param name="tLeft">The point at the top left of the polygon.</param>
    /// <param name="tRight">The point at the top right of the polygon.</param>
    /// <param name="bLeft">The point at the bottom left of the polygon.</param>
    /// <param name="bRight">The point at the bottom right of the polygon.</param>
    private void EstablishBounds(Vector3 tLeft, Vector3 tRight, Vector3 bLeft, Vector3 bRight)
    {
        if (showDebug)
        {
            pointsMaker.positionCount = 5;
            pointsMaker.SetPosition(0, tLeft);
            pointsMaker.SetPosition(1, tRight);
            pointsMaker.SetPosition(2, bRight);
            pointsMaker.SetPosition(3, bLeft);
            pointsMaker.SetPosition(4, tLeft);
        }

        Vector3[] pointList = { tLeft, tRight, bLeft, bRight };

        EstablishBounds(pointList);
    }

    /// <summary>
    /// Establishes the play area's bounds using a list of points.  
    /// The play area's bounds will be set to the smallest rectangle that encompasses
    /// the polygon formed by the points.
    /// </summary>
    /// <param name="pointList">A list of points to use.</param>
    private void EstablishBounds(Vector3[] pointList)
    {
        float minX, minY, minZ;
        minX = minY = minZ = Mathf.Infinity;
        float maxX, maxY, maxZ;
        maxX = maxY = maxZ = -Mathf.Infinity;

        foreach (Vector3 point in pointList)
        {
            //if this point's x value is the largest so far..
            if (point.x > maxX)
            {
                maxX = point.x;
            }
            // or the smallest so far...
            else if (point.x < minX)
            {
                minX = point.x;
            }

            //if this point's y value is the largest so far...
            if (point.y > maxY)
            {
                maxY = point.y;
            }
            //or the smallest so far...
            else if (point.y < minY)
            {
                minY = point.y;
            }

            //if this point's z value is the largest to far...
            if (point.z > maxZ)
            {
                maxZ = point.z;
            }
            //or the smallest so far...
            else if (point.z < minZ)
            {
                minZ = point.z;
            }
        }

        //set the center to the midpoint of the points
        Vector3 c = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);

        //calculate width. height, and depth
        int w = (int)(maxX - minX);
        int h = (int)(maxZ - minZ);
        int d = (int)(maxY - minY);

        EstablishBounds(c, w, h, d);
    }

    /// <summary>
    /// Establishes the play area's bounds using box dimensions and a center.
    /// </summary>
    /// <param name="center"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="depth"></param>
    private void EstablishBounds(Vector3 center, int width, int height, int depth)
    {
        this.center = center;
        this.width = width;
        this.height = height;
        this.depth = depth;

        if (showDebug)
        {
            boundsMaker.positionCount = 5;
            boundsMaker.SetPosition(0, new Vector3(center.x - width / 2, 2, center.y + height / 2));
            boundsMaker.SetPosition(1, new Vector3(center.x + width / 2, 2, center.y + height / 2));
            boundsMaker.SetPosition(2, new Vector3(center.x + width / 2, 2, center.y - height / 2));
            boundsMaker.SetPosition(3, new Vector3(center.x - width / 2, 2, center.y - height / 2));
            boundsMaker.SetPosition(4, new Vector3(center.x - width / 2, 2, center.y + height / 2));
        }

    }
}

