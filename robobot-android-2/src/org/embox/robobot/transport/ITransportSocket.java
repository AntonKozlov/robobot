package org.embox.robobot.transport;

import org.embox.robobot.IDevice;

public interface ITransportSocket {
	
	byte[] read(int size);
	
	void write(byte[] msg);
	
	void close();
	
	IDevice getDevice();

}
