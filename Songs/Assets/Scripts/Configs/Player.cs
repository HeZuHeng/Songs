using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainPlayer
{
    public static SongResultInfo songResultInfo;
    public int Id;
    public string name;
    public int fraction;
    public MemoryNumChange OnMemoryNumChange = new MemoryNumChange();
    int memoryNum = 0;
    public int MemoryNum {
        get
        {
            return memoryNum;
        }
        set
        {
            memoryNum = value;
            OnMemoryNumChange?.Invoke(memoryNum);
        }
    }

    public MainPlayer()
    {
        songResultInfo = new SongResultInfo();
    }
}

public class MemoryNumChange : UnityEvent<int>
{
    public MemoryNumChange()
    {

    }
}
