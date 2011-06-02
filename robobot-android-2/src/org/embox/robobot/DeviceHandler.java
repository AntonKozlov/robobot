package org.embox.robobot;

import android.os.Handler;
import android.os.Message;

public abstract class DeviceHandler extends Handler{
	public abstract void handleMessage(Message msg);
}
