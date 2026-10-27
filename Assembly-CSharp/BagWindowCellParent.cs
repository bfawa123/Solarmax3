using System;
using NetMessage;
using UnityEngine;

public class BagWindowCellParent : MonoBehaviour
{
	public void SetInfo(int index, PackItem pi)
	{
		if (index >= 0 && index < this.cell.Length)
		{
			this.cell[index].SetInfo(pi);
		}
	}

	public BagWindowCell[] cell;
}
