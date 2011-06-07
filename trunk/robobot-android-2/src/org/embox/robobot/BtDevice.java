package org.embox.robobot;

import java.io.IOException;
import java.io.OutputStream;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

import org.embox.robobot.proto.IProtocol;
import org.embox.robobot.transport.BluetoothTransport;

import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;

public class BtDevice implements IDevice {
	/**
	 * Auto generated, doesn't know AK
	 */
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
		btDevice = device;
	}
	
	BtDevice(String devId, String name, IProtocol proto, BluetoothDevice device) {
		this(devId, proto, device);
		this.name = name;
	}
	
	public void setControl(int[] control) {
		write(proto.setControl(control));
	}
	
	private class DeviceThread extends Thread {
		private DeviceThreadHandler threadHandler;
		private DeviceHandler deviceHandler;
		
		public Handler getThreadHandler() {
			return threadHandler;
		}

		public void writeData(byte[] data) {
			threadHandler.requestWrite(data);
		}
		
		private class DeviceThreadHandler extends Handler {
			private DeviceHandler hnd;
			public DeviceThreadHandler(DeviceHandler hnd) {
				this.hnd = hnd;
			}
			

			public void requestConnect() {
				if (deviceState != DEVICE_DISCONNECTED) {
					throw new IllegalStateException("BtDevice thread illegal state change");
				}
				try {
				//	socket = btDevice.createRfcommSocketToServiceRecord(BluetoothTransport.BT_UUID);
					Method m = btDevice.getClass().getMethod("createRfcommSocket", new Class[] {int.class});
			        socket = (BluetoothSocket) m.invoke(btDevice, 1);
					
				} catch (Exception e) {
					Message.obtain(hnd, IDevice.RESULT_CONNECT_ERROR, e.getMessage()).sendToTarget();
					e.printStackTrace();
					return;
				}
				
				try {
					socket.connect();
				} catch (IOException e1) {
					String exString = e1.toString();
					try {
						socket.close();
					} catch (IOException e) {
						exString.concat(e.getMessage());
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
					e1.printStackTrace();
					Message.obtain(hnd, IDevice.RESULT_CONNECT_ERROR,exString).sendToTarget();
					return;
				}
				deviceState = DEVICE_CONNECTED;
				Message.obtain(hnd, IDevice.RESULT_CONNECT_OK).sendToTarget();
			}
			
			public void requestDisconnect() {
				if (deviceState != DEVICE_CONNECTED) {
					throw new IllegalStateException("BtDevice thread illegal state change");
				}
				try {
					socket.close();
				} catch (IOException e) {
					Message.obtain(hnd, IDevice.RESULT_DISCONNECT_ERROR).sendToTarget();
					return;
				}
				deviceState = DEVICE_DISCONNECTED;
				Message.obtain(hnd, IDevice.RESULT_DISCONNECT_OK);
			}
			
			public void requestWrite(byte[] data) {
				if (deviceState != DEVICE_CONNECTED) {
					throw new IllegalStateException("BtDevice thread illegal state change");
				}
				
				try {
					OutputStream deviceOutput = socket.getOutputStream();
					deviceOutput.write(data);
					//Message.obtain(hnd, IDevice.RESULT_WRITE_DONE).sendToTarget();
				} catch (IOException e) {
					
					// TODO Auto-generated catch block
					// must crash in this case imho AK
					e.printStackTrace();
				}
				
			}
			
			public void requestClose() {
				if (deviceState == DEVICE_CONNECTED) {
					try {
						socket.close();
					} catch (IOException e) {
						// TODO Auto-generated catch block
						// cannot do anything? AK
						e.printStackTrace();
					}
				}
			}
			
			
			@Override
			public void handleMessage(Message msg) {
				switch (msg.what) {
				case IDevice.REQUEST_CONNECT:
					requestConnect();
					break;
				case IDevice.REQUEST_DISCONNECT:
					requestDisconnect();
					break;
				case IDevice.REQUEST_WRITE:
					requestWrite((byte[]) msg.obj);
					break;
				case IDevice.REQUEST_CLOSE:
					requestClose();
					break;
				}
			}
		}
		
		public DeviceThread(DeviceHandler hnd) {
			this.deviceHandler = hnd;
			
		}
		
		@Override
		public void run() { 
			Looper.prepare();
			threadHandler = new DeviceThreadHandler(deviceHandler);
			deviceHandler.obtainMessage(RESULT_INIT_OK).sendToTarget();
			Looper.loop();
			
		}
		
	}
	
	private class DeviceHandlerInternal extends DeviceHandler {
		DeviceHandler outsideHandler;
		public DeviceHandlerInternal(DeviceHandler outsideHandler) {
			this.outsideHandler = outsideHandler;
		}
		@Override
		protected void initOk() {
			threadHandler = deviceThread.getThreadHandler();
			deviceState = DEVICE_DISCONNECTED;
			outsideHandler.initOk();
		}
		
		@Override
		protected void connectOk() {
			outsideHandler.connectOk();
		}
		
		@Override
		protected void connectError(String error) {
			outsideHandler.connectError(error);
		}
		
		@Override
		protected void readDone(byte[] data) {
			outsideHandler.readDone(proto.translateInput(data));
		}
		
		@Override
		protected void writeDone() {
			outsideHandler.writeDone();
		}
	}
	@Override
	public synchronized void init() {
		if (deviceState != DEVICE_NULL) {
			
		} // how this case must be treated? AK 
		threadHandler = null;
		deviceThread = new DeviceThread(new DeviceHandlerInternal(deviceHandler));
		//deviceThread = new DeviceThread(deviceHandler);
		deviceThread.start();
		
		
	}
	@Override
	public synchronized void connect() {
		threadHandler.obtainMessage(IDevice.REQUEST_CONNECT).sendToTarget();		
	}
	@Override
	public synchronized void disconnect() {
		threadHandler.obtainMessage(IDevice.REQUEST_DISCONNECT).sendToTarget();
	}
	
	public synchronized void close() {
		threadHandler.obtainMessage(IDevice.REQUEST_CLOSE).sendToTarget();
	}
	
	private void write(byte[] data) {
		//threadHandler.obtainMessage(IDevice.REQUEST_WRITE, data).sendToTarget();
		deviceThread.writeData(data);
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
	
	@Override
	public void setDeviceHandler(DeviceHandler deviceHandler) {
		this.deviceHandler = deviceHandler;
	}
	
}
