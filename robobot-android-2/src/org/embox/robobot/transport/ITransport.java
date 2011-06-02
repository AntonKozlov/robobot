package org.embox.robobot.transport;

import android.content.BroadcastReceiver;

public interface ITransport {
	int DEVICE_FOUND = 1;
	int SCAN_FINISHED = 2;

	void startScan(BroadcastReceiver receiver);
	
}
