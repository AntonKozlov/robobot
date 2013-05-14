package org.embox.robobot.proto;

import org.embox.robobot.IControllable;
import org.embox.robobot.proto.ConfigurationMessage.DeviceConfigurationMessage;

public class ProtocolPythonSimulator implements IProtocol, IControllable {

    private ConfigurationMessage.DeviceConfigurationMessage config;
    private int[] control = new int[2];

    public ProtocolPythonSimulator() {
    }

    public ProtocolPythonSimulator(ConfigurationMessage.DeviceConfigurationMessage config) {
        this.config = config;
    }

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
        if (config != null) {
            if ((config.getCommands().byteAt(0) & 1 << 7) != 0)
                return (String.format("1#0#%d#%d\n", control[0], control[1])).getBytes();
        }
        return null;
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

    public void setConfig(ConfigurationMessage.DeviceConfigurationMessage config) {
        this.config = config;
    }

    public ConfigurationMessage.DeviceConfigurationMessage getConfig() {
        return  this.config;
    }
}
