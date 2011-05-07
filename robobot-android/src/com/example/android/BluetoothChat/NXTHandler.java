package com.example.android.BluetoothChat;

import java.lang.reflect.Array;

import android.text.format.Time;
import android.util.Log;
import android.widget.ArrayAdapter;

public class NXTHandler {
	
	final float fullX = (float) 0.7;
	final float fullY = (float) 0.7;
	float calibrate;
	
	BluetoothChatService mChatService;
	BluetoothChat mbBluetoothChat;
	
	public NXTHandler(BluetoothChatService mChatService, BluetoothChat mBluetoothChat) {
		this.mChatService = mChatService;
		this.mbBluetoothChat = mBluetoothChat;
				
	}
	
	private float cut(float val, float range) {
		if (val < -range) {
			return -range;
		}
		if (val > range) {
			return range;
		}
		return val;
	}
	
	private byte[] write(int mA, int mB, int mC) {
		Log.d("NXTHandler", "Motor:" + Integer.toString(mA) + " " 
				+ Integer.toString(mB));
		byte[] out = new byte[7];
		out[0] = 5;
		out[1] = 0;
		out[2] = (byte) 0x80;
		out[3] = (byte) 0x20;
		out[4] = (byte) mA;
		out[5] = (byte) mB;
		out[6] = (byte) mC;
		return out;
	}
	
	private byte[] updateMotor(float x, float y) {
		
		Float power = 100 * cut((x - calibrate) / fullX, 1);
		Float mA = cut(power + 100 * y / fullY, 100);
		Float mB = cut(power - 100 * y / fullY, 100);
		return write(mA.intValue(), mB.intValue(), 0);
	}
	
	static public float angle (float x, float y) {
		return (float) Math.acos(x) * Math.signum(y); 
		
		
	}
	
	public void calibrate(float x, float z) {
		calibrate = angle(x, z);
	}
	
	public void update(float x, float y) {
		mChatService.write(updateMotor(x, y));
	}

	public void transmitMode(Boolean isTransmit) {
		if (!isTransmit) {
			update(0, 0);
		}
		
	}

	public void handle(byte[] writeBuf) {
		String str = new String();
		for (int i = 0; i < writeBuf.length && i < 16; i++) {
			str += ":" + Integer.toHexString(writeBuf[i]);
		}
		BluetoothChat.setStatus(str);
	}
	
	public void askSensor(byte sensor) {
		byte[] out = {3, 0, 0x00, 0x07, 0x00};
		mChatService.write(out);
	}
}
