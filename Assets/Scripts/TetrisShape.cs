using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisShape {

	public Vector2Int position = default(Vector2Int);
	public string name = "";
	public int type = 0;
	public int spin = 0;
	
	public TetrisShape() {
		SetPosition(0, 0);
		SetupRandomShape();
		SetRandomSpin();
		SetRandomColor();
	}

	#region 形状数据

	/*
	 *    0 0 0 0
	 * ^  0 0 0 0
	 * |  0 1 0 0
	 * y  0 0 0 0
	 *   x ->
	 */
	/*
	static int[][][] shapeData = {
		// I
		new int[][] {
			new int[] {
				0, 1, 0, 0,
				0, 1, 0, 0,
				0, 1, 0, 0,
				0, 1, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				1, 1, 1, 1,
				0, 0, 0, 0,
			},
		},
		// O
		new int[][] {
			new int[] {
				0, 0, 0, 0,
				0, 1, 1, 0,
				0, 1, 1, 0,
				0, 0, 0, 0,
			},
		},
		// L
		new int[][] {
			new int[] {
				0, 0, 0, 0,
				0, 1, 0, 0,
				0, 1, 0, 0,
				0, 1, 1, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				1, 1, 1, 0,
				1, 0, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				1, 1, 0, 0,
				0, 1, 0, 0,
				0, 1, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 1, 0,
				1, 1, 1, 0,
				0, 0, 0, 0,
			},
		},
		// J
		new int[][] {
			new int[] {
				0, 0, 0, 0,
				0, 1, 0, 0,
				0, 1, 0, 0,
				1, 1, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				1, 0, 0, 0,
				1, 1, 1, 0,
				0, 0, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 1, 1, 0,
				0, 1, 0, 0,
				0, 1, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				1, 1, 1, 0,
				0, 0, 1, 0,
			},
		},
		// Z
		new int[][] {
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				1, 1, 0, 0,
				0, 1, 1, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 1, 0, 0,
				1, 1, 0, 0,
				1, 0, 0, 0,
			},
		},
		// N
		new int[][] {
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				0, 1, 1, 0,
				1, 1, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				1, 0, 0, 0,
				1, 1, 0, 0,
				0, 1, 0, 0,
			},
		},
	};
	*/
	static int[][][] shapeData = {
		// I
		new int[][] {
			new int[] {
				0, 1, 0, 0,
				0, 2, 0, 0,
				0, 3, 0, 0,
				0, 4, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				4, 3, 2, 1,
				0, 0, 0, 0,
			},
			new int[] {
				0, 4, 0, 0,
				0, 3, 0, 0,
				0, 2, 0, 0,
				0, 1, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				1, 2, 3, 4,
				0, 0, 0, 0,
			},
		},
		// O
		new int[][] {
			new int[] {
				0, 0, 0, 0,
				0, 1, 2, 0,
				0, 3, 4, 0,
				0, 0, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 4, 1, 0,
				0, 3, 2, 0,
				0, 0, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 3, 4, 0,
				0, 2, 1, 0,
				0, 0, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 2, 3, 0,
				0, 1, 4, 0,
				0, 0, 0, 0,
			},
		},
		// L
		new int[][] {
			new int[] {
				0, 0, 0, 0,
				0, 1, 0, 0,
				0, 2, 0, 0,
				0, 3, 4, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				3, 2, 1, 0,
				4, 0, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				4, 3, 0, 0,
				0, 2, 0, 0,
				0, 1, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 4, 0,
				1, 2, 3, 0,
				0, 0, 0, 0,
			},
		},
		// J
		new int[][] {
			new int[] {
				0, 0, 0, 0,
				0, 1, 0, 0,
				0, 2, 0, 0,
				3, 4, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				3, 0, 0, 0,
				4, 2, 1, 0,
				0, 0, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 4, 3, 0,
				0, 2, 0, 0,
				0, 1, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				1, 2, 4, 0,
				0, 0, 3, 0,
			},
		},
		// Z
		new int[][] {
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				1, 2, 0, 0,
				0, 3, 4, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 1, 0, 0,
				3, 2, 0, 0,
				4, 0, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				4, 3, 0, 0,
				0, 2, 1, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 4, 0, 0,
				2, 3, 0, 0,
				1, 0, 0, 0,
			},
		},
		// N
		new int[][] {
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				0, 1, 2, 0,
				3, 4, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				3, 0, 0, 0,
				4, 1, 0, 0,
				0, 2, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				0, 4, 3, 0,
				2, 1, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				2, 0, 0, 0,
				1, 4, 0, 0,
				0, 3, 0, 0,
			},
		},
		// T
		new int[][] {
			new int[] {
				0, 0, 0, 0,
				0, 1, 0, 0,
				2, 3, 4, 0,
				0, 0, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 2, 0, 0,
				0, 3, 1, 0,
				0, 4, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 0, 0, 0,
				4, 3, 2, 0,
				0, 1, 0, 0,
			},
			new int[] {
				0, 0, 0, 0,
				0, 4, 0, 0,
				1, 3, 0, 0,
				0, 2, 0, 0,
			},
		},
	};

	static string[] shapeNameList = { "I", "O", "L", "J", "Z", "N", "T" };
	static Dictionary<string, int> shapeDataIndex = new Dictionary<string, int>();

	static TetrisShape() {
		for (int i = 0; i < shapeNameList.Length; i++) {
			shapeDataIndex.Add(shapeNameList[i], i);
		}
	}

	#endregion
	// TODO : 区分每个格子的序号，因为要考虑颜色 ...

	
	#region 装载形状

	public void SetupShape(int index) {
		if (0 <= index && index < shapeNameList.Length) {
			type = index;
			name = shapeNameList[index];
		}
		else {

		}
	}
	public void SetupShape(string name) {
		if (shapeDataIndex.ContainsKey(name)) {
			SetupShape(shapeDataIndex[name]);
		}
		else {
			SetupShape(-1);
		}
	}
	public void SetupShape() {
		SetupShape(-1);
	}
	
	public void SetupRandomShape() {
		int index = Random.Range(0, shapeNameList.Length);
		SetupShape(index);
	}

	#endregion
	

	#region 设置位置

	public void SetPosition(Vector2Int pos) {
		position = pos;
	}
	public void SetPosition(int x, int y) {
		SetPosition(new Vector2Int(x, y));
	}

	#endregion


	#region 形状数组转换
	
	public int[] currShape {
		get {
			return shapeData[type][spin];
		}
	}
	
	public int GetShapeIndex(int offsetX, int offsetY) {
		return 4 * (2 - offsetY) + (offsetX + 1);
	}
	public int GetShapeIndex(Vector2Int offset = default(Vector2Int)) {
		return GetShapeIndex(offset.x, offset.y);
	}
	public Vector2Int GetShapeOffset(int index) {
		return new Vector2Int(index % 4 - 1, 2 - index / 4);
	}

	#endregion


	#region 获取位置
	
	public int GetSquare(int offsetX = 0, int offsetY = 0) {
		//if (0 <= offsetX && offsetX < 4 && 0 <= offsetY && offsetY < 4) {
		if (-1 <= offsetX && offsetX <= 2 && -1 <= offsetY && offsetY <= 2) {
			int index = GetShapeIndex(offsetX, offsetY);
			if (0 <= index && index < 16) {
				//return currShape[index
				return currShape[index] != 0
					? color[currShape[index] - 1]
					: 0;
			}
			else {
				return 0;
			}
		}
		else {
			return 0;
		}
	}
	public int GetSquare(Vector2Int offset) {
		return GetSquare(offset.x, offset.y);
	}
	
	public int GetSquareByGlobal(Vector2Int pos) {
		return GetSquare(pos - position);
	}
	public int GetSquareByGlobal(int x, int y) {
		return GetSquareByGlobal(new Vector2Int(x, y));
	}

	#endregion


	#region 枚举占据位置

	//public IEnumerable<Vector2Int> GetOccupy() {
	// ？...

	public Vector2Int[] GetOccupyOffsetList() {
		Vector2Int[] offsetList = new Vector2Int[4];
		int top = 0;
		for (int i = 0; i < currShape.Length; i++) {
			if (currShape[i] > 0) {
				offsetList[top++] = GetShapeOffset(i);
				if (top >= 4) break;
			}
		}
		return offsetList;
	}

	public Vector2Int[] GetOccupyPositionList() {
		Vector2Int[] positionList = GetOccupyOffsetList();
		for (int i = 0; i < positionList.Length; i++) {
			positionList[i] += position;
		}
		return positionList;
	}

	#endregion


	#region 设置旋转

	public int spinCount {
		get {
			return shapeData[type].Length;
		}
	}

	public void SetSpin(int index) {
		//int spinCount = shapeData[type].Length;
		// 写成 getter
		spin = index % spinCount;
	}
	
	public void SetRandomSpin() {
		//int spinCount = shapeData[type].Length;
		spin = Random.Range(0, spinCount);
	}

	#endregion


	#region 移动和旋转

	//public delegate void MoveDele();

	public void MoveDown() {
		position.y--;
	}
	public void MoveLeft() {
		position.x--;
	}
	public void MoveRight() {
		position.x++;
	}

	public void MoveSpin() {
		//int spinCount = shapeData[type].Length;
		spin = (spin + 1) % spinCount;
	}
	public void MoveSpinInv() {
		//int spinCount = shapeData[type].Length;
		spin = (spin - 1 + spinCount) % spinCount;
		//Debug.Log((3 - 6) % 5); // 负数取模测试
	}

	#endregion


	#region 复制

	// ？因为 判断是否可以移动 需要用到总的地图数组 ...
	// ？所以单独的 TetrisShape 是做不到的 ...
	// ？应该把 判断是否可以移动 的逻辑放到 TetrisTilemap 里 ...

	// ？TetrisShape 的移动 不能知道这次移动是否可行 ...
	// ？所以在 TetrisShape 的移动逻辑里不需要判断是否可行 ...

	// ？这里创建一个副本，并让副本移动或旋转 ...
	// ？再在 TetrisTilemap 里判断这个副本是否可行 ...
	// ？以实现检查 TetrisShape 的移动或旋转 是否可行 ...

	public TetrisShape CopyTo() {
		TetrisShape shape = new TetrisShape();
		CopyTo(shape);
		return shape;
	}

	public void CopyTo(TetrisShape shape) {
		shape.SetPosition(position);
		shape.SetupShape(type);
		shape.SetSpin(spin);
	}

	#endregion


	#region 形状颜色
	
	public int[] color = new int[4] { 1, 2, 3, 4 };

	public void SetRandomColor() {
		List<int> list = new List<int>();
		list.Add(1);
		list.Add(2);
		list.Add(3);
		list.Add(4);
		int top = 0;
		while (list.Count > 0) {
			int random = Random.Range(0, list.Count);
			color[top++] = list[random];
			//list.Remove(random);
			//list.Remove(list[random]);
			list.RemoveAt(random);
		}
	}

	#endregion


	#region 画进tilemap
	
	// TODO : 待完成 待使用
	
	/*
	private static Vector2Int[] tempPosList;
	private static UnityEngine.Tilemaps.TileBase[] tempTiles = { null, null, null, null, null, };

	public void DrawShape(UnityEngine.Tilemaps.Tilemap tilemap, 
			UnityEngine.Tilemaps.TileBase[] tiles) {
		tempPosList = GetOccupyPositionList();
		foreach (Vector2Int pos in tempPosList) {
			tilemap.SetTiles...
		}
	}

	...
	*/

	#endregion

}

