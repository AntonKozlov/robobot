package org.embox.robobot.proto;

public class ProtocolNxtDirect extends ProtocolNxtEmbox { 
	@Override
	public byte[] translateOutput(int[] control) {
		byte[] m1 = new byte[11];
		byte[] m2 = new byte[11];
		m1[0] = 0;
		m1[1] = (byte) control[0];
		m1[2] = 0x01;
		m1[3] = 0x01;
		m1[5] = 0x20;
		
		m2[0] = 1;
		m2[1] = (byte) control[1];
		m2[2] = 0x01;
		m2[3] = 0x01;
		m2[5] = 0x20;
		
		byte[] motor1Cmd = super.nxtPacket(false, (byte) 0x04, m1);
		byte[] motor2Cmd = super.nxtPacket(false, (byte) 0x04, m2);
		byte[] out = new byte[2 * motor1Cmd.length];
		int pos = 0;
		for (byte b : motor1Cmd) {
			out[pos++] = b;
		}
		for (byte b : motor2Cmd) {
			out[pos++] = b;
		}
		return out;
	}
}
