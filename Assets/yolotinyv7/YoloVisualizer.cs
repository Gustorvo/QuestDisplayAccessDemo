using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YoloVisualizer
{
	private RunYOLO _runYolo;
	private List<GameObject> boxPool = new List<GameObject>();


	public YoloVisualizer(RunYOLO runYolo)
	{
		_runYolo = runYolo;
	}

	public void DrawBox(RunYOLO.BoundingBox box, int id)
	{
		//Create the bounding box graphic or get from pool
		GameObject panel;
		if (id < boxPool.Count)
		{
			panel = boxPool[id];
			panel.SetActive(true);
		}
		else
		{
			panel = CreateNewBox(Color.yellow);
		}

		//Set box position
		panel.transform.localPosition = new Vector3(box.centerX, -box.centerY);

		//Set box size
		RectTransform rt = panel.GetComponent<RectTransform>();
		rt.sizeDelta = new Vector2(box.width, box.height);

		//Set label text
		var label = panel.GetComponentInChildren<Text>();
		label.text = box.label + " (" + box.confidence + "%)";
	}

	public GameObject CreateNewBox(Color color)
	{
		//Create the box and set image

		var panel = new GameObject("ObjectBox");
		panel.AddComponent<CanvasRenderer>();
		Image img = panel.AddComponent<Image>();
		img.color = color;
		img.fillCenter = false;
		img.pixelsPerUnitMultiplier = 92;
		img.sprite = _runYolo.boxTexture;
		img.type = Image.Type.Sliced;
		panel.transform.SetParent(_runYolo.displayLocation, false);

		//Create the label

		var text = new GameObject("ObjectLabel");
		text.AddComponent<CanvasRenderer>();
		text.transform.SetParent(panel.transform, false);
		Text txt = text.AddComponent<Text>();
		txt.font = _runYolo.font;
		txt.color = color;
		txt.fontSize = 40;
		txt.horizontalOverflow = HorizontalWrapMode.Overflow;

		RectTransform rt2 = text.GetComponent<RectTransform>();
		rt2.offsetMin = new Vector2(20, rt2.offsetMin.y);
		rt2.offsetMax = new Vector2(0, rt2.offsetMax.y);
		rt2.offsetMin = new Vector2(rt2.offsetMin.x, 0);
		rt2.offsetMax = new Vector2(rt2.offsetMax.x, 30);
		rt2.anchorMin = new Vector2(0, 0);
		rt2.anchorMax = new Vector2(1, 1);

		boxPool.Add(panel);
		return panel;
	}

	public void ClearAnnotations()
	{
		foreach (var box in boxPool)
		{
			box.SetActive(false);
		}
	}
}