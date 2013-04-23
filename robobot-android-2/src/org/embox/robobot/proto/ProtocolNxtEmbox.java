package org.embox.robobot.proto;

import org.embox.robobot.IControllable;

public class ProtocolNxtEmbox implements IControllable, IProtocol {

	protected int[] calbr = new int [3];
	
	protected double angCalbr;

	protected byte[] nxtPacket(boolean reply, byte command,
			byte[] data) {
		int dataLen;
		if (data != null) {
			dataLen= data.length;
		} else {
			dataLen = 0;
		}
		byte[] out = new byte[dataLen + 4];
		
		out[0] = (byte) ((2 + dataLen) & 0xff);
		out[1] = (byte) (((2 + dataLen) >> 8) & 0xff);
		out[2] = (byte) (reply ? 0 : 0x80);
		out[3] = command;
		
		if (data != null) {
			for (int i = 0; i < dataLen; i++) {
				out[4 + i] = data[i];
			}
		}
		return out;
	}
	
	private int[] control = new int[3];
	
	byte[] out = new byte[7];
	
	public ProtocolNxtEmbox() {
		out[0] = 5;
		out[1] = 0;
		out[2] = (byte) 0x80;
		out[3] = (byte) 0x20;
	
	}
	
	static protected double angle (int x, int y) {
		return (Math.acos(((double) x) / 100.0) * Math.signum(((double) y) / 100.0)); 
	}
	
	static protected int cut(int val, int range) {
		if (val < -range) {
			return -range;
		}
		if (val > range) {
			return range;
		}
		return val;
	}

	@Override
	public int[] setControl(int[] vals) {
		double fullY = 0.3;
		double fullX = 0.7;
		double ang = angle(vals[0],vals[2]);
		
		int power = cut((int) (100 * (ang - angCalbr) / fullX), 100);
		int mA = cut((int) (power + (((double) vals[1] - calbr[1]) / fullY)), 100);
		int mB = cut((int) (power - (((double) vals[1] - calbr[1]) / fullY)), 100);
		
		control[0] = mA;
		control[1] = mB;

		return this.control;
	}
	
	@Override
	public void calibrate(int[] vals) {
		for (int i = 0; i < 3; i++) {
			calbr[i] = vals[i];
		}
		angCalbr = angle(calbr[0],calbr[2]);
	}
	
	@Override
	public byte[] translateInput(byte[] data) {
		return null;
	}

    @Override
    public void setConfig(OptionMessage.OptionMessageEntity config) {
        //To change body of implemented methods use File | Settings | File Templates.
    }

    @Override
	public byte[] translateOutput(int[] control) {
		out[4] = (byte) control[0];
		out[5] = (byte) control[1];
		out[6] = (byte) 0;
		return out;
	}
}
