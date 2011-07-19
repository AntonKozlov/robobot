package org.embox.robobot;

import java.util.ArrayList;
import java.util.List;

import org.embox.robobot.proto.IProtocol;
import org.embox.robobot.proto.ProtocolNxtEmbox;
import org.embox.robobot.proto.ProtocolRobobotCar;
import org.embox.robobot.transport.BluetoothTransport;
import org.embox.robobot.transport.ITransport;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Message;

public class DeviceManager {
	private ScanDeviceHandler handler;
	private List<IDevice> deviceList = new ArrayList<IDevice>();
	Context context;
	
	ProtocolNxtEmbox mNxtDirect = new ProtocolNxtEmbox();
	
	ProtocolRobobotCar mRobobotCar = new ProtocolRobobotCar();
	
	ITransport bt;
	
	public DeviceManager(Context context, ScanDeviceHandler handler) {
		this.context = context;
		this.handler = handler;
		
		bt = new BluetoothTransport(handler);
	}
	
	List<IDevice> getDeviceList() {
		return deviceList;
	}
	
	private IDevice createDevice(String devId, String name, IProtocol proto, IControllable controllable,
			BluetoothDevice device) {
		for (IDevice dev : deviceList) {
			if (0 == dev.getId().compareTo(devId)) {
				return null;
			}
		}
		IDevice dev = new BtDevice(devId, name, proto, controllable, device); 
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
                //Resources res = context.getResources();
                //if (0 == device.getName().compareTo(res.getString(R.string.nxt_bt_name))) {
                	IDevice foundDevice = createDevice(device.getAddress(), 
        					device.getName(), 
        					mRobobotCar, mRobobotCar,
        					device);
                	if (foundDevice != null) {
                		Message.obtain(handler,ITransport.DEVICE_FOUND, foundDevice).sendToTarget();
                	}
                //}
            // When discovery is finished, change the Activity title
            } else if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED.equals(action)) {
            	Message.obtain(handler,ITransport.SCAN_FINISHED).sendToTarget();
            } else if (BluetoothDevice.ACTION_NAME_CHANGED.equals(action)) {
            	BluetoothDevice device = intent.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);
            	IDevice foundDevice = createDevice(device.getAddress(), 
    					device.getName(), 
    					mNxtDirect, mNxtDirect,
    					device);
            	foundDevice.setName(device.getName());
            	Message.obtain(handler,ITransport.DEVICE_NAME_CHANGED, foundDevice).sendToTarget();
            } else if (BluetoothAdapter.ACTION_STATE_CHANGED.equals(action)) {
            	            	
            }
        }
	};
	 
	public void startFullScan() {
		deviceList.clear();
		startIncrementScan();
	}
	
	public void startIncrementScan() {
		
		// Register for broadcasts when a device is discovered
        IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
        context.registerReceiver(btReceiver, filter);

        // Register for broadcasts when discovery has finished
        filter = new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED);
        context.registerReceiver(btReceiver, filter);

        filter = new IntentFilter(BluetoothDevice.ACTION_NAME_CHANGED);
        context.registerReceiver(btReceiver, filter);

        filter = new IntentFilter(BluetoothAdapter.ACTION_STATE_CHANGED);
        context.registerReceiver(btReceiver, filter);

		bt.startScan(btReceiver);
	}

}
