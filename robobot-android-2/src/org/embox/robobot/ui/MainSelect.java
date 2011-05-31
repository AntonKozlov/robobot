package org.embox.robobot.ui;

import org.embox.robobot.ui.R;
import org.embox.robobot.DeviceManager;
import org.embox.robobot.IDevice;
import org.embox.robobot.IDeviceEvent;
import org.embox.robobot.IDeviceEventListener;

import android.app.Activity;
import android.content.Context;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;

public class MainSelect extends Activity {
    private Button mRescanButton;
    private ListView mFoundDevicesList;
    private ArrayAdapter<String> mFoundDeviceAdapter;
	private DeviceManager deviceManager = new DeviceManager((Context) this);
	private ScanDeviceListener listener = new ScanDeviceListener();;
	private class ScanDeviceListener implements IDeviceEventListener {
		
		@Override
		public void receive(int event, IDevice device) {
			switch(event) {
				case IDeviceEvent.DEVICE_FOUND:
					mFoundDeviceAdapter.add(device.getId());
					break;
				case IDeviceEvent.SCAN_FINISHED:
					break;
				default:
					break;			
			}			
		}		
	}
	/** Called when the activity is first created. */
	@Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);
        setContentView(R.layout.devices);

        mFoundDevicesList = (ListView) findViewById(R.id.found_devices_list);
        mFoundDeviceAdapter = new ArrayAdapter<String>(this, R.id.found_devices_list);
        mFoundDevicesList.setAdapter(mFoundDeviceAdapter);
        
        mRescanButton = (Button) findViewById(R.id.button_scan);
        
        mRescanButton.setOnClickListener(new OnClickListener() {
			
			@Override
			public void onClick(View arg0) {
				deviceManager.startScan(listener);
			}
		});
    }
}