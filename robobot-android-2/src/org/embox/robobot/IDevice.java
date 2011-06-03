package org.embox.robobot;

import org.embox.robobot.proto.IProtocol;


public interface IDevice extends IControllable {
	int RESULT_CONNECT_OK = 0;
	int RESULT_CONNECT_ERROR = 1;
	int RESULT_DISCONNECT_OK = 10;
	int RESULT_DISCONNECT_ERROR = 11;
	int RESULT_WRITE_DONE = 20;
	int RESULT_READ_DONE = 21;
	int RESULT_INIT_OK = 30;
	
	
	
	int REQUEST_CONNECT = 100;
	int REQUEST_DISCONNECT = 101;
	int REQUEST_WRITE = 103;
	int REQUEST_CLOSE = 104;
	String ANDROID_DEVICE_UNIQ_STRING = "emboxDevice";
	
	
	public static final int DEVICE_NULL = 1;
	public static final int	DEVICE_DISCONNECTED = 2;
	public static final int DEVICE_СONNECTING = 3;
	public static final int	DEVICE_CONNECTED = 4;
	
	void init();
	void connect();
	void disconnect();
	void close();
	
	void setDeviceHandler(DeviceHandler deviceHandler);
	
	String getName();
	String getId();
	IProtocol getProto();
	
}
