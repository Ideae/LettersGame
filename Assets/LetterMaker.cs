using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LetterMaker : MonoBehaviour {

  Vector3[] tracePosList;
  int currentPos;
  float offset;
  LineRenderer line;
  LineRenderer trace;
  public Material mat;
  public GameObject marble;
  public bool isAutoCompleting;
  private float progress;
  public event Action OnComplete;
  public bool PointsOnly;

  // Use this for initialization
	void Start () {
	  var meshMarkers = transform.FindChild("MeshMarkers");
	  Transform firstChild = null;
	  var meshFilter = gameObject.AddComponent<MeshFilter>();
    Mesh m = new Mesh();
	  var vertices = new List<Vector3>();
	  var normals = new List<Vector3>();
    foreach (Transform child in meshMarkers) {
	    if (!firstChild) { firstChild = child;}
      vertices.Add(child.position);
      normals.Add(new Vector3(0,0,-1));
	  }
    m.SetVertices(vertices);

	  var indices = new List<int>();

	  for (int i = 0; i < vertices.Count -2; i++) {
      indices.Add(i);
      indices.Add(i+1);
      indices.Add(i+2);
	  }

    m.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);
    m.SetNormals(normals);

	  meshFilter.mesh = m;

    var pathmarkers = transform.FindChild("PathMarkers");
    var tp = from Transform path in pathmarkers select path.position;
    var tr = from Transform path in pathmarkers select path.GetComponent<Renderer>();
	  foreach (var renderer1 in tr) {
	    renderer1.sortingLayerName = "Middle";
      renderer1.material.color = Color.blue;

    }
    //tracePosList = Curver.MakeSmoothCurve(tp.ToArray(), 6);
    tracePosList = tp.ToArray();

    GetComponent<MeshRenderer>().sortingLayerName = "Background";

    line = new GameObject().AddComponent<LineRenderer>();
    line.transform.parent = transform;
    line.SetWidth(0.5f,0.5f);
	  line.material = mat;
    line.SetColors(Color.black, Color.black);

    trace = new GameObject().AddComponent<LineRenderer>();
    trace.transform.parent = transform;
    trace.SetWidth(0.5f, 0.5f);
    trace.material = mat;
    trace.SetColors(Color.red, Color.red);


	  marble.transform.position = tracePosList[0];
	  marble.GetComponent<Renderer>().sortingLayerName = "Marble";
	  marble.GetComponent<MeshRenderer>().material.color = Color.yellow;


	}

  private float prevDot = 0;
  // Update is called once per frame
  void Update () {
    var mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0,0,10));
    var dist = Vector3.Distance(mousepos , marble.transform.position);
    var dir = tracePosList[currentPos + 1] - tracePosList[currentPos];
    var relPt = mousepos - tracePosList[currentPos];
    if (isAutoCompleting || dist < 1f && Input.GetMouseButton(0)) {
      var dot = Vector2.Dot(relPt, dir.normalized);
      if (dot < prevDot) dot = prevDot;

      var projPt = dot*dir.normalized;
      if (isAutoCompleting)  projPt = (progress += .1f )*dir.normalized;

      if((isAutoCompleting || !(dot < 0) ) && !(projPt.sqrMagnitude > dir.sqrMagnitude)) {
        marble.transform.position = Vector3.Lerp(marble.transform.position, projPt + tracePosList[currentPos],0.7f);
      }
        if (Vector3.Distance(marble.transform.position, tracePosList[currentPos + 1]) <0.5) {
          if (currentPos < tracePosList.Length - 2) {
            marble.transform.position = tracePosList[currentPos+1];
          currentPos++;
            progress = 0;
          prevDot = 0;
        }
          else {
            isAutoCompleting = false;
            if (OnComplete != null) {
              var a = OnComplete;
               OnComplete = null;
              a.Invoke();
            }
          }
        }
    }

    else {
      Reset();
    }

    var t = new List<Vector3>();
    var l = new List<Vector3>();
    
    l.Add(marble.transform.position);
    for (int index = 0; index < tracePosList.Length; index++)
    {
      var p = tracePosList[index];
      if (index <= currentPos) {
        if(t.Count > 0) t.Add(Vector3.MoveTowards(p,t[t.Count-1],0.2f));
        t.Add(p);
        if (index < tracePosList.Length - 1) t.Add(Vector3.MoveTowards(p, tracePosList[index + 1], 0.2f));
      }else{
        if (l.Count > 0) l.Add(Vector3.MoveTowards(p, l[l.Count - 1], 0.2f));
        l.Add(p);
        if (index < tracePosList.Length - 1) l.Add(Vector3.MoveTowards(p, tracePosList[index + 1], 0.2f));

      }
    }
    if (t.Count > 0) t.Add(Vector3.MoveTowards(marble.transform.position, t[t.Count - 1], 0.2f));
    t.Add(marble.transform.position);
    if (t.Count > 0) t.Add(Vector3.MoveTowards(marble.transform.position, t[t.Count - 1], -0.2f));
    
    line.SetVertexCount(l.Count);
    trace.SetVertexCount(t.Count);
    for (int i = 0; i < l.Count; i++) line.SetPosition(i,l[i]);
    for (int i = 0; i < t.Count; i++) trace.SetPosition(i,t[i]);
    line.enabled = !PointsOnly;
  }
  public void Reset()
  {

    currentPos = 0;
    progress = 0;
    marble.transform.position = tracePosList[0];
  }
  public void AutoComplete() {
    this.isAutoCompleting = true;
  }
}
