using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO : 更好的命名 

// TODO : SetTiles

// ？TODO : 图层顺序 瓦片顺序 ...

// TODO : 切换选中

public class TetrisTilemap : MonoBehaviour {

	public UnityEngine.Tilemaps.Tilemap tilemap;

	private void Start() {
		if (tilemap == null) {
			tilemap = GetComponent<UnityEngine.Tilemaps.Tilemap>();
		}
		//InitLayerOrder();
		InitTilemapData();
		RefreshTilemapShow();
	}

	private void FixedUpdate() {
		UpdateAllShape();
	}
	
	public void UpdateAllShape() {
		ClearShapeShow();

		UpdateCreateNewShape();

		UpdateAllShapeDown();
		UpdateMarkStopShape();

		UpdateShapeMove();
		UpdateSelectShape();

		UpdateCheckEliminateLine();
		ShowAllShape();
	}


	/*
	public float CreateInterval = 1f;
	[System.NonSerialized]
	public float lastCreateTime = -60f;

	private void Update() {
		// 暂时这样创建新的下落图形
		if (Input.GetKey(KeyCode.Z) && Time.time - lastCreateTime > CreateInterval) {
			TetrisShape shape = new TetrisShape();
			shape.SetPosition(GetTopCenterPosition());
			AddShape(shape);

			lastCreateTime = Time.time;

			//selectedShape = shape;
			// ？因为更换selectedShape的时候还要变换其他的东西 ...
			// ？比如还要清除选中方块的光标等 ...
			// ？所以要把更换selectedShape写成一个方法或setter 在方法里完成其他操作 ...
			// ？对selectedShape更安全、严格的管理，即是扩展其getter,setter，在其中完成其他操作使得安全 ...
			// TODO : 笔记
			//SetSelectedShape(shape);
			if (shape != null) {
				SetSelectedShape(shape);
			}
		}
	}
	*/


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
		//if (tilemapHeight <= y && y < tilemapHeightEx) return 0;
		if (!(0 <= y && y < tilemapHeight)) return -1;
		return tilemapData[y][x];
	}
	public int GetTilemapData(Vector2Int pos) {
		return GetTilemapData(pos.x, pos.y);
	}

	// 对于高度拓展的部分 需要视为地图内
	public int GetTilemapDataEx(int x, int y) {
		//if (tilemapHeight <= y && y < tilemapHeightEx) return 0;
		if (!(0 <= x && x < tilemapWidth)) return -1;
		if (tilemapHeight <= y && y < tilemapHeightEx) return 0;
		if (!(0 <= y && y < tilemapHeight)) return -1;
		return GetTilemapData(x, y);
	}
	public int GetTilemapDataEx(Vector2Int pos) {
		return GetTilemapDataEx(pos.x, pos.y);
	}

	public void SetTilemapData(int x, int y, int data) {
		if (!(0 <= x && x < tilemapWidth)) return;
		if (!(0 <= y && y < tilemapHeight)) return;
		tilemapData[y][x] = data;
		// ？修改 tilemapData 的同时直接写进 tilemap ...
		// ？而不必在固定时间 全部写进 tilemap ...
		//tilemap.SetTile(new Vector3Int(x, y, 0), tileList[tilemapData[y][x]]);
		//ShowTile(x, y, tilemapData[y][x]);
		ShowTileMapData(x, y, tilemapData[y][x]);
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
			//if (GetTilemapData(pos) != 0) {
			if (GetTilemapDataEx(pos) != 0) {
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
	// ？为了能在切换选中图形的时候有一个固定顺序 改 字典 为 列表 ...
	// ？为了能保留info，使用KeyValuePair，不过这样就不能对list查找了 ...
	// ？若不查找，则需要维护当前选中图形的索引号 ...
	//public List<KeyValuePair<TetrisShape, TetrisShapeInfo>> shapeList = new List<KeyValuePair<TetrisShape, TetrisShapeInfo>>();

	// ？暂时直接存方块的列表，查找直接使用遍历 ...
	public List<TetrisShape> shapeList = new List<TetrisShape>();

	public void AddShape(TetrisShape shape) {
		TetrisShapeInfo info = new TetrisShapeInfo(shape);
		shapeDict.Add(shape, info);

		shapeList.Add(shape);
	}

	public TetrisShapeInfo RemoveShape(TetrisShape shape) {
		if (shapeDict.ContainsKey(shape)) {
			TetrisShapeInfo info = shapeDict[shape];
			shapeDict.Remove(shape);

			// ？刷新选中 ...
			TetrisShape newSelShape;
			if (shape == selectedShape) {
				if (shapeList.Count > 1) {
					newSelShape = shapeList[NextSelIndex()];
				}
				else {
					newSelShape = null;
				}
			}
			else {
				newSelShape = selectedShape;
			}
			//newSelShape = shape == selectedShape ? shapeList[NextSelIndex()] : selectedShape;
			//newSelShape = newSelShape == shape ? null : newSelShape;
			// ？确定新的图形或当前图形，再用selectedShape的setter计算其索引 ...
			//selectedShape = newSelShape;
			// ？需要在移除shape后 再查找其索引 ...
			
			// ？移除的方块必然是停止的 ...
			// ？所以只需要在判断停止的逻辑和移除的逻辑中，选一处插入刷新选中的逻辑即可 ...
			// ？这里选择在移除的时候 ...
			shapeList.Remove(shape);
			selectedShape = newSelShape;
			//SetSelectedShape(newSelShape);

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
					//if (CheckShapeStop(shape)) {
					//	//StopShape(shape);
					//	MarkStopShape(shape);
					//}
					// ？在下降之后不再立刻判断结束 ...
					// ？这样可以允许在即将结束前再次移动 ...
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
		//CheckSelectedShapeStop(shape);

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


	#region 选中的图形

	//[System.NonSerialized]
	//public TetrisShape selectedShape = null;
	// ？改用索引存储选中图形 ...
	// ？再为selectedShape写getter,setter访问器，以继续支持之前的操作 ...
	[System.NonSerialized]
	public int selectedShapeIndex = -1;
	public TetrisShape selectedShape {
		get {
			if (0 <= selectedShapeIndex && selectedShapeIndex < shapeList.Count) {
				return shapeList[selectedShapeIndex];
			}
			else {
				return null;
			}
		}
		set {
			//selectedShapeIndex = shapeList.FindIndex(value); 
			// ？System.predicate<T> match ...
			// ？暂时直接遍历 ...
			if (value == null) {
				selectedShapeIndex = -1;
			}
			else {
				for (int i = 0; i < shapeList.Count; i++) {
					if (shapeList[i] == value) {
						selectedShapeIndex = i;
						break;
					}
				}
			}
		}
	}

	public void SetSelectedShape(TetrisShape shape) {
		// ？改变选中图形前 需要用之前图形的信息清除光标 ...
		//ClearCursorShow();
		selectedShape = shape;
	}

	public void UnsetSelectedShape() {
		SetSelectedShape(null);
	}

	/*
	// ？在停止图形的时候 判断是否是被选中的图形 若是则清除光标 ...
	public bool CheckSelectedShapeStop(TetrisShape shape) {
		if (shape == selectedShape) {
			//UnsetSelectedShape();
			// ？可以切换选中之后 选中方块停止后不是清楚选中 而是选择下一个 ...
			...
			return true;
		}
		else {
			return false;
		}
	}
	*/
	// ？移进停止方块或移除方块的逻辑中 ...

	#endregion


	#region 方块移动和旋转控制

	/*
	public float keyInterval = 0.1f;
	[System.NonSerialized]
	public float lastKeyTime = -60f;
	
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
	*/

	// ？不是限制每次触发的间隔 ...
	// ？而是应该把判定的Input.GetKey改成Input.GetKeyDown ...
	
	public float keyInterval = 0.08f;
	[System.NonSerialized]
	public float lastKeyTimeLR = -60f;
	[System.NonSerialized]
	public float lastKeyTimeD = -60f;
	
	public bool allowKeyDownLR {
		get { return Time.time - lastKeyTimeLR >= keyInterval; }
	}
	public bool allowKeyDownD {
		get { return Time.time - lastKeyTimeD >= keyInterval; }
	}
	
	public void RefreshKeyTimeLR() {
		lastKeyTimeLR = Time.time;
	}
	public void RefreshKeyTimeD() {
		lastKeyTimeD = Time.time;
	}

	// ？TODO : 自己写一个控制类 ...

	public void UpdateShapeMove() {
		if (selectedShape != null) {
			if (Input.GetKey(KeyCode.LeftArrow) && allowKeyDownLR) {
				InvokeShapeMoveIfCan(selectedShape, ShapeMoveType.left);
				RefreshKeyTimeLR();
			}
			else if (Input.GetKey(KeyCode.RightArrow) && allowKeyDownLR) {
				InvokeShapeMoveIfCan(selectedShape, ShapeMoveType.right);
				RefreshKeyTimeLR();
			}
			else if (Input.GetKeyDown(KeyCode.UpArrow)) {
				//InvokeShapeMoveIfCan(selectedShape, ShapeMoveType.spin);
				InvokeShapeMoveIfCan(selectedShape, ShapeMoveType.spinInv);
			}
			//else if (Input.GetKeyDown(KeyCode.DownArrow) && allowKeyDownD) {
			else if (Input.GetKey(KeyCode.DownArrow) && allowKeyDownD) {
				InvokeShapeMoveIfCan(selectedShape, ShapeMoveType.down);
				RefreshKeyTimeD();
			}

			// ？TODO : 允许独立移动，即全为if，不要else ...
		}
	}

	#endregion


	#region 显示到tilemap上

	public TileBase GetTileBase(int data) {
		return 0 <= data && data < tileList.Length ? tileList[data] : null;
	}

	// ？显示到tilemap上 允许超出tilemapData的范围 ...
	/*
	public void ShowTile(int x, int y, int data) {
		tilemap.SetTile(new Vector3Int(x, y, 0), GetTileBase(data));
		//tilemap.SetTile(new Vector3Int(x, y, data), GetTileBase(data));
		// ？测试用 结果4号图块大范围出现 ...
		// ？第一个参数的z值表示图层，即显示的顺序 ...
	}
	public void ShowTile(Vector2Int pos, int data) {
		ShowTile(pos.x, pos.y, data);
	}
	*/
	// ？加上对图层的配置 ...
	public void ShowTile(int x, int y, int data, int layer = 0) {
		tilemap.SetTile(new Vector3Int(x, y, layer), GetTileBase(data));
	}
	public void ShowTile(Vector2Int pos, int data, int layer = 0) {
		ShowTile(pos.x, pos.y, data, layer);
	}
	

	public enum TileLayer {
		mapData, cursor, shape
	}

	public void ShowTileMapData(int x, int y, int data) {
		ShowTile(x, y, data, (int)TileLayer.mapData);
	}
	public void ShowTileMapData(Vector2Int pos, int data) {
		ShowTileMapData(pos.x, pos.y, data);
	}
	
	public void ShowTileShape(int x, int y, int data) {
		ShowTile(x, y, data, (int)TileLayer.shape);
	}
	public void ShowTileShape(Vector2Int pos, int data) {
		ShowTileShape(pos.x, pos.y, data);
	}

	//public int cursorTileIndex = 5;
	//public void ShowTileCursor(int x, int y) {
	//	ShowTile(x, y, cursorTileIndex, (int)TileLayer.cursor);
	//}
	//public void ShowTileCursor(Vector2Int pos) {
	//	ShowTileCursor(pos.x, pos.y);
	//}
	public void ShowTileCursor(int x, int y, int data) {
		ShowTile(x, y, data, (int)TileLayer.cursor);
	}
	public void ShowTileCursor(Vector2Int pos, int data) {
		ShowTileCursor(pos.x, pos.y, data);
	}

	public void InitLayerOrder() {
		// ？测试是否是 以第一次设置瓦片的顺序作为显示顺序 ...
		/*
		ShowTileMapData(0, 0, 0);
		ShowTileMapData(0, 0, -1);
		ShowTileCursor(0, 0, 0);
		ShowTileCursor(0, 0, -1);
		ShowTileShape(0, 0, 0);
		ShowTileShape(0, 0, -1);
		*/
		// ？结果 不是这样的 ...
		
		// ？把MapData图层改为10进行测试 ...
		/*
		ShowTileMapData(-1, 5, 3);
		ShowTileCursor(-1, 3, 2);
		ShowTileShape(-1, 1, 1);
		*/
		/*
		ShowTileMapData(-1, 5, 3); // 3
		ShowTileCursor(-1, 5, 2);
		ShowTileCursor(-1, 3, 2);
		ShowTileShape(-1, 3, 1); // 1
		ShowTileShape(-1, 1, 1);
		ShowTileMapData(-1, 1, 3); // 3
		*/
		/*
		ShowTileMapData(-1, 3, 3); // 3
		ShowTileCursor(-1, 3, 2);
		ShowTileShape(-1, 3, 1);
		*/
		
		// ？把MapData图层改为10与cursor进行测试 ...
		/*
		ShowTileCursor(-1, 5, 2); // 0
		ShowTileMapData(-1, 5, 0);

		ShowTileMapData(-1, 3, 0); // 0
		ShowTileCursor(-1, 3, 2);
		
		ShowTileCursor(-1, 1, 2);
		ShowTileMapData(-1, 1, 0); // 0
		ShowTileCursor(-1, 1, 2);
		*/
		
		// ？把MapData图层改为10与cursor进行测试 ...
		/*
		ShowTileCursor(-1, 5, 0);
		ShowTileMapData(-1, 5, 5); // 5

		ShowTileMapData(-1, 3, 0);
		ShowTileCursor(-1, 3, 5); // 5
		
		ShowTileCursor(-1, 1, 5); // 5
		ShowTileMapData(-1, 1, 0);
		ShowTileCursor(-1, 1, 5);  // 5
		*/
		
		// ？把MapData图层改为0与cursor进行测试 ...
		/*
		ShowTileCursor(-1, 7, 5); // 5
		ShowTileMapData(-1, 7, 0);

		ShowTileCursor(-1, 5, 0);
		ShowTileMapData(-1, 5, 5); // 5
		
		ShowTileCursor(-1, 3, 0);
		ShowTileMapData(-1, 3, 1); // 1
		
		ShowTileCursor(-1, 1, 1);
		ShowTileMapData(-1, 1, 0); // 0
		*/

		// ？与图层无关  与瓦片有关 ...

	}
	// ？...

	#endregion
	
	
	#region 显示图形

	public void ClearShapeShow() {
		Vector2Int[] list;
		//int data;
		foreach (TetrisShape shape in shapeDict.Keys) {
			list = shape.GetOccupyPositionList();
			foreach (Vector2Int pos in list) {
				//ShowTile(pos, 0);
				//data = GetTilemapData(pos);
				//data = data == -1 ? 0 : data;
				//ShowTile(pos, data);
				//ShowTileShape(pos, data);
				// ？因为有分图层 所以直接设为null 就可以了 ...
				ShowTileShape(pos, -1);
			}
		}

		//ClearCursorShow();
	}

	public void ShowAllShape() {
		//RefreshCursorShow();

		Vector2Int[] list;
		foreach (TetrisShape shape in shapeDict.Keys) {
			list = shape.GetOccupyPositionList();
			foreach (Vector2Int pos in list) {
				//ShowTile(pos, shape.GetSquareByGlobal(pos));
				ShowTileShape(pos, shape.GetSquareByGlobal(pos));
			}
		}

		//RefreshCursorShow();
	}

	#endregion


	#region 刷新显示地图数据

	public void RefreshTilemapShow() {
		for (int y = 0; y < tilemapHeight; y++) {
			for (int x = 0; x < tilemapWidth; x++) {
				//ShowTile(x, y, GetTilemapData(x, y));
				ShowTileMapData(x, y, GetTilemapData(x, y));
			}
		}
		for (int y = tilemapHeight; y < tilemapHeightEx; y++) {
			for (int x = 0; x < tilemapWidth; x++) {
				//ShowTile(x, y, 0);
				//ShowTile(x, y, -1);
				ShowTileMapData(x, y, -1);
			}
		}
		ShowAllShape();
	}

	#endregion


	#region 显示光标
	
	/*
	// selectedShape

	[System.NonSerialized]
	public int cursorTileIndex = 5;

	public void RefreshCursorShow() {
		// ？对方块的所有占据格 显示比一个方格稍大的图形到方块图层下 ...
		if (selectedShape != null) {
			Vector2Int[] list = selectedShape.GetOccupyPositionList();
			foreach (Vector2Int pos in list) {
				ShowTileCursor(pos, cursorTileIndex);
			}
		}
	}

	public void ClearCursorShow() {
		if (selectedShape != null) {
			Vector2Int[] list = selectedShape.GetOccupyPositionList();
			foreach (Vector2Int pos in list) {
				ShowTileCursor(pos, -1);
			}
		}
	}
	*/
	
	// ？需要让被选中的图形顶置，但是?渲染顺序是按瓦片顺序 ...
	// ？把逻辑交给 TetrisSelShapeTilemap 处理 ...

	#endregion

	
	#region 判断和消除行

	public void UpdateCheckEliminateLine() {
		CheckEliminateAllLine();
		// TODO : 增加分数 等
	}

	public int CheckEliminateAllLine() {
		// 暂时直接判断所有行 而不是只判断变动的行
		
		//int[] yList = new int[tilemapHeight];
		//int size = 0;
		//for (int y = 0; y < tilemapHeight; y++) {
		//	if (CheckLine(y)) {
		//		yList[size++] = y;
		//	}
		//}
		//EliminateLines(yList, size);
		//return size;
		List<int> list = new List<int>(); // TODO : 写成静态成员
		for (int y = 0; y < tilemapHeight; y++) {
			if (CheckLine(y)) {
				list.Add(y);
			}
		}
		EliminateLines(list);
		return list.Count;
		// 返回消除行数 用于在其他地方计算分数等
	}

	public bool CheckLine(int y) {
		for (int x = 0; x < tilemapWidth; x++) {
			if (GetTilemapData(x, y) == 0) { // 暂时不包括小于0的
				return false;
			}
		}
		return true;
	}
	
	//public void EliminateLines(int[] yList, int size) {
	public void EliminateLines(List<int> list) {
		if (list.Count <= 0) return ;

		// TODO : 调用 播放音效和动画 等

		int index = 0;
		for (int y = 0; y < tilemapHeight; y++) {
			if (!list.Contains(y)) {
			// ？用判断是否存在list的方式 而不是循环list ...
			// ？这样可以允许list无序 ...

				// line[y] copy to line[index++] and clear line[y]

				// ？这里暂时直接 替换数组引用 和 重新创建数组 ...
				//tilemapData[index++] = tilemapData[y];
				//tilemapData[y] = new int[tilemapWidth];
				// ...
				// ？这样会 ...
				// ...
				
				for (int x = 0; x < tilemapWidth; x++) {
					if (index != y) {
						SetTilemapData(x, index, GetTilemapData(x, y));
					}
				}
				index++;
			}
		}
		// ？之后还要处理未处理的index ~ 结尾行
		for (int y = index; y < tilemapHeight; y++) {
			for (int x = 0; x < tilemapWidth; x++) {
				SetTilemapData(x, y, 0);
			}
		}
	}

	#endregion


	#region 切换选中图形
	
	public int NextSelIndex() {
		if (shapeList.Count == 0) {
			return -1;
		}
		else {
			return (selectedShapeIndex + 1) % shapeList.Count;
		}
	}
	// ？计算而不赋值 ...
	// ？仅是计算下一个，而不直接参与逻辑中的修改 ...

	public int LastSelIndex() {
		if (shapeList.Count == 0) {
			return -1;
		}
		else {
			return (selectedShapeIndex - 1 + shapeList.Count) % shapeList.Count;
		}
	}

	//public int NextSelIndexBeforeRemove() {
	//	if (shapeList.Count == 0) {
	//	}
	//	else if (shapeList.Count == 1) {
	//	}
	//	else {
	//	}
	//}
	// ？不只删除选中图形时 需要处理选中索引 ...
	// ？选中图形索引在删除图形索引后时 也要处理索引 ...
	// ？与其分情况考虑索引的变化 ...
	// ？不如直接重新计算索引，就像重新赋值选中图形那样 ...

	public void SelectNextShape() {
		selectedShapeIndex = NextSelIndex();
	}

	public void SelectLastShape() {
		selectedShapeIndex = LastSelIndex();
	}
	

	[System.NonSerialized]
	public float lastKeyTimeAS = -60f;
	public bool allowKeyDownAS {
		get { return Time.time - lastKeyTimeAS >= keyInterval; }
	}
	public void RefreshKeyTimeAS() {
		lastKeyTimeAS = Time.time;
	}

	public void UpdateSelectShape() {
		if (Input.GetKeyDown(KeyCode.A) && allowKeyDownAS) {
			SelectLastShape();
			RefreshKeyTimeAS();
		}
		else if (Input.GetKeyDown(KeyCode.S) && allowKeyDownAS) {
			SelectNextShape();
			RefreshKeyTimeAS();
		}
	}

	#endregion


	#region 产生下落方块

	//public float CreateInterval = 1f;
	//[System.NonSerialized]
	//public float lastCreateTime = -60f;
	
	[System.NonSerialized]
	public float lastKeyTimeZ = -60f;
	public bool allowKeyDownZ {
		get { return Time.time - lastKeyTimeZ >= keyInterval; }
	}
	public void RefreshKeyTimeZ() {
		lastKeyTimeAS = Time.time;
	}

	public void UpdateCreateNewShape() {
		// 暂时这样创建新的下落图形
		//if (Input.GetKey(KeyCode.Z) && Time.time - lastCreateTime > CreateInterval) {
		if (Input.GetKeyDown(KeyCode.Z) && allowKeyDownZ) {
			TetrisShape shape = new TetrisShape();
			shape.SetPosition(GetTopCenterPosition());
			AddShape(shape);

			//lastCreateTime = Time.time;
			RefreshKeyTimeZ();

			//selectedShape = shape;
			// ？因为更换selectedShape的时候还要变换其他的东西 ...
			// ？比如还要清除选中方块的光标等 ...
			// ？所以要把更换selectedShape写成一个方法或setter 在方法里完成其他操作 ...
			// ？对selectedShape更安全、严格的管理，即是扩展其getter,setter，在其中完成其他操作使得安全 ...
			// TODO : 笔记
			//SetSelectedShape(shape);
			if (shape != null) {
				SetSelectedShape(shape);
			}
		}
	}

	#endregion

}

// ？分为活动的方块和停止的方块 ...
// ？活动的方块是一个整体，储存一个整体 ...
// ？停止的方块可能被部分消除，所以直接转化成每一个小方格储存 ...
