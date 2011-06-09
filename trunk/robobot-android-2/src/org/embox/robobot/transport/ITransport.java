package org.embox.robobot.transport;

import android.content.BroadcastReceiver;

public interface ITransport {
	int DEVICE_FOUND = 1;
	int SCAN_FINISHED = 2;
	int DEVICE_NAME_CHANGED = 3;
	int DEVICE_NAME_USER_CHANGED = 4;

	void startScan(BroadcastReceiver receiver);
	
}
