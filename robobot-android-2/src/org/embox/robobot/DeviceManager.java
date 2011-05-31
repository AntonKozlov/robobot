package org.embox.robobot;

import java.util.ArrayList;
import java.util.List;

import org.embox.robobot.proto.IProto;
import org.embox.robobot.proto.NxtDirect;
import org.embox.robobot.transport.BluetoothTransport;
import org.embox.robobot.transport.ITransport;
import org.embox.robobot.ui.R;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.res.Resources;


public class DeviceManager {
	private IDeviceEventListener listener;
	private List<IDevice> deviceList;
	Context context;
	
	IProto mNxtDirect;
	
	public DeviceManager(Context context) {
		deviceList = new ArrayList<IDevice>();
		mNxtDirect = new NxtDirect();
		this.context = context;
	}
	
	List<IDevice> getDeviceList() {
		return deviceList;
	}
	private IDevice createDevice(String devId, String name, IProto proto) {
		for (IDevice dev : deviceList) {
			if (0 == dev.getId().compareTo(devId)) {
				return dev;
			}
		}
		IDevice dev = new Device(devId, name, proto); 
		deviceList.add(dev);
		return dev;
	}
	
	private final BroadcastReceiver btReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            String action = intent.getAction();
            if (BluetoothDevice.ACTION_FOUND.equals(action)) {
                // Get the BluetoothDevice object from the Intent
                BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
                Resources res = context.getResources();
                if (0 == device.getName().compareTo(res.getString(R.string.nxt_bt_name))) {
                	listener.receive(IDeviceEvent.DEVICE_FOUND, 
                			createDevice(device.getAddress(), device.getName(), mNxtDirect));
                }
            // When discovery is finished, change the Activity title
            } else if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equals(action)) {
            	listener.receive(IDeviceEvent.SCAN_FINISHED, null);
            }
        }
	};
	
	public void startScan(IDeviceEventListener listener) {
		this.listener = listener; 
		ITransport bt = new BluetoothTransport();
		
		// Register for broadcasts when a device is discovered
        IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
        context.registerReceiver(btReceiver, filter);

        // Register for broadcasts when discovery has finished
        filter = new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED);
        context.registerReceiver(btReceiver, filter);

		bt.startScan(btReceiver);
	}

}
