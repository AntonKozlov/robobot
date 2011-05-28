package org.embox.robobot.transport;

import android.content.BroadcastReceiver;

public interface ITransport {
	void init();
	void startScan(BroadcastReceiver receiver);
}
