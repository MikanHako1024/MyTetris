using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO : 更好的命名 

public class TetrisTilemap : MonoBehaviour {

	public UnityEngine.Tilemaps.Tilemap tilemap;

	private void Start() {
		tilemap = GetComponent<UnityEngine.Tilemaps.Tilemap>();
		InitTilemapData();
		RefreshTilemapShow();
	}
	
	private void FixedUpdate () {
		UpdateAllShape();
	}


	[System.NonSerialized]
	public float lastCreateTime = -60f;

	private void Update() {
		if (Input.GetKey(KeyCode.Z) && Time.time - lastCreateTime > 1f) {
			TetrisShape shape = new TetrisShape();
			shape.SetPosition(GetTopCenterPosition());
			AddShape(shape);

			lastCreateTime = Time.time;

			selectedShape = shape;
		}
	}



	#region 地图数据

	public int tilemapWidth = 8;
	public int tilemapHeight = 14;
	public int tilemapOffsetX = 0;
	public int tilemapOffsetY = 0;

	public int tilemapHeightEx {
		get {
			return tilemapHeight + 4;
		}
	}
	
	public TileBase[] tileList;

	//public enum tilemapType {
	//	empty, ...
	//}

	public int[][] tilemapData;
	public void InitTilemapData() {
		//tilemapData = new int[tilemapHeight][]();
		tilemapData = new int[tilemapHeight][];
		for (int y = 0; y < tilemapHeight; y++) {
			tilemapData[y] = new int[tilemapWidth];
			for (int x = 0; x < tilemapWidth; x++) {
				tilemapData[y][x] = 0;
			}
		}
	}

	public int GetTilemapData(int x, int y) {
		if (!(0 <= x && x < tilemapWidth)) return -1;
		if (tilemapHeight <= y && y < tilemapHeightEx) return 0;
		if (!(0 <= y && y < tilemapHeight)) return -1;
		return tilemapData[y][x];
	}
	public int GetTilemapData(Vector2Int pos) {
		return GetTilemapData(pos.x, pos.y);
	}

	public void SetTilemapData(int x, int y, int data) {
		if (!(0 <= x && x < tilemapWidth)) return ;
		if (!(0 <= y && y < tilemapHeight)) return ;
		tilemapData[y][x] = data;
		// ？修改 tilemapData 的同时直接写进 tilemap ...
		// ？而不必在固定时间 全部写进 tilemap ...
		//tilemap.SetTile(new Vector3Int(x, y, 0), tileList[tilemapData[y][x]]);
		ShowTile(x, y, tilemapData[y][x]);
		// ？因为要显示的不仅有已经停止的方块 ...
		// ？还有活动的方块也要显示，但是不在 tilemapData 中 ...
		// ？所以暂时还是使用全部刷新 ...
		// ？但是全部刷新效率低 ...
	}
	public void SetTilemapData(Vector2Int pos, int data) {
		SetTilemapData(pos.x, pos.y, data);
	}

	public Vector2Int GetTopCenterPosition() {
		return new Vector2Int(tilemapWidth / 2, tilemapHeight);
	}

	#endregion


	#region 执行方块移动和旋转
	
	public enum ShapeMoveType {
		down, left, right, spin, spinInv
	}

	public void InvokeShapeMove(TetrisShape shape, ShapeMoveType moveType) {
		switch (moveType) {
			case ShapeMoveType.down:
				shape.MoveDown(); break;
			case ShapeMoveType.left:
				shape.MoveLeft(); break;
			case ShapeMoveType.right:
				shape.MoveRight(); break;
			case ShapeMoveType.spin:
				shape.MoveSpin(); break;
			case ShapeMoveType.spinInv:
				shape.MoveSpinInv(); break;
		}
	}

	public bool InvokeShapeMoveIfCan(TetrisShape shape, ShapeMoveType moveType) {
		if (CheckShapeMoveVaild(shape, moveType)) {
			InvokeShapeMove(shape, moveType);
			return true;
		}
		else {
			return false;
		}
	}

