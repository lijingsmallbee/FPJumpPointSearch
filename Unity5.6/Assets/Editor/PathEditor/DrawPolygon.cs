using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DrawPolygon : MonoBehaviour {

    public GameObject EmptyPolygon=null;
    public Camera CameraObject = null;
    public int InitializeHeight = 20;

    private Vector2 TerrainBound;
    private Vector2 TerrainOffset;
    private List<List<Vector3>> PointList;
    private List<GameObject> PolygonList;

    private static Material aMaterial = null;
    private GameObject LocateObject = null;

    private float x, y;
    private int ScaleFactor = 30;

    public static void CreateMaterial()
    {
        if (aMaterial != null)
            return;
        aMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                                    "SubShader { Pass { " +
                                    "    Blend SrcAlpha OneMinusSrcAlpha " +
                                    "    ZWrite Off Cull Off Fog { Mode Off } " +
                                    "    BindChannels {" +
                                    "      Bind \"vertex\", vertex Bind \"color\", color }" +
                                    "} } }");
        aMaterial.hideFlags = HideFlags.HideAndDontSave;
        aMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
    }

    Vector2 ConvertToPlan(Vector3 pos)
    {
        return new Vector2(ScaleFactor * (pos.x - TerrainOffset.x), ScaleFactor * (pos.z - TerrainOffset.y));
    }

 

    void Start()
    {
        if (EmptyPolygon == null || CameraObject==null )
            return;
        Terrain TerrainObject = gameObject.GetComponent<Terrain>();

        TerrainBound = new Vector2(TerrainObject.terrainData.size.x * ScaleFactor, TerrainObject.terrainData.size.z * ScaleFactor);
        TerrainOffset = new Vector2(transform.position.x, -transform.position.z );

        PointList = new List<List<Vector3>>();
        PolygonList = new List<GameObject>();
        CreateMaterial();

        LocateObject = (GameObject)Instantiate(EmptyPolygon);

        x = 0;
        y = 90;
        CameraObject.transform.rotation = Quaternion.Euler(y, x, 0);
        Vector3 tmpPos = new Vector3(0, 0, InitializeHeight);
        CameraObject.transform.position = Quaternion.Euler(y, x, 0) * tmpPos + LocateObject.transform.position;
    }

    float ClampAngle(float angle, float min , float max )
    { 
	    if (angle < -360)  
		    angle += 360; 
	    if (angle > 360)  
		    angle -= 360; 
	    return Mathf.Clamp (angle, min, max);
    }

    void AddNewPolygon()
    {
        PointList.Add(new List<Vector3>());
        GameObject newObject = (GameObject)Instantiate(EmptyPolygon);
        LineRenderer mLine = newObject.AddComponent<LineRenderer>();
        PolygonList.Add(newObject);
        mLine.GetComponent<Renderer>().enabled = true;
        mLine.SetWidth(0.1f, 0.1f);
		mLine.SetVertexCount(0);
        mLine.SetColors(Color.red, Color.yellow);
        mLine.material = aMaterial;
    }

    void Update()
    {
        if (PointList == null || EmptyPolygon == null || CameraObject == null)
            return;
        if (Input.GetMouseButtonDown(1))
            OnDrawPoint();

        if (Input.GetKeyDown(KeyCode.Return) )
            OnEndPoint();

        if (Input.GetKeyDown(KeyCode.Backspace))
            OnDelPoint();

        if (Input.GetKeyDown(KeyCode.Delete))
            OnDelPolygon();

        if (Input.GetKeyDown(KeyCode.F12))
            OnExportToFile();


        float distance = Vector3.Distance(LocateObject.transform.position, CameraObject.transform.position);
        if (Input.GetMouseButton(0))
        {
            if (!Input.GetKey(KeyCode.LeftAlt))
            {
                float xOffset = -Input.GetAxis("Mouse X") * 0.5f*Mathf.Cos(CameraObject.transform.eulerAngles.y*3.124159f/180);
                float zOffset = Input.GetAxis("Mouse X") * 0.5f * Mathf.Sin(CameraObject.transform.eulerAngles.y * 3.124159f / 180);
                xOffset += -Input.GetAxis("Mouse Y") * 0.5f * Mathf.Sin((180-CameraObject.transform.eulerAngles.y) * 3.124159f / 180); 
                zOffset += Input.GetAxis("Mouse Y") * 0.5f * Mathf.Cos((180- CameraObject.transform.eulerAngles.y) * 3.124159f / 180);
                LocateObject.transform.Translate( xOffset, 0, zOffset );
            }
            else
            {
                x += Input.GetAxis("Mouse X") * 200 * 0.02f;
                y -= Input.GetAxis("Mouse Y") * 120 * 0.02f;
                y = ClampAngle(y, 10, 90 );
            }
        }

        distance -= Input.GetAxis("Mouse ScrollWheel") * 5;
        //获取鼠标中建响应
        distance = Mathf.Clamp(distance, -20, 100 );
        //距离取最大值和最小值
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 tempV = new Vector3(0, 0, -distance);
        CameraObject.transform.position = rotation * tempV + LocateObject.transform.position;
        CameraObject.transform.rotation = rotation;
        //Debug.Log(CameraObject.transform.eulerAngles);
        
    }

    void OnMouseControl()
    {
        CameraObject.transform.Translate(-Input.GetAxis("Mouse X") * 0.5f, 0, -Input.GetAxis("Mouse Y") * 0.5f);
    }

    void OnDrawPoint()
    {
        //从摄像机的原点向鼠标点击的对象身上设法一条射线
        Ray ray = CameraObject.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //当射线彭转到对象时
        if (Physics.Raycast(ray, out hit))
        {
            //目前场景中只有地形
            //其实应当在判断一下当前射线碰撞到的对象是否为地形。
            if (hit.collider.gameObject != gameObject )
                return;

            if (PointList.Count == 0)
                AddNewPolygon();
            int CurrentPoly = PointList.Count - 1;
            if( PointList[CurrentPoly].Count>1 && PointList[CurrentPoly][0] == PointList[CurrentPoly][PointList[CurrentPoly].Count-1] )
                AddNewPolygon();
            CurrentPoly = PointList.Count - 1;
            LineRenderer mLine = PolygonList[CurrentPoly].GetComponent<LineRenderer>();

            //得到在3D世界中点击的坐标
            Vector3 Point = new Vector3(hit.point.x, hit.point.y + 0.1f, hit.point.z);
            //Debug.Log(Point);
            PointList[CurrentPoly].Add(Point);
            mLine.SetVertexCount(PointList[CurrentPoly].Count);
            mLine.SetPosition(PointList[CurrentPoly].Count - 1, Point);
        }
    }


    void OnEndPoint()
    {
        if (PointList.Count == 0)
            return;
        int CurrentPoly = PointList.Count - 1;
        LineRenderer mLine = PolygonList[CurrentPoly].GetComponent<LineRenderer>();
        if (PointList[CurrentPoly].Count > 0)
        {
            PointList[CurrentPoly].Add(PointList[CurrentPoly][0]);
            mLine.SetVertexCount(PointList[CurrentPoly].Count);
            mLine.SetPosition(PointList[CurrentPoly].Count - 1, PointList[CurrentPoly][0]);
        }
    }

    void OnDelPoint()
    {
        if (PointList.Count == 0)
            return;

        int CurrentPoly = PointList.Count - 1;
        CurrentPoly = PointList.Count - 1;
        LineRenderer mLine = PolygonList[CurrentPoly].GetComponent<LineRenderer>();
        if (PointList[CurrentPoly].Count > 0)
        {
            PointList[CurrentPoly].RemoveAt(PointList[CurrentPoly].Count - 1);
            mLine.SetVertexCount(PointList[CurrentPoly].Count);
            if (PointList[CurrentPoly].Count <= 1)
                OnDelPolygon();
        }
    }

    void OnDelPolygon()
    {
        if (PointList.Count == 0)
            return;

        int CurrentPoly = PointList.Count - 1;
        PointList.RemoveAt(CurrentPoly);
        Object.Destroy(PolygonList[CurrentPoly]);
        PolygonList.RemoveAt(CurrentPoly);
    }

    void OnExportToFile()
    {
        string BufferString = "Bound:" + TerrainBound.x.ToString() + "," + TerrainBound.y.ToString() + "\r\n";
        string Line = "";

        for (int i = 0; i < PointList.Count; i++)
        {
            if (PointList[i].Count > 1)
            {
                Line = "Polygon:";
                for (int j = 0; j < PointList[i].Count; j++)
                {
                    Vector2 v2 = ConvertToPlan(PointList[i][j]);
                    Line += (int)v2.x + "," + (int)v2.y + ";";
                }
                Line += "\r\n";
                BufferString += Line;
            }
        }

        foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.transform.parent != null)
                continue;
            if (obj.GetComponent<BoxCollider>() != null)
            {
                Line = "Box:";
                Quaternion q = new Quaternion(0f, 0f, 0f, 1f);
                BoxCollider cod = obj.GetComponent<BoxCollider>();
                Line += Quaternion.Angle(q, obj.transform.rotation).ToString() + ";";
                Vector3 pos = new Vector3(obj.transform.position.x + cod.center.x, 0, obj.transform.position.z + cod.center.z );
                Vector2 tpos = ConvertToPlan( pos );
                Line += tpos.x.ToString() + "," + tpos.y.ToString() + ";";
                Line += (ScaleFactor * obj.transform.localScale.x * cod.size.x).ToString() + "," + (ScaleFactor * obj.transform.localScale.z * cod.size.z).ToString() + "\r\n";
                BufferString += Line;
            }
            if (obj.GetComponent<CapsuleCollider>() != null)
            {
                Line = "Cylinder:";
                CapsuleCollider cod = obj.GetComponent<CapsuleCollider>();
                Vector3 pos = new Vector3(obj.transform.position.x + cod.center.x, 0, obj.transform.position.z + cod.center.z);
                Vector2 tpos = ConvertToPlan(pos);
                Line += tpos.x.ToString() + "," + tpos.y.ToString() + ";";
                Line += (cod.radius * ScaleFactor).ToString() + "\r\n";
                BufferString += Line;
            }
            if (obj.GetComponent<SphereCollider>() != null)
            {
                Line = "Cylinder:";
                SphereCollider cod = obj.GetComponent<SphereCollider>();
                Vector3 pos = new Vector3(obj.transform.position.x + cod.center.x, 0, obj.transform.position.z + cod.center.z);
                Vector2 tpos = ConvertToPlan(pos);
                Line += tpos.x.ToString() + "," + tpos.y.ToString() + ";";
                Line += (cod.radius * ScaleFactor).ToString() + "\r\n";
                BufferString += Line;
            }
        }


        using (BinaryWriter fw = new BinaryWriter(File.Open( Application.dataPath + "\\sceneCollider.txt", FileMode.Create)))
        {
            fw.Write(BufferString.ToCharArray());
        }
    }
}
