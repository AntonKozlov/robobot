package org.embox.robobot.transport;

import android.bluetooth.BluetoothAdapter;
import android.content.BroadcastReceiver;

public class Bluetooth implements ITransport {
	private BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
	public Bluetooth() {
		
	}
	@Override
	public void init() {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void startScan(BroadcastReceiver receiver) {
		mBluetoothAdapter.startDiscovery();
		
	}

}
