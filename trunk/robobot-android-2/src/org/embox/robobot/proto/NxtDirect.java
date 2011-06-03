package org.embox.robobot.proto;

public class NxtDirect implements IProtocol {

	@Override
	public byte[] setControl(int[] control) {
		return Integer.toString(control[0]).getBytes();
	}

	@Override
	public byte[] translateInput(byte[] data) {
		return data;
	}
	

}
