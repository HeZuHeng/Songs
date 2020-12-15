using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MREngine
{
    public delegate void SimpleInitSignal();
    public delegate void SimpleDataSignal(string _url_);
    public delegate void SimpleProgressSignal(float _progress_);
}
