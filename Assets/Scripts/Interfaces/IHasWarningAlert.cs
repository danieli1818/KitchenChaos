using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasWarningAlert
{
    public event EventHandler<OnWarningStateChangedEventArgs> OnWarningStateChanged;
    public class OnWarningStateChangedEventArgs : EventArgs {
        public bool isWarningOn;
    }
}
