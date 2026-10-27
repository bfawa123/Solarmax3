using System;

public interface IPoolNode
{
	void Recovery();

	void Release();

	void SetPool(ObjectPool pool);

	bool IsActive();

	void Update(int frame, float dt);
}
