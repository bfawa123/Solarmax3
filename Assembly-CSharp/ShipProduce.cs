using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ShipProduce
{
	public float startTime { get; set; }

	public float produceRate { get; set; }
}
