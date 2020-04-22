using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ？把方块的图形和方块的信息分开成两个类 ...

// ？TODO : 是否应该把方块的图形里的位置 也放到方块的信息里 ...
// ？这样一来 方块的图形就只需要一些固定的图形信息 ...

public class TetrisShapeInfo {
	
	public int speedCount = 20;
	public int currCount = 0;

	public TetrisShape shape;
	
	public TetrisShapeInfo(TetrisShape shape, int speedCount = 20) {
		this.shape = shape;
		this.speedCount = speedCount;
		currCount = this.speedCount;
	}

	public bool needMoveDown {
		get {
			return currCount <= 0;
		}
	}

	public bool UpdateCount() {
		currCount++;
		currCount %= speedCount;
		return needMoveDown;
	}
}