	#endregion


	#region 判断方块合法，及移动和旋转后合法

	public bool CheckShapeVaild(TetrisShape shape) {
		Vector2Int[] list = shape.GetOccupyPositionList();
		foreach (Vector2Int pos in list) {
			//if (GetTilemapData(pos) > 0) {
			if (GetTilemapData(pos) != 0) {
				return false;
			}
		}
		return true;
	}
	
	//public TetrisShape tempShape = new TetrisShape();
	// ？...
	public TetrisShape tempShape = null;

	//public bool CheckShapeMoveVaild(TetrisShape shape, TetrisShape.MoveDele moveDele) {
	//public bool CheckShapeMoveVaild(TetrisShape shape, string methodName) {
	// ？不能调用 tempShape 的 Invoke ...
	public bool CheckShapeMoveVaild(TetrisShape shape, ShapeMoveType moveType) {
		//shape.CopyTo(tempShape);
		if (tempShape != null) {
			shape.CopyTo(tempShape);
		}
		else {
			tempShape = shape.CopyTo();
		}

		InvokeShapeMove(tempShape, moveType);
		return CheckShapeVaild(tempShape);
	}
	
	public bool CheckShapeStop(TetrisShape shape) {
		return !CheckShapeMoveVaild(shape, ShapeMoveType.down);
	}

	#endregion

	
	#region 方块数组
	
	//public TetrisShape[] shapeList;
	//public List<TetrisShape> shapeList = new List<TetrisShape>();
	public Dictionary<TetrisShape, TetrisShapeInfo> shapeDict = new Dictionary<TetrisShape, TetrisShapeInfo>();
	
	public void AddShape(TetrisShape shape) {
		TetrisShapeInfo info = new TetrisShapeInfo(shape);
		shapeDict.Add(shape, info);
	}

	public TetrisShapeInfo RemoveShape(TetrisShape shape) {
		if (shapeDict.ContainsKey(shape)) {
			TetrisShapeInfo info = shapeDict[shape];
			shapeDict.Remove(shape);
			return info;
		}
		else {
			return null;
		}
	}

	#endregion


	#region 更新方块下降

	// ？地图控制方块自然下落，而不是在方块自己的逻辑里 ...

	public void UpdateAllShapeDown() {
		// 暂时同步下落
		// FINISH : 每个方块都有独立的下落更新帧 和 下落速度 等 ...

		//List<TetrisShape> removeList = new List<TetrisShape>();
		//foreach (TetrisShape shape in shapeList) {
		//	if (CheckShapeMoveVaild(shape, ShapeMoveType.down)) {
		//		InvokeShapeMove(shape, ShapeMoveType.down);
		//	}
		///	else {
		//		removeList.Add(shape);
		//	}
		//}
		// ？改 用List储存 遍历时先列出要移除的对象 之后再移除 ...
		// ？为 用Dict储存 遍历时判断要移除就直接移除 ...
		foreach (TetrisShape shape in shapeDict.Keys) {
			if (shapeDict[shape].UpdateCount()) {
				//if (CheckShapeMoveVaild(shape, ShapeMoveType.down)) {
				if (!CheckShapeStop(shape)) {
					InvokeShapeMove(shape, ShapeMoveType.down);
					if (CheckShapeStop(shape)) {
						//StopShape(shape);
						MarkStopShape(shape);
					}
				}
				else {
					//StopShape(shape);
					MarkStopShape(shape);
				}
			}
		}
		// ？在遍历字典时移除元素也会报错 ...
		// ？改成 在StopShape里标记 之后统一移除 ...
		UpdateMarkStopShape();

		// ？TODO : 把 shapeDict 改回 List ...
	}

	#endregion


	#region 标记停止方块

	public List<TetrisShape> stopShapeList = new List<TetrisShape>();

