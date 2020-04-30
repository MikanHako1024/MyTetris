using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

// ？TODO : 写一个基类 ...

// ？TODO : 或者合并进 TetrisTilemap ...

public class TetrisSelShapeTilemap : MonoBehaviour {
	
	public UnityEngine.Tilemaps.Tilemap tilemap;
	public TetrisTilemap tetrisTilemap;

	private void Start() {
		if (tilemap == null) {
			tilemap = GetComponent<UnityEngine.Tilemaps.Tilemap>();
		}
		if (tetrisTilemap == null) {
			tetrisTilemap = FindObjectOfType<TetrisTilemap>();
		}
	}
	
	private void FixedUpdate () {
		UpdateSelectedShapeShow();
	}

	// ？TODO : 是否会因为TetrisSelShapeTilemap和TetrisTilemap的处理顺序问题而出错 ...

	
	#region 显示到tilemap上

	// ？TODO : 写一个基类 ...

	public TileBase[] tileList {
		get {
			return tetrisTilemap.tileList;
		}
	}

	public TileBase GetTileBase(int data) {
		return 0 <= data && data < tileList.Length ? tileList[data] : null;
	}

	public void ShowTile(int x, int y, int data, int layer = 0) {
		tilemap.SetTile(new Vector3Int(x, y, layer), GetTileBase(data));
	}
	public void ShowTile(Vector2Int pos, int data, int layer = 0) {
		ShowTile(pos.x, pos.y, data, layer);
	}


	public enum TileLayer {
		cursor, shape
	}
	
	public void ShowTileShape(int x, int y, int data) {
		ShowTile(x, y, data, (int)TileLayer.shape);
	}
	public void ShowTileShape(Vector2Int pos, int data) {
		ShowTileShape(pos.x, pos.y, data);
	}
	
	public void ShowTileCursor(int x, int y, int data) {
		ShowTile(x, y, data, (int)TileLayer.cursor);
	}
	public void ShowTileCursor(Vector2Int pos, int data) {
		ShowTileCursor(pos.x, pos.y, data);
	}

	#endregion


	#region 显示和清除图形
	
	public void ShowShape(TetrisShape shape) {
		if (shape != null) {
			Vector2Int[] list = shape.GetOccupyPositionList();
			foreach (Vector2Int pos in list) {
				ShowTileShape(pos, shape.GetSquareByGlobal(pos));
			}
		}
	}

	public void ClearShape(TetrisShape shape) {
		if (shape != null) {
			Vector2Int[] list = shape.GetOccupyPositionList();
			foreach (Vector2Int pos in list) {
				ShowTileShape(pos, -1);
			}
		}
	}

	#endregion


	#region 显示被选中的图形

	public TetrisShape selectedShape {
		get {
			return tetrisTilemap.selectedShape;
		}
	}

	[System.NonSerialized]
	public TetrisShape lastSelectedShape = null;
	
	public void ShowSelectedShape() {
		ShowShape(selectedShape);
		//lastSelectedShape = selectedShape;
		// ？需要保留的是之前的图形的位置 ...
		// ？若用引用，获取的位置也会随之改变 ...
		// ？所以需要复制 ...
		//if (lastSelectedShape != null) {
		//	selectedShape.CopyTo(lastSelectedShape);
		//}
		//else {
		//	lastSelectedShape = selectedShape.CopyTo();
		//}
		// ？要需要判断 selectedShape 非null ...
		if (selectedShape != null) {
			if (lastSelectedShape != null) {
				selectedShape.CopyTo(lastSelectedShape);
			}
			else {
				lastSelectedShape = selectedShape.CopyTo();
			}
		}
	}

	public void ClearLastShape() {
		ClearShape(lastSelectedShape);
		//lastSelectedShape = null;
	}

	public void UpdateSelectedShapeShow() {
		ClearCursorShow();
		ClearLastShape();
		RefreshCursorShow();
		ShowSelectedShape();
	}

	#endregion


	#region 显示光标

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
		//if (selectedShape != null) {
		//	Vector2Int[] list = selectedShape.GetOccupyPositionList();
		if (lastSelectedShape != null) {
			Vector2Int[] list = lastSelectedShape.GetOccupyPositionList();
			foreach (Vector2Int pos in list) {
				ShowTileCursor(pos, -1);
			}
		}
	}

	// ？TetrisSelShapeTilemap自己获取数据并刷新 ...
	// ？而不需要TetrisTilemap控制 ...
	// ？TetrisTilemap也可以独立于TetrisSelShapeTilemap运作 ...

	#endregion

}
