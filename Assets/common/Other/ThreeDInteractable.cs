using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThreeDInteractable : EventTrigger {
	private static OnCallBackGameObjectIntPointerEventData SceneLuaFunc;
	private Vector2 pressdownPos;
	private Vector2 pressupPos;
    private float dpi = 0;
	private void Start() {
        dpi = Screen.dpi / 96;
        if (SceneLuaFunc == null && XLuaManager.Instance != null && XLuaManager.Instance.GetLuaEnv() != null)
		{
			SceneLuaFunc = XLuaManager.Instance.GetLuaEnv().Global.GetInPath<OnCallBackGameObjectIntPointerEventData>("SceneManager.ThreeDInteract");
		}
	}

    public override void OnPointerClick(PointerEventData data)
    {
			if(SceneLuaFunc != null && Vector2.Distance(pressdownPos,pressupPos) < 5 * dpi)
			{
				SceneLuaFunc.Invoke(gameObject,1,data);
			}
    }

    public override void OnPointerDown(PointerEventData data)
    {
			pressdownPos = data.position;
			if(SceneLuaFunc != null)
			{
				SceneLuaFunc.Invoke(gameObject,2,data);
			}
    }

    public override void OnPointerUp(PointerEventData data)
    {
			pressupPos = data.position;
			if(SceneLuaFunc != null)
			{
				SceneLuaFunc.Invoke(gameObject,3,data);
			}
    }

	public override void OnBeginDrag(PointerEventData data)
    {
			if(SceneLuaFunc != null)
			{
				SceneLuaFunc.Invoke(gameObject,4,data);
			}
    }

	public override void OnDrag(PointerEventData data)
    {
			if(SceneLuaFunc != null)
			{
				SceneLuaFunc.Invoke(gameObject,5,data);
			}
    }

	public override void OnEndDrag(PointerEventData data)
    {
			pressupPos = data.position;
			if(SceneLuaFunc != null)
			{
				SceneLuaFunc.Invoke(gameObject,6,data);
			}
    }

	public static void Dispose()
	{
		SceneLuaFunc = null;
	}
}
