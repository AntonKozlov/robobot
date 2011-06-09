package org.embox.robobot;

import org.embox.robobot.transport.ITransport;

import android.os.Handler;
import android.os.Message;

public abstract class ScanDeviceHandler extends Handler {
	public void handleMessage(Message msg) {
		switch (msg.what) {
			case ITransport.DEVICE_FOUND:
				deviceFound((IDevice) msg.obj);				
				break;
			case ITransport.SCAN_FINISHED:
				scanFinished();
				break;
			case ITransport.DEVICE_NAME_CHANGED:
				deviceNameChanged((IDevice) msg.obj);
				break;
			case ITransport.DEVICE_NAME_USER_CHANGED:
				deviceNameUserChanged((IDevice) msg.obj);
				break;
			default:
				break;
		}
	}
	protected void deviceNameUserChanged(IDevice obj) {
		
	}
	protected void deviceNameChanged(IDevice obj) {
		
	}
	protected void deviceFound(IDevice device) {
		
	}
	
	protected void scanFinished() {
		
	}
}
