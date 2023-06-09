using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProgress
{
    public event EventHandler<OnProgressUpdateEventArgs> OnProgressUpdate;
    public class OnProgressUpdateEventArgs {
        public float progress;
    }
}
