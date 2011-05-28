package org.embox.robobot;

import org.embox.robobot.proto.IProto;

public interface IDevice {
	void connect();
	void disconnect();
	
	String getName();
	String getId();
	IProto getProto();
	

}
