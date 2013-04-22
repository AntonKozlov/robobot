package org.embox.robobot.proto;

import com.google.protobuf.InvalidProtocolBufferException;
import org.embox.robobot.IControllable;
import org.embox.robobot.proto.OptionMessage;

public class ProtocolPythonSimulator implements IProtocol, IControllable {

    int[] control = new int[2];

    //TODO in utils
    static int cut(int val, int range) {
        if (val < -range) {
            return -range;
        }
        if (val > range) {
            return range;
        }
        return val;
    }

	@Override
	public byte[] translateOutput(int[] control) {
		    return (String.format("%d %d\n", control[0], control[1])).getBytes();
	}

	@Override
	public byte[] translateInput(byte[] data) {
        return null;
    }

	@Override
	public int[] setControl(int[] control) {
        double power = (double) control[1] * 10;
        double turn = (double) control[0] * 10;

        this.control[0] = cut((int)power, 100);
        this.control[1] = cut((int)turn, 100);

        return this.control;
	}

	@Override
	public void calibrate(int[] control) {
        return;
	}
}
