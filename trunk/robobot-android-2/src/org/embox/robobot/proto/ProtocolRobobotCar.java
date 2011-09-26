package org.embox.robobot.proto;

import org.embox.robobot.IControllable;

public class ProtocolRobobotCar extends ProtocolNxtEmbox implements IProtocol, IControllable {
	
	int[] control = new int[3];
	
	@Override
	public byte[] translateOutput(int[] control) {
		byte[] ret = new byte[4];
		ret[0] = (byte) 0x42;
		ret[1] = (byte) 0x24;
		ret[2] = (byte) super.cut(control[0], 100);
		ret[3] = (byte) super.cut(control[1], 100);
		return ret;
	}
	
	@Override
	public int[] setControl(int[] vals) {
		double fullY = 0.3;
		double fullX = 0.7;
		double ang = angle(vals[0],vals[2]);
		
		int power = cut((int) (100 * (ang - angCalbr) / fullX), 100);
		int turn = cut((int) (((double) vals[1] - calbr[1]) / fullY), 100);
		
		control[0] = power;
		control[1] = turn;
		
		return this.control;
	}
}
