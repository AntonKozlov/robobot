package org.embox.robobot;

import org.embox.robobot.proto.IProto;

public class Device implements IDevice, IProto {
	String devId;
	String name;
	IProto proto;
	
	Device(String devId, IProto proto) {
		this.devId = devId;
		this.proto = proto;
	}
	
	Device(String devId, String name, IProto proto) {
		this(devId, proto);
		this.name = name;
	}
	
	@Override
	public void setControl(int[] control) {
		// TODO Auto-generated method stub
		
	}
	@Override
	public void connect() {
		// TODO Auto-generated method stub
		
	}
	@Override
	public void disconnect() {
		// TODO Auto-generated method stub
		
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
	public IProto getProto() {
		return proto;
	}
}
