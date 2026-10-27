using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ui_event : MonoBehaviour
{
	public Button loader;

	public Button editbuilding;

	public Button editscenes;

	public Button runmap;

	public Text text;

	public ScrollRect view;

	public RectTransform rectTransform;

	public GameObject r_node;

	public GameObject Item;

	public Dropdown dwOption;

	public InputField ifOption;

	public GameObject top;

	public GameObject table;

	public GameObject map;

	public Dropdown dwMap;

	public InputField mapId;

	public InputField player_num;

	public Toggle isv;

	private List<string> option = new List<string>();

	private List<BuildNode> nodelist = new List<BuildNode>();
}
