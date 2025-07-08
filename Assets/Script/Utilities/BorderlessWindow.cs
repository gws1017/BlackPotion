using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BorderlessWindow : MonoBehaviour
{

    const int GWL_STYLE = -16; //���ο� ��Ÿ�� ����
    const int SWP_NOMOVE = 0x0002; //���� ��ġ ����
    const int SWP_NOSIZE = 0x0001; //���� ũ�� ����
    const int SWP_SHOWWINDOW = 0x40; // ������ǥ��

    const int WS_BORDER = 0x00800000; //�׵θ�����
    const int WS_DLGFRAME = 0x00400000; //
    const int WS_CAPTION = WS_BORDER | WS_DLGFRAME; //����ǥ���� ����
    const uint WS_SIZEBOX = 0x00040000; //ũ������ �׵θ��� ����
    const uint WS_POPUP = 0x80000000;
    const uint WS_VISIBLE = 0x10000000;

    [DllImport("user32.dll")] static extern System.IntPtr GetActiveWindow();
    [DllImport("user32.dll")] static extern int FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")] static extern System.IntPtr GetWindowLong (System.IntPtr hWnd,int nIndex);
    [DllImport("user32.dll")] static extern System.IntPtr SetWindowLong(System.IntPtr hWnd,int nIndex, uint dwNewLong);
    [DllImport("user32.dll")] static extern bool SetWindowPos(
        System.IntPtr hWnd, System.IntPtr hWndInsertAfter,
        short X, short Y,
        short cx, short cy,
        uint uFlags);

    static readonly System.IntPtr HWND_TOPMOST = new System.IntPtr(-1); // �ֻ��� ��ġ

    //private void Awake()
    //{
    //    hWnd = GetActiveWindow();
    //}
    //void Start()
    //{
    //    HideWindowBorders();
    //}

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void InitializeWindow()
    {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        SetFramelessWindow();
#endif
    }
    public static void SetFramelessWindow()
    {
        var hwnd = GetActiveWindow();
        SetWindowLong(hwnd, GWL_STYLE, WS_POPUP | WS_VISIBLE);
    }
    public static void HideWindowBorders()
    {
        System.IntPtr hWnd = GetActiveWindow();

        int style = GetWindowLong(hWnd,GWL_STYLE).ToInt32();

        SetWindowLong(hWnd, GWL_STYLE, (uint)(style & ~(WS_CAPTION | WS_SIZEBOX)));
        SetWindowPos(hWnd,HWND_TOPMOST,0,0,0,0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
    }

}
