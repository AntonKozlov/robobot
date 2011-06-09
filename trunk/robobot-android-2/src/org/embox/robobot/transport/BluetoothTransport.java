package org.embox.robobot.transport;

import java.util.UUID;

import org.embox.robobot.ScanDeviceHandler;

import android.bluetooth.BluetoothAdapter;
import android.content.BroadcastReceiver;

public class BluetoothTransport implements ITransport {
	public static final UUID BT_UUID = UUID.fromString("00001101-0000-1000-8000-00805F9B34FB");
	private BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
	
	public BluetoothTransport(ScanDeviceHandler handler) {
		if (mBluetoothAdapter.isEnabled() == false) {
			handler.obtainMessage(ITransport.REQUEST_HW_ENABLE, this).sendToTarget();
		}
	}
	
	@Override
	public void startScan(BroadcastReceiver receiver) {
		if (mBluetoothAdapter.isDiscovering()) {
			mBluetoothAdapter.cancelDiscovery();
		}
			
		mBluetoothAdapter.startDiscovery();
		
	}

}
