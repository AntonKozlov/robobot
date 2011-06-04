package org.embox.robobot.proto;

public class NxtDirect implements IProtocol {

	@Override
	public byte[] setControl(int[] control) {
		byte[] out = new byte[7];
		out[0] = 5;
		out[1] = 0;
		out[2] = (byte) 0x80;
		out[3] = (byte) 0x20;
		out[4] = (byte) control[0];
		out[5] = (byte) control[1];
		out[6] = (byte) control[2];
		return out;
	}

	@Override
	public byte[] translateInput(byte[] data) {
		return data;
	}
	

}
