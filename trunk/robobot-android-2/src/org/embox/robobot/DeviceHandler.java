package org.embox.robobot;

import android.os.Handler;
import android.os.Message;

public class DeviceHandler extends Handler {
	public void handleMessage(Message msg) {
		switch (msg.what) {
			case IDevice.RESULT_INIT_OK:
				initOk();
				break;
			case IDevice.RESULT_CONNECT_OK:
				connectOk();
				break;
			case IDevice.RESULT_CONNECT_ERROR:
				connectError((String) msg.obj);
				break;
			case IDevice.RESULT_DISCONNECT_OK:
				disconnectOk();
				break;
			case IDevice.RESULT_DISCONNECT_ERROR:
				disconnectError();
				break;
			case IDevice.RESULT_WRITE_DONE:
				writeDone();
				break;
			case IDevice.RESULT_READ_DONE:
				readDone((byte[]) msg.obj);
				break;
			default:
				break;
		}
	}
	
	protected void readDone(byte[] data) {
		
	}

	protected void writeDone() {
		
	}

	protected void disconnectError() {
		
	}

	protected void disconnectOk() {
		
	}

	protected void connectOk() {
		
	}
	
	protected void connectError(String error) {
		
	}
	
	protected void initOk() {
		
	}
}