	public void MarkStopShape(TetrisShape shape) {
		stopShapeList.Add(shape);

		// ？暂时 标记时 也不转换图形 ...
		// ？TODO : 思考要不要在标记时就转换图形 ...
	}

	public void UpdateMarkStopShape() {
		foreach (TetrisShape shape in stopShapeList) {
			StopShape(shape);
		}
		stopShapeList.Clear();
	}

	#endregion


	#region 停止方块

	public bool StopShape(TetrisShape shape) {
		TetrisShapeInfo info = RemoveShape(shape);
		if (info != null) {
			TransShape(shape);
			return true;
		}
		else {
			return false;
		}
	}

	public void TransShape(TetrisShape shape) {
		Vector2Int[] list = shape.GetOccupyPositionList();
		foreach (Vector2Int pos in list) {
			SetTilemapData(pos, shape.GetSquareByGlobal(pos));
		}
	}

	#endregion


	#region 方块移动和旋转控制
	
	public TetrisShape selectedShape = null;

	public float keyInterval = 0.1f;
	[System.NonSerialized]
	public float lastKeyTime = -60f;
	// ？...

	public bool allowKeyDown {
		get {
			return Time.time - lastKeyTime >= keyInterval;
		}
	}

	public void RefreshKeyTime() {
		lastKeyTime = Time.time;
	}

	public void UpdateShapeMove() {
		if (selectedShape != null) {
			if (allowKeyDown) {
				if (Input.GetKey(KeyCode.LeftArrow)) {
					InvokeShapeMoveIfCan(selectedShape, ShapeMoveType.left);
					RefreshKeyTime();
				}
				else if (Input.GetKey(KeyCode.RightArrow)) {
					InvokeShapeMoveIfCan(selectedShape, ShapeMoveType.right);
					RefreshKeyTime();
				}
				else if (Input.GetKey(KeyCode.UpArrow)) {
					InvokeShapeMoveIfCan(selectedShape, ShapeMoveType.spin);
					RefreshKeyTime();
				}
			}
		}
	}

	#endregion


	#region 更新显示

	public void ShowTile(int x, int y, int data) {
		tilemap.SetTile(new Vector3Int(x, y, 0), tileList[data]);
	}
	public void ShowTile(Vector2Int pos, int data) {
		ShowTile(pos.x, pos.y, data);
	}

	public void ClearShapeShow() {
		Vector2Int[] list;
		int data;
		foreach (TetrisShape shape in shapeDict.Keys) {
			list = shape.GetOccupyPositionList();
			foreach (Vector2Int pos in list) {
				//ShowTile(pos, 0);
				data = GetTilemapData(pos);
				data = data == -1 ? 0 : data;
				ShowTile(pos, data);
			}
		}
	}

	public void ShowAllShape() {
		Vector2Int[] list;
		foreach (TetrisShape shape in shapeDict.Keys) {
			list = shape.GetOccupyPositionList();
			foreach (Vector2Int pos in list) {
				ShowTile(pos, shape.GetSquareByGlobal(pos));
			}
		}
	}

	public void UpdateAllShape() {
		ClearShapeShow();
		UpdateAllShapeDown();
		UpdateMarkStopShape();
		UpdateShapeMove();
		ShowAllShape();
	}

	public void RefreshTilemapShow() {
		for (int y = 0; y < tilemapHeight; y++) {
			for (int x = 0; x < tilemapWidth; x++) {
				ShowTile(x, y, GetTilemapData(x, y));
			}
		}
		for (int y = tilemapHeight; y < tilemapHeightEx; y++) {
			for (int x = 0; x < tilemapWidth; x++) {
				ShowTile(x, y, 0);
			}
		}
		ShowAllShape();
	}

	#endregion
}

// ？分为活动的方块和停止的方块 ...
// ？活动的方块是一个整体，储存一个整体 ...
// ？停止的方块可能被部分消除，所以直接转化成每一个小方格储存 ...
