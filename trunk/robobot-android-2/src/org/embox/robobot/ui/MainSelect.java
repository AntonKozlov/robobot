package org.embox.robobot.ui;


import java.util.ArrayList;

import org.embox.robobot.ScanDeviceHandler;
import org.embox.robobot.DeviceManager;
import org.embox.robobot.IDevice;

import org.embox.robobot.ui.R;
import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;


public class MainSelect extends Activity {
    private Button mRescanButton;
    private ListView mFoundDevicesList;
    private ArrayAdapter<String> mFoundDeviceAdapter;
	private DeviceManager deviceManager = new DeviceManager((Context) this);
	private ArrayList<IDevice> deviceList = new ArrayList<IDevice>();
	
	public static IDevice choosedDevice;
	
	public static IDevice getChoosedDevice() {
		return choosedDevice;
	}
	
	ScanDeviceHandler mDeviceHandler;
	
	private class DeviceListClickListener implements OnItemClickListener {

		@Override
		public void onItemClick(AdapterView<?> parent, View view, int position,
				long id) {
			//Toast.makeText(getApplicationContext(),deviceList.get(position).getName(),Toast.LENGTH_LONG).show();
			choosedDevice = deviceList.get(position);
			Intent intent = new Intent(MainSelect.this, ControlActivity.class);
			startActivity(intent);
			
		}
		
	}
	/** Called when the activity is first created. */
	@Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        setContentView(R.layout.devices);
        mFoundDevicesList = (ListView) findViewById(R.id.found_devices_list);
        mFoundDeviceAdapter = new ArrayAdapter<String>(this, android.R.layout.simple_list_item_1);
        mFoundDevicesList.setAdapter(mFoundDeviceAdapter);
        mFoundDevicesList.setOnItemClickListener(new DeviceListClickListener());
        mRescanButton = (Button) findViewById(R.id.button_scan);
        
        mDeviceHandler = new ScanDeviceHandler() {
    		protected void deviceFound(IDevice device) {
    			mFoundDeviceAdapter.add(device.getId());
    			deviceList.add(device);
    		};
    	};
        
        mRescanButton.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View arg0) {
				
				deviceManager.startScan(mDeviceHandler);
			}
		});
        
        deviceManager.startScan(mDeviceHandler);
        }
}