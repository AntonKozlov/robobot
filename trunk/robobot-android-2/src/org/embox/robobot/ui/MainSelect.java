package org.embox.robobot.ui;

import java.util.ArrayList;

import org.embox.robobot.DeviceHandler;
import org.embox.robobot.DeviceManager;
import org.embox.robobot.IDevice;
import org.embox.robobot.transport.ITransport;

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Toast;

public class MainSelect extends Activity {
    private Button mRescanButton;
    private ListView mFoundDevicesList;
    private ArrayAdapter<String> mFoundDeviceAdapter;
	private DeviceManager deviceManager = new DeviceManager((Context) this);
	private ArrayList<IDevice> deviceList = new ArrayList<IDevice>();
	Handler mHandler = new Handler();
	
	private class selectDeviceHandler extends DeviceHandler  {
		@Override
		public void handleMessage(Message msg) {
			switch (msg.what) {
				case ITransport.DEVICE_FOUND:
					IDevice device = (IDevice) msg.obj;
					mFoundDeviceAdapter.add(device.getId());
					deviceList.add(device);
					break;
				case ITransport.SCAN_FINISHED:
					break;
				default:
					break;
			}
		}
	}
	
	selectDeviceHandler mDeviceHandler = new selectDeviceHandler();
	
	private class DeviceListClickListener implements OnItemClickListener {

		@Override
		public void onItemClick(AdapterView<?> parent, View view, int position,
				long id) {
			Toast.makeText(getApplicationContext(),deviceList.get(position).getName(),Toast.LENGTH_LONG).show();
			
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
        
        mRescanButton.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View arg0) {
				
				deviceManager.startScan(mDeviceHandler);
			}
		});
    }
}