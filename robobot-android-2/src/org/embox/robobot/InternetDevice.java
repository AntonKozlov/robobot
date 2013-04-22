package org.embox.robobot;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Arrays;
import java.util.concurrent.atomic.AtomicBoolean;
import java.net.Socket;

import com.google.protobuf.InvalidProtocolBufferException;
import org.embox.robobot.proto.IProtocol;

import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.util.Log;
import org.embox.robobot.proto.OptionMessage;
import org.embox.robobot.proto.OptionMessageHelper;

public class InternetDevice implements IDevice, IControllable {
	String devId;
	String name;

    int deviceId;
	
	IProtocol proto;
	IControllable controllable;
	Socket socket;
	DeviceHandler deviceHandler;

	DeviceThread deviceThread;
	DeviceReadThread deviceReadThread;
	Handler threadHandler; 
	
	int deviceState = DEVICE_NULL;
	
	public int getDeviceState() {
		return deviceState;
	}

	public InternetDevice(String devId, IProtocol proto, IControllable controllable, Socket socket) {
		this.devId = devId;
		this.proto = proto;
		this.controllable = controllable;
        this.socket = socket;
	}
	
	public InternetDevice(String devId, String name, IProtocol proto, IControllable controllable, Socket socket) {
		this(devId, proto, controllable, socket);
		this.name = name;
	}

    public InternetDevice(String name, Socket socket) {
        this.socket = socket;
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
			byte[] buff = new byte[512];
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
				deviceState = DEVICE_DETERMING;
				Message.obtain(hnd, IDevice.RESULT_CONNECT_OK).sendToTarget();
			}
			
			public void requestDisconnect() {
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
                    if (data != null) {
					deviceOutput.write(data);
                    deviceOutput.flush();
                    }
				} catch (Exception e) {
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

        private void determBotSendStamp() {
            final byte stamp[] = new byte[2];
            stamp[0] = '2';
            stamp[1] = '\n';
            deviceThread.writeData(stamp);
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
				//TODO bot determination
				determBotSendStamp();
			} else {
				outsideHandler.connectOk();
			}
		}
		 
		@Override
		protected void connectError(String error) {
			outsideHandler.connectError(error);
		}
		
		@Override
		protected void readDone(byte[] data, int count) {
			Log.d("robobot", Integer.toString(count)+ Arrays.toString(data));
			if (deviceState == DEVICE_DETERMING) {
				int status = determBotDeterm(data, count);
				if (status == 0) {
					connectError("Cannot determ bot");
					disconnect();
				} else if (status == 2){
					deviceState = DEVICE_CONNECTED;
					connectOk();
				}
			} else {
				outsideHandler.readDone(proto.translateInput(data), count);
			}
		}

        private int determBotDeterm(byte[] data, int count) {
            OptionMessage.OptionMessageEntity message;
            byte[] messageBody = new byte[count - 1];
            System.arraycopy(data, 1, messageBody, 0, count - 1);
            if (data[0] == 0x0) {
                message = OptionMessageHelper.getOptionMessage(messageBody);
                proto = OptionMessageHelper.getDeviceProtocolByType(message.getType());
                controllable = (IControllable)proto;
                if (proto != null) {
                    //TODO as string
                    deviceId = message.getId();
                    return RESULT_DETERM_OK;
                } else {
                    return RESULT_DETERM_ERROR;
                }
            }
            return RESULT_DETERM_ERROR;
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
