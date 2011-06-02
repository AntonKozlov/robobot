package org.embox.robobot;

import java.io.IOException;

import org.embox.robobot.proto.IProtocol;
import org.embox.robobot.transport.BluetoothTransport;

import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;

public class BtDevice implements IDevice, IProtocol {
	String devId;
	String name;
	
	IProtocol proto;
	BluetoothTransport transport;
	BluetoothDevice btDevice;
	BluetoothSocket socket;
	DeviceHandler deviceHandler;

	DeviceThread deviceThread;
	Handler threadHandler; 
	
	int deviceState = DEVICE_NULL;
	
	public int getDeviceState() {
		return deviceState;
	}

	BtDevice(String devId, IProtocol proto, BluetoothDevice device) {
		this.devId = devId;
		this.proto = proto;
	}
	
	BtDevice(String devId, String name, IProtocol proto, BluetoothDevice device) {
		this(devId, proto, device);
		this.name = name;
	}
	
	@Override
	public byte[] setControl(int[] control) {
		return null;
	}
	
	private class DeviceThread extends Thread {
		private Handler threadHandler;
		
		public Handler getThreadHandler() {
			return threadHandler;
		}

		private class DeviceThreadHandler extends Handler {
			private DeviceHandler hnd;
			public DeviceThreadHandler(DeviceHandler hnd) {
				this.hnd = hnd;
			}
			@Override
			public void handleMessage(Message msg) {
				switch (msg.what) {
				case IDevice.REQUEST_CONNECT:
					try {
						socket = btDevice.createRfcommSocketToServiceRecord(BluetoothTransport.BT_UUID);
					} catch (IOException e) {
						Message.obtain(hnd, IDevice.RESULT_CONNECT_ERROR).sendToTarget();
					}
					deviceState = DEVICE_CONNECTED;
					Message.obtain(hnd, IDevice.RESULT_CONNECT_OK).sendToTarget();
					break;
				case IDevice.REQUEST_DISCONNECT:
					try {
						socket.close();
					} catch (IOException e) {
						Message.obtain(hnd, IDevice.RESULT_DISCONNECT_ERROR).sendToTarget();
					}
					deviceState = DEVICE_DISCONNECTED;
					Message.obtain(hnd, IDevice.RESULT_DISCONNECT_OK);
				}
			}
		}
		
		public DeviceThread(DeviceHandler hnd) {
			Looper.prepare();
			threadHandler = new DeviceThreadHandler(hnd);
		}
		
		@Override
		public void run() {
			Looper.loop();
		}
		
	}
	@Override
	public synchronized void connect() {
		if (deviceThread == null) {
			deviceThread = new DeviceThread(deviceHandler);
			deviceThread.start();
			threadHandler = deviceThread.getThreadHandler();
		}
		threadHandler.obtainMessage(IDevice.REQUEST_CONNECT).sendToTarget();		
	}
	@Override
	public synchronized void disconnect() {
		threadHandler.obtainMessage(IDevice.REQUEST_DISCONNECT).sendToTarget();
	}
	
	public void setName(String name) {
		this.name = name;
	}
	
	@Override
	public String getName() {
		return name;
	}
	@Override
	public String getId() {
		return devId;
	}
	@Override
	public IProtocol getProto() {
		return proto;
	}
	
	public DeviceHandler getDeviceHandler() {
		return deviceHandler;
	}

	public void setDeviceHandler(DeviceHandler deviceHandler) {
		this.deviceHandler = deviceHandler;
	}
	
	
	
}
