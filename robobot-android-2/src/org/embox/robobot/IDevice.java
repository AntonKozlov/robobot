package org.embox.robobot;

import org.embox.robobot.proto.IProtocol;


public interface IDevice {
	int RESULT_CONNECT_OK = 0;
	int RESULT_CONNECT_ERROR = 1;
	
	int RESULT_DISCONNECT_OK = 10;
	int RESULT_DISCONNECT_ERROR = 11;
	
	int REQUEST_CONNECT = 20;
	int REQUEST_DISCONNECT = 21;
	
	public static final int DEVICE_NULL = 1;
	public static final int	DEVICE_DISCONNECTED = 2;
	public static final int DEVICE_СONNECTING = 3;
	public static final int	DEVICE_CONNECTED = 4;
	
	void connect();
	void disconnect();
	
	String getName();
	String getId();
	IProtocol getProto();
	
	
	

}
