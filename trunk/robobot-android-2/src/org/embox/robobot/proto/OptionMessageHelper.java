package org.embox.robobot.proto;

import com.google.protobuf.InvalidProtocolBufferException;

/**
 * Created with IntelliJ IDEA.
 * User: vloginova
 * Date: 22.04.13
 * Time: 16:15
 * To change this template use File | Settings | File Templates.
 */
public class OptionMessageHelper {
    private static final int ROBOBOT_CAR = 0;
    private static final int LEGO_NXT = 1;
    private static final int PYTHON_SIMULATOR = 100;

    public static OptionMessage.OptionMessageEntity getOptionMessage(byte[] data) {
        OptionMessage.OptionMessageEntity message;
        try {
            message = OptionMessage.OptionMessageEntity.parseFrom(data);
            return message;
        } catch (InvalidProtocolBufferException e) {
            //TODO normal logger
            e.printStackTrace();
        }
        return null;
    }

    public static IProtocol getDeviceProtocolByType(int type) {
        switch (type) {
            case ROBOBOT_CAR :
               return new ProtocolRobobotCar();
            case LEGO_NXT :
                return new ProtocolNxtEmbox();
            case PYTHON_SIMULATOR :
                return new ProtocolPythonSimulator();
            default :
                //TODO throw exception
                return null;
        }
    }
}
