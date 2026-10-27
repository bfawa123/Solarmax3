using System;
using UnityEngine;

[Serializable]
public class SDKProductInfoList
{
	public SDKProductInfo FindProduct(string productId)
	{
		if (this.Products == null)
		{
			Debug.LogError("FindProduct - Null Products!");
			return null;
		}
		for (int i = 0; i < this.Products.Length; i++)
		{
			SDKProductInfo sdkproductInfo = this.Products[i];
			if (sdkproductInfo.productId == productId)
			{
				return sdkproductInfo;
			}
		}
		Debug.LogErrorFormat("FindProduct - Can not find product info of id {0}", new object[]
		{
			productId
		});
		return null;
	}

	public SDKProductInfo[] Products;
}
