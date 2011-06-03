package org.embox.robobot;

import org.embox.robobot.transport.ITransport;

import android.os.Handler;
import android.os.Message;

public abstract class ScanDeviceHandler extends Handler{
	public void handleMessage(Message msg) {
		switch (msg.what) {
			case ITransport.DEVICE_FOUND:
				deviceFound((IDevice) msg.obj);
				
				break;
			case ITransport.SCAN_FINISHED:
				scanFinished();
				break;
			default:
				break;
		}
	}
	protected void deviceFound(IDevice device) {
		
	}
	
	protected void scanFinished() {
		
	}
}
