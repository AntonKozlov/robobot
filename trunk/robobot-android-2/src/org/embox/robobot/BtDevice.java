package org.embox.robobot;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.lang.reflect.Method;
import java.util.Arrays;
import java.util.concurrent.atomic.AtomicBoolean;

import org.embox.robobot.proto.*;
import org.embox.robobot.proto.ConfigurationMessage.DeviceConfigurationMessage;
import org.embox.robobot.transport.BluetoothTransport;

import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.util.Log;

public class BtDevice implements IDevice, IControllable {
	/**
	 * Auto generated, doesn't know AK
	 */
	String devId;
	String name;
	
	IProtocol proto;
	IControllable controllable;
	BluetoothTransport transport;
	BluetoothDevice btDevice;
	BluetoothSocket socket;
	DeviceHandler deviceHandler;

	DeviceThread deviceThread;
	DeviceReadThread deviceReadThread;
	Handler threadHandler; 
	
	int deviceState = DEVICE_NULL;
	
	public int getDeviceState() {
		return deviceState;
	}

	BtDevice(String devId, IProtocol proto, IControllable controllable, BluetoothDevice device) {
		this.devId = devId;
		this.proto = proto;
		this.controllable = controllable;
		btDevice = device;
	}
	
	BtDevice(String devId, String name, IProtocol proto, IControllable controllable, BluetoothDevice device) {
		this(devId, proto, controllable, device);
		this.name = name;
	}
	
	public int[] setControl(int[] control) {
		int[] ctrl = controllable.setControl(control);
		write(proto.translateOutput(ctrl));
		return ctrl;
	}
	
	private class DeviceReadThread extends Thread {
		private DeviceHandler handler;
		private InputStream stream;
		private final AtomicBoolean aBool = new AtomicBoolean(true);
		
		public DeviceReadThread(DeviceHandler handler, InputStream stream) {
			this.handler = handler;
			this.stream = stream;
		}
	
		public void setNotRun() {
			aBool.set(false);
		}
		
		@Override
		public void run() {
			byte[] buff = new byte[5];
			int count;
			while (aBool.get()) {
				try {
					count = stream.read(buff);
					handler.obtainMessage(IDevice.RESULT_READ_DONE, count, 0, buff.clone()).sendToTarget();
				} catch (IOException e) {
					//disconnect();
					e.printStackTrace();
				}
			}
		}
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
				if (deviceState < DEVICE_DISCONNECTED) {
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
					String exString = e1.getMessage();
					try {
						socket.close();
					} catch (IOException e) {
						exString.concat(" + " + e.getMessage());
						// TODO Auto-generated catch block
						e.printStackTrace();
					}
					e1.printStackTrace();
					Message.obtain(hnd, IDevice.RESULT_CONNECT_ERROR,exString).sendToTarget();
					return;
				}
				deviceState = DEVICE_DETERMING;
				Message.obtain(hnd, IDevice.RESULT_CONNECT_OK).sendToTarget();
			}
			
			public void requestDisconnect() {
		//		if (deviceState < DEVICE_CONNECTED) {
			//		throw new IllegalStateException("BtDevice thread illegal state change");
		//		}
				try {
					deviceReadThread.setNotRun();
					deviceReadThread = null;
					socket.close();
				} catch (IOException e) {
					Message.obtain(hnd, IDevice.RESULT_DISCONNECT_ERROR).sendToTarget();
					return;
				}
				deviceState = DEVICE_DISCONNECTED;
				Message.obtain(hnd, IDevice.RESULT_DISCONNECT_OK);
			}
			
			public void requestWrite(byte[] data) {
				if (deviceState < DEVICE_CONNECTED) {
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
	
	private void determBotSendStamp() {
		final byte stamp[] = new byte[3];
        stamp[0] = 1;
        stamp[1] = 0;
        stamp[2] = 0;
//		threadHandler.obtainMessage(IDevice.REQUEST_WRITE, stamp).sendToTarget();
		threadHandler.postDelayed(new Runnable() {
			
			@Override
			public void run() {
				deviceThread.writeData(stamp);
			}
		}, 1000);
		//write(stamp);
	}
	
	private byte[] botHeader = {0x0, 0x2, 0xd};
	private int botHeaderLen = botHeader.length;
	private int headerPos = 0;
	
	private int determBotDeterm(byte[] data, int count) {
               DeviceConfigurationMessage message;
        byte[] messageBody = new byte[count - 2];
        System.arraycopy(data, 2, messageBody, 0, count - 2);
        if (data[0] == 0) {
            try {
                message = ConfigurationMessageHelper.getOptionMessage(messageBody);
                if (message == null) {
                    return 1;
                }
            } catch (Exception e) {
                return 1;
            }
            proto = ConfigurationMessageHelper.getDeviceProtocolByType(message.getType());
            controllable = (IControllable)proto;
            if (proto != null) {
                devId = message.getId();
                proto.setConfig(message);
                return RESULT_DETERM_OK;
            } else {
                return RESULT_DETERM_ERROR;
            }
        }
        return RESULT_DETERM_ERROR;
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
			InputStream stream;
			try {
				stream = socket.getInputStream();
			} catch (IOException e) {
				// cant happened
				e.printStackTrace();
				return;
			}
			if (deviceState == DEVICE_DETERMING) {
				deviceReadThread = new DeviceReadThread(this, stream);
				deviceReadThread.start();
				determBotSendStamp();
			} else {
				outsideHandler.connectOk();
			}
		}
		 
		@Override
		protected void connectError(String error) {
			outsideHandler.connectError(error);
		}

        private byte[] message = new byte[0];
		@Override
		protected void readDone(byte[] data, int count) {
			Log.d("robobot", Integer.toString(count)+ Arrays.toString(data));
			if (deviceState == DEVICE_DETERMING) {
                int status = -1;
                if (message.length == 0){
				    status = determBotDeterm(data, count);
                } else {
                    byte[] oldMessage = new byte[message.length];
                    System.arraycopy(message, 0, oldMessage, 0, message.length);
                    message = new byte[oldMessage.length + count];
                    System.arraycopy(oldMessage, 0, message, 0, oldMessage.length);
                    System.arraycopy(data, 0, message, oldMessage.length, count);
                    status = determBotDeterm(message, message.length);
                }
				if (status == 0) {
					connectError("Cannot determ bot");
					disconnect();
				} else if (status == 2){
					deviceState = DEVICE_CONNECTED;
					connectOk();
				} else if (status == 1){
                    if (message.length == 0) {
                        message = new byte[count];
                        System.arraycopy(data, 0, message, 0, count);
                    }
                    return;
                }
			} else {
				outsideHandler.readDone(proto.translateInput(data), count);
			}
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
	
	private void write(byte[] data) {
		//threadHandler.obtainMessage(IDevice.REQUEST_WRITE, data).sendToTarget();
		deviceThread.writeData(data);
	}
	@Override
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

	@Override
	public void calibrate(int[] control) {
		controllable.calibrate(control);
	}
	
}
