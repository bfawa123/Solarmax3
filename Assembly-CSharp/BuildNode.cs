using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildNode
{
	public GameObject go;

	public InputField key;

	public Dropdown drop;

	public Dictionary<int, Dictionary<string, InputField>> dic = new Dictionary<int, Dictionary<string, InputField>>();

	public RectTransform subNode;
}
